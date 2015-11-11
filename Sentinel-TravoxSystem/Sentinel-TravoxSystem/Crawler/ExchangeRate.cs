using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Travox.Sentinel.Engine;
using Travox.Systems;


namespace Travox.Sentinel.Crawler
{
    public class ExchangeRate : Controller
    {
        public ExchangeRate()
        {
            base.OnceTime = true;
            base.SetIntervel = new TimeSpan(1, 0, 0);
        }

        public override void Start()
        {

            base.Start();
        }

        public override void Update()
        {
            String PatturnRate = @"<item><title>(?<from_desc>.*?)\((?<from>\w{3})\)/(?<to_desc>.*?)\((?<to>\w{3})\).*?</title>.*?";
            PatturnRate += @"<pubDate>(?<date>.*?)</pubDate>.*?<description>.*?=.*?(?<rate>[\d|.]+).*?</description>.*?</item>";

            String RateRSS = XHR.Connect("thb.fxexchangerate.com/rss.xml");

            DB db = new DB("travox_global");

            try
            {
                foreach (Match item in Regex.Matches(RateRSS.ToString(), PatturnRate, RegexOptions.Singleline))
                {
                    String UpdateDate = Regex.Replace(item.Groups["date"].Value, "\r\n", " ");
                    CultureInfo culture = CultureInfo.CurrentCulture;
                    DateTimeStyles style = DateTimeStyles.AssumeUniversal;
                    DateTime date = DateTime.ParseExact(UpdateDate.Trim(), "ddd MMM d yyyy H:m:s UTC", culture, style);

                    SQLCollection param = new SQLCollection();
                    param.Add("@to", DbType.String, item.Groups["to"].Value.Trim());
                    param.Add("@to_desc", DbType.String, item.Groups["to_desc"].Value.Trim());
                    param.Add("@rate", DbType.Decimal, item.Groups["rate"].Value.Trim());
                    param.Add("@date", DbType.DateTime, date.ToString("dd-MM-yyyy HH:mm:ss"));

                    db.Execute("UPDATE currency SET currency_rate=@rate, last_update=@date WHERE currency = @to", param);
                }
                db.Apply();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw new Exception(e.Message, new Exception(RateRSS.ToString()));
            }

            RateRSS = null;
            base.Update();
        }

        public override void Stop()
        {
            base.Stop();
        }


    }
}
