using System;
using System.Linq;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using PCP.Models;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.Threading.Tasks;
using System.IO;


namespace PCP.Controllers
{
    public class StudentController : Controller
    {
        static string connectionString = @"mongodb://pcpUser:pcpUser1234@localhost:27017/pcp";
        static MongoClientSettings settings = MongoClientSettings.FromUrl(
                new MongoUrl(connectionString)
            );
        MongoClient client = new MongoClient(settings);

        private IFaceServiceClient faceClient = new FaceServiceClient("3ce50159bc674b818c7a47756f8f9f61");

        [HttpPost]
        public string Login(string data)
        {
            var db = client.GetDatabase("pcp");
            var students = db.GetCollection<Student>("student");
            Login temp = JsonConvert.DeserializeObject<Login>(data);
            string json = null;
            BsonDocument filter = new BsonDocument("_id", temp.username);
            try
            {
                var t = students.Find(filter).ToList();
                if (t.Count == 1)
                {
                    if (t[0].Password == temp.password)
                    {
                        json = JsonConvert.SerializeObject(t[0]);
                        var update = new BsonDocument { { "$set", new BsonDocument("AuthToken",temp.AuthToken) } };
                        var result = students.UpdateOne(filter, update);
                    }
                    else
                    {
                        BsonDocument b = new BsonDocument("isPresent", true);
                        json = b.ToJson();
                    }
                }
                else
                {
                    BsonDocument b = new BsonDocument("isPresent", false);
                    b.Add("RequestUsername", temp.username);
                    b.Add("RequestPassword", temp.password);
                    b.Add("Number of Users", t.Count);
                    json = b.ToJson();
                }
            }
            catch (Exception e)
            {
                BsonDocument b = new BsonDocument("isPresent", false);
                b.Add("Error", e.Message);
                json = b.ToJson();
            }

            return (json);
        }

        public string GetNews(string data)
        {
            NewsReq n = JsonConvert.DeserializeObject<NewsReq>(data);
            var db = client.GetDatabase("pcp");
            var news = db.GetCollection<NewsItem>("news");
            string json = null;

            var filter = new BsonDocument("CourseId", n.CourseId);
            filter.Add("CentreId", n.CentreId);

            try
            {
                var result = news.Find(filter).ToList();
                json = "[";
                int t = result.Count;
                for (int i = 0; i < t; i++)
                {
                    json += JsonConvert.SerializeObject(result[i]);
                    if (i != t - 1)
                        json += ",";
                }
                json += "]";
                
            }
            catch(Exception)
            {

            }
            
            return json;
        }

        [HttpPost]
        public async Task<string> VerifyFace(string data)
        {
            FaceLoginReq frq = JsonConvert.DeserializeObject<FaceLoginReq>(data);
            string imagePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/Images/" + frq.Personid + ".jpg");
            try
            {
                using (Stream imageFileStream = System.IO.File.OpenRead(imagePath))
                {
                    var faces = await faceClient.DetectAsync(imageFileStream);
                    Guid faceid1 = Guid.Parse(frq.FaceId);
                    Guid faceid2 = faces[0].FaceId;
                    var result = await faceClient.VerifyAsync(faceid1, faceid2);
                    if (result.IsIdentical)
                    {
                        var db = client.GetDatabase("pcp");
                        var student = db.GetCollection<Student>("student");

                        var res = student.Find(new BsonDocument("_id", frq.Personid)).ToList();
                        return JsonConvert.SerializeObject(res[0]);
                    }
                    else
                    {
                        return "Not Verified";
                    }
                }
            }
            catch (Exception)
            {
                return "Failed";
            }
        }
    }
}