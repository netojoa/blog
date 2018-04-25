using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Feature.ElasticSearch
{

    using Sitecore.Data;

    public struct Templates
    {
        public struct NewsArticle
        {
            public static readonly ID ID = new ID("{B69277AD-E917-4B9F-9136-A12E0A3E462F}");

            public struct Fields
            {
                public static readonly ID Title = new ID("{BD9ECD4A-C0B0-4233-A3CD-D995519AC87B}");
                public const string Title_FieldName = "NewsTitle";

                public static readonly ID Image = new ID("{3437EAAC-6EE8-460B-A33D-DA1F714B5A93}");

                public static readonly ID Date = new ID("{C464D2D7-3382-428A-BCDF-0963C60BA0E3}");

                public static readonly ID Summary = new ID("{9D08271A-1672-44DD-B7EF-0A6EC34FCBA7}");
                public const string Summary_FieldName = "NewsSummary";

                public static readonly ID Body = new ID("{801612C7-5E98-4E3C-80D2-A34D0EEBCBDA}");
                public const string Body_FieldName = "NewsBody";
            }
        }

    }

}