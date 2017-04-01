using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace PCP.Models
{
    public class Student
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
        public string Country { get; set; }
        public string ParentName { get; set; }
        public string CentreId { get; set; }
        public string CourseId { get; set; }
        public string AuthToken { get; set; }
        public string ISOtemplate { get; set; }
    }
}