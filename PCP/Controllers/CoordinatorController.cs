using System;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using PCP.Models;
using FirebaseNet.Messaging;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace PCP.Controllers
{
    public class CoordinatorController : Controller
    {
        static string connectionString = @"mongodb://pcpUser:pcpUser1234@localhost:27017/pcp";

        static MongoClientSettings settings = MongoClientSettings.FromUrl(
                new MongoUrl(connectionString)
            );
        MongoClient client = new MongoClient(settings);
        FCMClient fcmClient = new FCMClient("AAAAZlhBhI4:APA91bEfE8OxHapi-WYgoeuywYNXALeJPkTKn9tr4-RKln0hassN7BbIg_zhBiuK82rqP4K1F-oqO1j9uVPnvBCzO5-0dnmW9aWl1_vys25I38laWC9SqPasICL6HAu2xvkHBZxMxLPp");

        [HttpPost]
        public string CoordinatorLogin(string data)
        {
            var db = client.GetDatabase("pcp");
            var coordinators = db.GetCollection<Coordinator>("coordinator");
            Login temp = JsonConvert.DeserializeObject<Login>(data);
            string json = null;
            BsonDocument filter = new BsonDocument("_id", temp.username);
            try
            {
                var t = coordinators.Find(filter).ToList();
                if (t.Count == 1)
                {
                    if (t[0].Password == temp.password)
                        json = JsonConvert.SerializeObject(t[0]);
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

        [HttpPost]
        public async Task SendFirebaseNotification(string tokens, string msg, string title)
        {
            string[] Tokens = tokens.Split(',');
            foreach (string t in Tokens)
            {
                var message = new Message()
                {
                    To = t,
                    Notification = new AndroidNotification()
                    {
                        Title = title,
                        Body = msg
                    }
                };

                var result = await fcmClient.SendMessageAsync(message);
            }
        }

        [HttpPost]
        public async Task SendNotification(string nq_data)
        {
            NotificationReq nq = JsonConvert.DeserializeObject<NotificationReq>(nq_data);
            var db = client.GetDatabase("pcp");
            var student = db.GetCollection<Student>("student");
            string data = null;
            var filter = new BsonDocument("CentreId", nq.centreid);
            filter.Add("CourseId", nq.courseid);

            var result = student.Find(filter).ToList();
            int count = result.Count;
            for(int i=0;i<count;i++)
            {
                data = data + result[i].AuthToken + ",";
            }
            await SendFirebaseNotification(data, nq.msg, nq.title);   
        }

        [HttpPost]
        public string AddNews(string data)
        {
            News n = JsonConvert.DeserializeObject<News>(data);
            NewsItem ni = new NewsItem();
            ni.date = DateTime.Now;
            ni.CentreId = n.CentreId;
            ni.CourseId = n.CourseId;
            try
            {

                var db = client.GetDatabase("pcp");
                var newsCollection = db.GetCollection<NewsItem>("news");
                ni.News = n.NewsMsg;
                newsCollection.InsertOne(ni);

                NotificationReq nq = new NotificationReq();
                nq.centreid = n.CentreId;
                nq.courseid = n.CourseId;
                nq.msg = n.NewsMsg;
                nq.title = ni.NewsTitle;

                var result = SendNotification(JsonConvert.SerializeObject(nq));

                return "true";
            }
            catch(Exception)
            {
                return "false";
            }
        } 

    }
}