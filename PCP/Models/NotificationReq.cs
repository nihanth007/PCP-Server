using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCP.Models
{
    public class NotificationReq
    {
        public string centreid { get; set; }
        public string courseid { get; set; }
        public string msg { get; set; }
        public string title { get; set; }
    }
}