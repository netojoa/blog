using System;

namespace ElasticSearchDemo
{
    public class Article
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }
        public DateTime DatePublished { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }

}
