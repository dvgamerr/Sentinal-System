using System;
using System.Data;
using System.Threading.Tasks;
using Travox.Sentinel.Engine;
using Travox.Systems;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using Travox.Systems.DataCollection;
using Travox.Systems.Security;

namespace Travox.Sentinel.Crawler
{
    public class Tirkx : Controller
    {
        String Username;
        String Password;
        XHR Trikx;

        public Tirkx()
        {
            Username = "dvgamer";
            Password = MD5.Encrypt("dvg7po8ai");
            base.SetTimeout = new TimeSpan(23, 0, 0);
            base.OnceTime = true;
        }
        
        public override void Start()
        {
            // LOGIN 
            
            RequestBuilder doLogin = new RequestBuilder("forum.tirkx.com/main/login.php?do=login");
            doLogin.By = RequestBuilder.Method.POST;
            doLogin.CacheControl = "max-age=0";
            doLogin.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            doLogin.Origin = "http://forum.tirkx.com";
            doLogin.ContentType = "application/x-www-form-urlencoded";
            doLogin.Referer = "http://forum.tirkx.com/main/forum.php";

            doLogin.POST("vb_login_username", Username);
            doLogin.POST("vb_login_password", "");
            doLogin.POST("vb_login_password_hint", "Password");
            doLogin.POST("cookieuser", "1");
            doLogin.POST("s", "");
            doLogin.POST("securitytoken", "guest");
            doLogin.POST("do", "login");
            doLogin.POST("vb_login_md5password", Password);
            doLogin.POST("vb_login_md5password_utf", Password);

            Trikx = new XHR();
            //Trikx.AsyncSend(doLogin, true);

            //Trikx.AsyncReceive();
            //Trikx.WaitResponse();

            doLogin.Clear();

            base.Start();
        }

        public override void Update()
        {
            //Trikx.AsyncSend(new RequestBuilder("forum.tirkx.com/main/tirkx_anime_list_home.php"), true);
            //Trikx.AsyncReceive();
            //Trikx.WaitResponse();

            foreach (Match item in Regex.Matches(Trikx.HTMLBody.ToString(), @"(?<locate>.*?)\$(?<thread>\d+)\$(?<filename>.*?)\$(?<language>.*?)\$(?<stream>\w+)`"))
            {

            }
            base.Update();
        }

        public override void Stop()
        {

            base.Stop();
        }


        // 
    }
}
