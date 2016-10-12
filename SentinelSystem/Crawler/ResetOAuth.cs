using System;
using System.Data;
using System.Threading.Tasks;
using Travox.Sentinel.Engine;
using Travox.Systems;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using Travox.Systems.DataCollection;
using Travox.Systems.Security;

namespace Travox.Sentinel.Crawler
{
    public class ResetOAuth : Controller
    {
        IMongoCollection<BsonDocument> oAuth;
        IMongoDatabase db;

        public ResetOAuth()
        {
 
        }

        public override async void Start()
        {
            base.OnceTime = true;
            // Standard URI format: mongodb://[dbuser:dbpassword@]host:port/dbname
            String uri = "mongodb://[petahost2.ns.co.th:27017/travox-mbos";

            var client = new MongoClient(uri);
            db = client.GetDatabase("travox-mbos");
            oAuth = db.GetCollection<BsonDocument>("oAuth");

            await oAuth.Find(filter).ForEachAsync(song =>
              Console.WriteLine("In the {0}, {1} by {2} topped the charts for {3} straight weeks",
                song["Decade"], song["Title"], song["Artist"], song["WeeksAtOne"])
            );


            base.Start();
        }

        public override async void Update()
        {
            var updateFilter = Builders<BsonDocument>.Filter.Eq("Title", "One Sweet Day");
            var update = Builders<BsonDocument>.Update.Set("Artist", "Mariah Carey ft. Boyz II Men");

            await oAuth.UpdateOneAsync(updateFilter, update);
        }

        public override void Stop()
        {

            base.Stop();
        }


        // 
    }
}
