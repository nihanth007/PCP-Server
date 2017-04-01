using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace PCP.Models
{
    public class NewsItem
    {
        public ObjectId _id { get; set; }
        public string NewsTitle { get; set; }
        public string CourseId { get; set; }
        public string CentreId { get; set; }
        public string News { get; set; }
        public DateTime date { get; set; }
    }

    public class News
    {
        public string NewsTitle { get; set; }
        public string NewsMsg { get; set; }
        public string CourseId { get; set; }
        public string CentreId { get; set; }

    }
}