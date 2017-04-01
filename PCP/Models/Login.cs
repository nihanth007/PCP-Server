using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCP.Models
{
    public class Login
    {
        public string username { get; set; }
        public string password { get; set; }
        public string AuthToken { get; set; }
    }
}