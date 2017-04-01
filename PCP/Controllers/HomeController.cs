using System;
using System.Web.Mvc;
using MongoDB.Driver;
using PCP.Models;

namespace PCP.Controllers
{
    public class HomeController : Controller
    {
        static string connectionString = @"mongodb://pcpUser:pcpUser1234@localhost:27017/pcp";
        static MongoClientSettings settings = MongoClientSettings.FromUrl(
                new MongoUrl(connectionString)
            );
        MongoClient client = new MongoClient(settings);
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StudentRegister()
        {
            return View();
        }

        public string Register(Student s)
        {
            var db = client.GetDatabase("pcp");
            var students = db.GetCollection<Student>("student");
            try
            {
                students.InsertOne(s);
                return "Success";
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
    }
}