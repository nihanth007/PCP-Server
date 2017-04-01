using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCP.Models
{
    public class Coordinator
    {
        public string id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string CurrentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public object Country { get; set; }
        public string ParentName { get; set; }
        public string CenterId { get; set; }
        public string AuthToken { get; set; }
    }
}