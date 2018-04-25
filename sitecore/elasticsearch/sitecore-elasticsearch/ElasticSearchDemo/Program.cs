using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchDemo
{
    using System;
    using Nest;

    class Program
    {
        static void Main(string[] args)
        {

            // Connecting to Elasticsearch
            var node = new Uri("http://elastic.local:9200");
            var settings = new ConnectionSettings(node);
            var client = new ElasticClient(settings);

            // Creating an index
            string indexName = "articles-index";
            var createResponse = client.CreateIndex(indexName);
            System.Console.WriteLine("The index {0} - has been created? {1}", indexName, createResponse.Acknowledged);

            // Indexing a document
            var article = new Article
            {
                Id = 1,
                Author = "kitty",
                Title = "Lorem ipsum dolor sit amet",
                Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed aliquam feugiat est ut condimentum.",
                DateCreated = new DateTime(2016, 12, 01),
                DateModified = new DateTime(2016, 12, 15),
                DatePublished = new DateTime(2016, 12, 20),
                Url = "https://goo.gl/tvcMLp"
            };

            var indexResponse = client.Index(article, x => x.Index(indexName));

            // Getting a document
            var getResponse = client.Get<Article>(1, i => i.Index(indexName));
            var articleDocument = getResponse.Source; // the original document

            var searchResponse = client.Search<Article>(s => 
                s.Index("articles-index").Size(10).Query(q => 
                q.Match(m => m.Field(f => 
                f.Author).Query("kitty"))));

            var articles = searchResponse.Documents;


            System.Console.ReadLine();

        }
    }
}
