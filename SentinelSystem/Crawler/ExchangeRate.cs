using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Travox.Sentinel.Engine;
using Travox.Systems;
using Travox.Systems.DataCollection;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Travox.Sentinel.Crawler
{
    public class ExchangeRate : Controller
    {
        public ExchangeRate()
        {
            base.OnceTime = true;
            base.SetIntervel = new TimeSpan(6, 0, 0);
        }

        public override void Start()
        {

            base.Start();
        }

        [DataContract]
        struct RateAPI
        {
            [DataMember]
            public String updated;
            [DataMember]
            public String currency;
            [DataMember]
            public float rate;
        }

        public override void Update()
        {
            DB db = new DB("travox_global"); 

            RequestBuilder doExchange = new RequestBuilder("api.travox.com/API-v3/exchange-rate/");
            doExchange.By = RequestBuilder.Method.POST;
            doExchange.ContentType = "application/x-www-form-urlencoded";
            doExchange.AddHeader("Token-Auth", "ZHNnc2RmaCxrZXIgbmFsZ25zIGRmZ2RzZmc");

            doExchange.AddBody("from", db.GetField("SELECT ISNULL(currency,'') FROM currency FOR XML PATH('')"));
            doExchange.AddBody("to", "THB");
            doExchange.AddBody("amt", "1");

            XHR rate = new XHR().AsyncSend(doExchange).Wait();

            try
            {
                foreach (RateAPI item in JsonConvert.DeserializeObject<List<RateAPI>>(rate.ToString()))
                {
                    SQLCollection param = new SQLCollection();
                    param.Add("@to", DbType.String, item.currency);
                    param.Add("@rate", DbType.Decimal, item.rate);
                    param.Add("@date", DbType.DateTime, DateTime.Parse(item.updated).ToString("dd-MM-yyyy HH:mm:ss"));

                    db.Execute("UPDATE currency SET currency_rate=@rate, last_update=@date WHERE currency = @to", param);
                }
                db.Apply();
                base.Update();
            }
            catch (Exception e)
            {
                db.Rollback();
                base.Update();
                throw new Exception(base.DBName, e);
            }
        }

        public override void Stop()
        {
            base.Stop();
        }


    }
}
