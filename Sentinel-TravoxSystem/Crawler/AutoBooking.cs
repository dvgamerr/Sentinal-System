using Travox.Sentinel.Engine;
using Travox.Systems;

namespace Travox.Sentinel.Crawler
{
    public class AutoBooking : Controller
    {
        // GD::db2.ns.co.th On local Please use IP: 192.168.20.3 / db9.ns.co.th


        string MBOSConnection = "server=db3.ns.co.th;database={0};uid=travoxmos;pwd=systrav; connect timeout=30;";

        public AutoBooking()
        {
            //base.SetIntervel = new TimeSpan(0, 1, 0);
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Stop()
        {
            base.Stop();
        }


    }
}
