namespace Demo.Feature.ElasticSearch.Infrastructure.Commands
{

    using Demo.Feature.ElasticSearch.Models;
    using Nest;
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Jobs;
    using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
    using Sitecore.Shell.Framework.Commands;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class RefreshTree : Command
    {

        protected Handle JobHandle { get; set; }

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            var item = context.Items[0];

            Assert.IsNotNull(item, "context item cannot be null");

            var progressBoxMethod = new ProgressBoxMethod(Refresh);

            ProgressBox.Execute(
                string.Format("{0} ({1})", "Re-Index Elastic Tree.", item.Paths.ContentPath),
                "Re-indexing the current item and its descendants in Elastic",
                progressBoxMethod,
                new object[] { item });
        }

        private void Refresh(object[] parameters)
        {

            JobHandle = Sitecore.Context.Job.Handle;

            if (parameters.Length != 1)
                return;

            var contextItem = parameters[0] as Item;

            if (contextItem == null)
                return;

            var webDb = Database.GetDatabase("web");
            var job = JobManager.GetJob(JobHandle);

            var items = new List<Item>();
            if (contextItem.TemplateID == Templates.NewsArticle.ID)
                items.Add(contextItem);

            items.AddRange(contextItem.Axes.GetDescendants()
                .Where(s => s.TemplateID == Templates.NewsArticle.ID));

            List<NewsArticle> newsArticles = items.Select(news => new NewsArticle
            {
                Id = news.ID.ToString(),
                Body = news[Templates.NewsArticle.Fields.Body],
                Date = news[Templates.NewsArticle.Fields.Date],
                Image = news[Templates.NewsArticle.Fields.Image],
                Summary = news[Templates.NewsArticle.Fields.Summary],
                Title = news[Templates.NewsArticle.Fields.Title]
            }).ToList();

            if (job != null)
                job.Status.Messages.Add(string.Format("Indexing: {0} entries", newsArticles.Count));

            var response = IndexNews(newsArticles);

            if (response != null)
                job.Status.Messages.Add(string.Format("Indexing result: {0}", response.DebugInformation));

        }

        private IBulkResponse IndexNews(IEnumerable<NewsArticle> newsArticles)
        {

            if (newsArticles == null || !newsArticles.Any())
                return null;

            // Connecting to Elasticsearch
            string protocol = Settings.GetSetting("ElasticSearch.Protocol", "http");
            string host = Settings.GetSetting("ElasticSearch.Host", "elastic.local");
            string port = Settings.GetSetting("ElasticSearch.Port", "9200");

            var node = new Uri(string.Format("{0}://{1}:{2}", protocol, host, port));
            var settings = new Nest.ConnectionSettings(node);
            var client = new Nest.ElasticClient(settings);

            // Reindexing items
            var indexName = Settings.GetSetting("ElasticSearch.ArticlesIndex", "articles-index");

            var indexerResponse = client.IndexMany(newsArticles, indexName);

            return indexerResponse;
        }

    }
}