using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travox.Sentinel.Engine;
using Travox.Systems;
using Travox.Systems.DataCollection;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace Travox.Sentinel.Crawler
{
    public class GDS : Controller
    {
        public GDS()
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
