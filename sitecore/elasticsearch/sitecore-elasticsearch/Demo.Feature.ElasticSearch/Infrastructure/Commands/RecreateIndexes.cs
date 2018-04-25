using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
namespace Demo.Feature.ElasticSearch.Infrastructure.Commands
{

    using Sitecore.Diagnostics;
    using Sitecore.Jobs;
    using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
    using Sitecore.Shell.Framework.Commands;
    using System;

    [Serializable]
    public class ReCreateIndexes : Sitecore.Shell.Framework.Commands.Command
    {

        protected Handle JobHandle { get; set; }

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            var item = context.Items[0];

            Assert.IsNotNull(item, "context item cannot be null");

            var progressBoxMethod = new ProgressBoxMethod(Recreate);

            ProgressBox.Execute(
                "Recreate Elastic Indexes.",
                "Recreating all Elastic indexes.",
                progressBoxMethod,
                new object[] { item });

        }

        private void Recreate(params object[] parameters)
        {
            JobHandle = Context.Job.Handle;

            if (parameters.Length != 1)
                return;

            var item = parameters[0] as Item;

            if (item == null)
                return;

            var job = JobManager.GetJob(JobHandle);

            // Connecting to Elasticsearch
            string protocol = Settings.GetSetting("ElasticSearch.Protocol", "http");
            string host = Settings.GetSetting("ElasticSearch.Host", "elastic.local");
            string port = Settings.GetSetting("ElasticSearch.Port", "9200");

            var node = new Uri(string.Format("{0}://{1}:{2}", protocol, host, port));
            var settings = new Nest.ConnectionSettings(node);
            var client = new Nest.ElasticClient(settings);

            // Re-creating index
            var indexName = Settings.GetSetting("ElasticSearch.ArticlesIndex", "articles-index");

            DisplayStatusMessage(job, string.Format("Deleting '{0}' index", indexName));
            var deleteResponse = client.DeleteIndex(indexName);
            DisplayStatusMessage(job, string.Format("The index {0} - has been deleted? - {1}", indexName, deleteResponse.Acknowledged));

            DisplayStatusMessage(job, string.Format("Creating '{0}' index", indexName));
            var createResponse = client.CreateIndex(indexName);
            DisplayStatusMessage(job, string.Format("The index {0} - has been created? {1}", indexName, createResponse.Acknowledged));

        }

        private void DisplayStatusMessage(Job job, string message)
        {
            if (job != null)
                job.Status.Messages.Add(message);
        }
    }
}