using System;
using System.Data;
using Travox.Sentinel.Engine;
using Travox.Systems;

namespace Travox.Sentinel.Crawler
{
    public class MidBackStored : Controller
    {
        public MidBackStored()
        {
            base.OnceTime = true;
            base.SetIntervel = new TimeSpan(0, 10, 0);
        }

        public override void Update()
        {
            DB db = new DB("travox_system");
            db.StoredProcedure("sentinel.booking_status_finish");
            db.StoredProcedure("sentinel.booking_status_payment");
            db.StoredProcedure("sentinel.session_status");
            base.Update();
        }
    }
}
