using System;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Text;
using Travox.Systems;

namespace Travox.Sentinel.Engine
{
    public class Controller : IDisposable
    {
        private Stopwatch Timing;
        private TimeSpan Intervel, Timeout;
        private DateTime CurrentDate;

        public Stopwatch Log;
        public HandlerState State;

        public ResourceManager TravoxResources = Travox.Sentinel.Properties.Resources.ResourceManager;
        public String DBName = "travoxmos";
        public Boolean OnceTime { get; set; }
        public Boolean SetEnabled { get; set; }
        public Boolean IsStoped { get; set; }
        public Boolean IsStarted { get; set; }
        public Boolean IsUpdate
        {
            get
            {
                Boolean GetUpdated = false;
                if (this.GetIntervel > 0) GetUpdated = Timing.ElapsedMilliseconds / this.GetIntervel > 1.0;
                if (this.GetTimeout > 0)
                {
                    GetUpdated = DateTime.Now >= CurrentDate;
                    if (GetUpdated) CurrentDate = CurrentDate.AddDays(1);    
                }
                return GetUpdated;
            }
        }
        protected TimeSpan SetIntervel { set { Timeout = new TimeSpan(0, 0, 0); Intervel = value; } }
        protected TimeSpan SetTimeout { set { Intervel = new TimeSpan(0, 0, 0); Timeout = value; } }
        public Double GetIntervel { get { return (Double)this.Intervel.TotalMilliseconds; } }
        public Double GetTimeout { get { return (Double)this.Timeout.TotalMilliseconds; } }

        public Controller()
        {
            SetEnabled = true;
            IsStoped =  true;
            OnceTime = false;
            Log = new Stopwatch();
            Timing = new Stopwatch();
            State = new HandlerState();
            Timeout = new TimeSpan(0, 0, 0);
            Intervel = new TimeSpan(1, 0, 0);
        }

        ~Controller()
        {
            Timing = null;
            State = null;
            Log = null;
            TravoxResources = null;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public virtual void Start()
        {
            Console.WriteLine("Start Controller({1}) [{0}]", State.DatabaseName, this.GetType().Name.ToString());
            if (this.GetTimeout > 0)
            {
                IsStoped = false;
                CurrentDate = DateTime.Now.Date.AddTicks(Timeout.Ticks);
            }
            Log.Start();
            Timing.Start();
            // TODO Load Content 
        }

        public virtual void Update()
        {
            Console.WriteLine("Update Controller({1}) [{0}]", State.DatabaseName, this.GetType().Name.ToString());
            IsStoped = false;
            Timing.Restart();
            Timing.Start();
            // TODO Update Function with timer 
        }

        public virtual void Stop()
        {
            Console.WriteLine("[{0}] Stop Controller({1}) [{0}]", State.DatabaseName, this.GetType().Name.ToString());
            Log.Stop();
            Timing.Stop();
            if (!Travox.Sentinel.App.CrawlerRunning) IsStoped = true;
            // TODO Clear Data
        }

        protected String GetResource(String filename)
        {
            filename = String.Format("{0}/Crawler/{1}/{2}", new String[] { Module.BaseDirectory, this.GetType().Name.ToString(), filename });
            if (File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    filename = "";
                    Byte[] Buffer = new Byte[fs.Length];
                    Int32 BytesTransferred = (Int32)fs.Length;
                    Int32 BytesIndex = 0;
                    while (BytesTransferred > 0)
                    {
                        Int32 n = fs.Read(Buffer, BytesIndex, BytesTransferred);
                        filename += Encoding.UTF8.GetString(Buffer, BytesIndex, BytesTransferred);

                        if (n == 0) break;
                        BytesIndex += n;
                        BytesTransferred -= n;
                    }
                    BytesTransferred = Buffer.Length;
                }
            }
            else
            {
                filename = "";
            }
            return filename;
        }
    }
}
