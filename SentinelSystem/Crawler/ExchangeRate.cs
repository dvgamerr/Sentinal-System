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
            base.SetIntervel = new TimeSpan(6, 0, 0);
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
            if (State.CompanyCode == "MOS")
            {
                DB db = new DB("travox_global");

                RequestBuilder doExchange = new RequestBuilder("https://api.travox.com/API-v3/exchange-rate/")
                {
                    Method = RequestBuilder.By.POST,
                    ContentType = "application/x-www-form-urlencoded"
                };

                doExchange.Headers.Add("Token-Auth", "ZHNnc2RmaCxrZXIgbmFsZ25zIGRmZ2RzZmc");

                doExchange.AddBody("from", db.GetField("SELECT ISNULL(currency,'') FROM currency FOR XML PATH('')"));
                doExchange.AddBody("to", "THB");
                doExchange.AddBody("amt", "1");

                try
                {
                    String res = XHR.Request(doExchange, true);

                    foreach (RateAPI item in JsonConvert.DeserializeObject<List<RateAPI>>(res.ToString()))
                    {
                        SQLCollection param = new SQLCollection
                        {
                            { "@to", DbType.String, item.currency },
                            { "@rate", DbType.Decimal, item.rate },
                            { "@date", DbType.DateTime, DateTime.Parse(item.updated).ToString("dd-MM-yyyy HH:mm:ss") }
                        };
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
        }

        public override void Stop()
        {
            base.Stop();
        }


    }
}
