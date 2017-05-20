using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Travox.Sentinel.Crawler;
using Travox.Sentinel.Engine;
using Travox.Sentinel.Setting;
using Travox.Systems;

namespace Travox.Sentinel
{
    public partial class App
    {
        public static Boolean CrawlerRunning { get; set; }
        public static Boolean DebugMode { get { return Regex.Match(Process.GetCurrentProcess().ProcessName, ".vshost$").Success ; } }
        public static Boolean ServerConnected { get; set; }
        public static Boolean WebCrawlerConnected { get; set; }
        public static Boolean WebCrawlerRestarted { get; set; }
        public static Version PublishVersion { get; set; }
        public static StringBuilder Log { get; set; }

        public static TcpListener Listen;

        public DialogSetting WindowConfig;
        public Configuration Config;

        DialogInitialize WindowInitialize;
        DialogContext WindowContext;

        System.Windows.Forms.NotifyIcon NotifySentinal;
        ResourceManager TravoxResources = Travox.Sentinel.Properties.Resources.ResourceManager;
        Stopwatch TimeUp;
        ConsoleWriter logWrite;
        FileSystemWatcher logWatch;

        String CrawlerDatabase = "";
        Int32 DBTotal = 1, CtrlTotal = 0;
        Int32 Idx = 0;
        Boolean isOnline = false;
        //Task TaskListen;
        IPAddress TravoxIP;
        UInt16 TravoxPort;

        public enum StatePanelProgress { Load, Update, Close }
        public enum StateTravox { InitReadConfig, InitStartServer, InitDatabase, LoadDatabase, InitSuccess, InitShutdown, OnStatus }

        private List<Controller> CrawlerTravoxDBInitialize()
        {
            List<Controller> control = new List<Controller>();
            //control.Add(new ExchangeRate());
            //control.Add(new MidBackStored());

            //control.Add(new Secretary());
            //control.Add(new AutoBooking());
            //control.Add(new SyncGD());
            return control;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            App.PublishVersion = GetPublishedVersion();
            if (Directory.Exists(Module.TravoxSentinel + "Exception"))
            {
                Array.ForEach(Directory.GetFiles(Module.TravoxSentinel + "Exception"), (string path) => { File.Delete(path); });
            }
            
            if (!Directory.Exists(Module.TravoxSentinel)) Directory.CreateDirectory(Module.TravoxSentinel);
            if (!Directory.Exists(Module.TravoxTemp)) Directory.CreateDirectory(Module.TravoxTemp);
            
            Mutex objMutex = new Mutex(false, Process.GetCurrentProcess().ProcessName);

            if (e.Args.Length > 0)
            {

            }
            else if (objMutex.WaitOne(0, false) == false)
            {
                MessageBox.Show("The Travox Sentinel is still running, Please try again in a moment.", "Travox Sentinel " + PublishVersion, MessageBoxButton.OK);
                Application.Current.Shutdown();
            }
            else
            {
                String LogCurrent = Module.TravoxSentinel + "log/" + DateTime.Now.ToString("yyyy-MM-dd") + Module.File_Log;
                if (File.Exists(LogCurrent)) File.Delete(LogCurrent);

                Log = new StringBuilder();
                TimeUp = new Stopwatch();
                BackgroundWorker InitWorker = new BackgroundWorker();

                WindowContext = new DialogContext();
                WindowInitialize = new DialogInitialize();
                NotifySentinal = new System.Windows.Forms.NotifyIcon();
                NotifySentinal.Visible = true;
                NotifySentinal.Text = "Travox Sentinel";
                NotifySentinal.Click += new EventHandler(Notify_OnClick);
                this.NotifyIcon(FindResource("NotifyOnStop"));

                InitWorker.DoWork += WorkSentinelServices;
                InitWorker.DoWork += WorkCrawlerCollection;
                InitWorker.ProgressChanged += WorkProgress;
                InitWorker.RunWorkerCompleted += WorkCompleted;
                InitWorker.WorkerSupportsCancellation = true;
                InitWorker.WorkerReportsProgress = true;

                App.ServerConnected = false;
                App.WebCrawlerConnected = false;
                App.WebCrawlerRestarted = true;
                App.CrawlerRunning = true;
                InitWorker.RunWorkerAsync();
                TimeUp.Start();
            }
        }
        private void Task_HandlerControl(object sender)
        {
            Controller StateItem = (Controller)sender;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                if (StateItem.IsUpdate || StateItem.IsStoped)
                {
                    if (!StateItem.IsStarted) StateItem.Start(); else StateItem.Update();
                }
            }
            catch (Exception e)
            {
                Module.WriteException(e);
            }
        }

        private void WorkProgress(object sender, ProgressChangedEventArgs e)
        {
            switch ((StateTravox)e.UserState)
            {
                case StateTravox.InitReadConfig: //-10: // InitReadConfig 
                    WindowInitialize.lblProgressTitle.Content = "Initialize Configuration...";
                    WindowInitialize.pgbProgress.IsIndeterminate = true;
                    break;
                case StateTravox.InitStartServer: // -11: // InitStartServer 
                    WindowInitialize.lblProgressTitle.Content = "Starting Server and Crawler...";
                    WindowInitialize.pgbProgress.IsIndeterminate = true;
                    break;
                case StateTravox.InitDatabase: // -1: // InitDatabase 
                    WindowInitialize.lblProgressTitle.Content = "Travox customer database...";
                    WindowInitialize.pgbProgress.IsIndeterminate = false;
                    WindowInitialize.StateInitProgress(e.ProgressPercentage, DBTotal, CrawlerDatabase);
                    break;
                case StateTravox.LoadDatabase: // -5: // LoadDatabase 
                    WindowInitialize.lblProgressTitle.Content = "Initialize Travox customer database...";
                    WindowInitialize.StateInitProgress(e.ProgressPercentage, DBTotal, CrawlerDatabase);
                    break;
                case StateTravox.InitSuccess: // - 2: // InitSuccess 
                    isOnline = true;
                    WindowInitialize.StateInitSuccess();
                    this.NotifyIcon(FindResource("NotifyOnStart"));
                    break;
                case StateTravox.InitShutdown: // - 3: // InitShutdown 
                    isOnline = false;
                    this.NotifyIcon(FindResource("NotifyOnStop"));
                    WindowInitialize.StateShutdown();
                    WindowInitialize.StateInitProgress(e.ProgressPercentage, DBTotal, CrawlerDatabase);
                    break;
                case StateTravox.OnStatus: // - 4: // OnStatus
                    DateTime newDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(TimeUp.ElapsedMilliseconds);

                    WindowContext.lblTimeUp.Content = "";
                    if (newDate.DayOfYear - 1 > 0) WindowContext.lblTimeUp.Content = String.Format("{0} Days ", newDate.DayOfYear - 1);
                    WindowContext.lblTimeUp.Content += String.Format("{0}", String.Format("{0:HH:mm:ss.fff}", newDate));
                    WindowContext.lblThread.Content = Process.GetCurrentProcess().Threads.Count;
                    WindowContext.lblClient.Content = "NaN";
                    WindowContext.lblDatabase.Content = DBTotal;
                    WindowContext.lblCrawler.Content = Idx;
                    WindowContext.lblIPAddress.Content = String.Format("{0}:{1}", TravoxIP.ToString(), TravoxPort);

                    if (true)
                    {
                        WindowContext.btnServerSentinel.IsEnabled = true;
                        WindowContext.btnServerSentinel.Content = (App.ServerConnected) ? "Online" : "Offline";
                        WindowContext.btnServerSentinel.Foreground = (SolidColorBrush)((App.ServerConnected) ? FindResource("TextOnline") : FindResource("TextOffline"));
                        WindowContext.btnServerSentinel.Background = (ImageBrush)((App.ServerConnected) ? FindResource("ClientStart") : FindResource("ClientStop"));
                    }

                    if (App.WebCrawlerRestarted)
                    {
                        WindowContext.btnWebServer.IsEnabled = true;
                        WindowContext.btnWebServer.Content = (App.WebCrawlerConnected) ? "Online" : "Offline";
                        WindowContext.btnWebServer.Foreground = (SolidColorBrush)((App.WebCrawlerConnected) ? FindResource("TextOnline") : FindResource("TextOffline"));
                        WindowContext.btnWebServer.Background = (ImageBrush)(App.WebCrawlerConnected ? FindResource("ClientStart") : FindResource("ClientStop"));
                    }

                    break;
            }
        }
        private void WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (App.ServerConnected)
            {
                Listen.Server.Disconnect(false);
                Listen.Server.Close();
                Listen.Stop();
            }

            if (!(e.Error == null))
            {

            }
            else
            {
                this.WriteLineConsoleDispose();
                TimeUp.Stop();
                WindowInitialize.Hide();
                NotifySentinal.Visible = false;
                Application.Current.Shutdown();
            }
            
        }
        private void WorkCrawlerCollection(object sender, DoWorkEventArgs e)
        {
            Int32 ThreadProgress = 0;

            logWrite = null;
            logWatch = null;
            BackgroundWorker init = sender as BackgroundWorker;
            DB database;
            DataTable db_customer = new DataTable();

            try
            {
                // if(!App.DebugMode) this.LogRecheck();
                // Sentinel Crawler
                init.ReportProgress(0, StateTravox.InitDatabase);
                database = new DB("travox_global");
                String db = "SELECT id, code, database_name, [description] FROM site_customer ";
                db += "WHERE[status] = 'ACTIVE' AND ISNULL(database_name,'') <> ''";

                db_customer = database.GetTable(db);
                DBTotal = db_customer.Rows.Count;
                // Initinalize Database Travox            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Travox Sentinel " + App.PublishVersion.ToString(), MessageBoxButton.OK, MessageBoxImage.Exclamation);
                e.Cancel = true;
                return;
            }

            HandlerItems[] TaskAgrs = new HandlerItems[DBTotal];

            init.ReportProgress(0, StateTravox.LoadDatabase);
            CtrlTotal = this.CrawlerTravoxDBInitialize().Count;
            for (Int32 db = 0; db < DBTotal; db++)
            {
                TaskAgrs[db] = new HandlerItems();
                TaskAgrs[db].Items = new List<HandlerCrawler>();

                Thread.Sleep(10);
                init.ReportProgress(ThreadProgress++, StateTravox.LoadDatabase);
                CrawlerDatabase = "Initialize " + db_customer.Rows[db]["description"].ToString();

                foreach (Controller item in this.CrawlerTravoxDBInitialize())
                {
                    if (!item.OnceTime || db_customer.Rows[db]["database_name"].ToString() == item.DBName)
                    {
                        HandlerCrawler CreatedCrawler = new HandlerCrawler();
                        CreatedCrawler.Crawler = item;
                        CreatedCrawler.Crawler.State = new HandlerState();
                        CreatedCrawler.Crawler.State.ThreadId = Idx;
                        CreatedCrawler.Crawler.State.DatabaseName = db_customer.Rows[db]["database_name"].ToString();
                        CreatedCrawler.Crawler.State.CompanyName = db_customer.Rows[db]["description"].ToString();
                        CreatedCrawler.Crawler.State.CompanyCode = db_customer.Rows[db]["code"].ToString();
                        CreatedCrawler.Crawler.State.CompanyID = db_customer.Rows[db]["id"].ToString();
                        CreatedCrawler.Task = Task.Factory.StartNew((object state) => { }, null);
                        TaskAgrs[db].Items.Add(CreatedCrawler);

                        this.Task_HandlerControl((object)CreatedCrawler.Crawler);
                        Idx++;
                    }
                }
            }

            init.ReportProgress(ThreadProgress++, StateTravox.LoadDatabase);
            Thread.Sleep(100); //100
            init.ReportProgress(0, StateTravox.InitSuccess);

            db_customer.Clear();
            db_customer = null;
            do
            {
                for (Int32 db = 0; db < DBTotal; db++)
                {
                    if (!CrawlerRunning) break;
                    for (Int32 i = 0; i < TaskAgrs[db].Items.Count; i++)
                    {
                        if (!CrawlerRunning || !TaskAgrs[db].Items[i].Crawler.SetEnabled) break;
                        switch (TaskAgrs[db].Items[i].Task.Status)
                        {
                            case TaskStatus.Canceled:
                            case TaskStatus.Faulted:
                            case TaskStatus.RanToCompletion:
                                TaskAgrs[db].Items[i].Task.Dispose();
                                TaskAgrs[db].Items[i].Crawler.IsStarted = true;
                                TaskAgrs[db].Items[i].Task = Task.Factory.StartNew(Task_HandlerControl, TaskAgrs[db].Items[i].Crawler);
                                break;
                        }
                    }
                }
                this.WriteLineConsoleCheck();

                // Thread Sleep Manual.
                init.ReportProgress(0, StateTravox.OnStatus);
                Stopwatch SleepTime = new Stopwatch();
                TimeSpan _TimeInterval = new TimeSpan(0, 0, 3);
                SleepTime.Start();
                do { Thread.Sleep(128); } while (CrawlerRunning && SleepTime.ElapsedMilliseconds < _TimeInterval.TotalMilliseconds);
                SleepTime.Stop();
                // Thread Sleep Manual.
            } while (CrawlerRunning);

            App.ServerConnected = false;
            App.WebCrawlerConnected = false;
            CrawlerDatabase = null;
            init.ReportProgress(0, StateTravox.InitShutdown);

            Int32 DBStoped = 0;
            Int32 DBStopedMax = 0;
            do
            {
                DBStoped = 0;
                for (Int32 db = 0; db < DBTotal; db++)
                {
                    Int32 CrawlerTotal = TaskAgrs[db].Items.Count, CrawlerStoping = 0;
                    for (Int32 i = 0; i < TaskAgrs[db].Items.Count; i++)
                    {
                        switch (TaskAgrs[db].Items[i].Task.Status)
                        {
                            case TaskStatus.Canceled:
                            case TaskStatus.Faulted:
                            case TaskStatus.RanToCompletion:
                                CrawlerStoping++; 
                                if (!TaskAgrs[db].Items[i].Crawler.IsStoped) TaskAgrs[db].Items[i].Crawler.Stop();
                                break;
                        }
                    }
                    if (CrawlerStoping == CrawlerTotal) DBStoped++;
                    if (DBStopedMax < DBStoped) DBStopedMax = DBStoped;
                    Thread.Sleep(10);
                    init.ReportProgress(DBStopedMax, StateTravox.InitShutdown);
                }
            } while (DBStoped != DBTotal);
        }
        private void WorkSentinelServices(object sender, DoWorkEventArgs e)
        {
            // this.WriteLineConsoleCheck();
            BackgroundWorker init = sender as BackgroundWorker;
            
            init.ReportProgress(0, StateTravox.InitReadConfig);
            Config = new Configuration();

            if (!Config.Load()) Config.Default();

            String IP = null; //XHR.Connect("checkip.dyndns.com");
            if (MBOS.Null(IP)) IP = "IP Address: 127.0.0.1";

            Configuration.InternetIP = IPAddress.Parse(Regex.Match(IP, @"IP Address:.*?(?<ip>[\d|\.]+)").Groups["ip"].Value);
            if (App.DebugMode) TravoxIP = Configuration.NetworkIP; else TravoxIP = Configuration.InternetIP;

            TravoxPort = Config.SentinelPort;
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

            TcpListener.Create(TravoxPort).Stop();
            foreach (TcpConnectionInformation tcpi in ipGlobalProperties.GetActiveTcpConnections())
            {
                if (tcpi.LocalEndPoint.Port == TravoxPort) throw new Exception("Port is Unavailable.");
            }
            Config.Save();

            init.ReportProgress(0, StateTravox.InitStartServer);

            // Crawler Sentinel for Client Connected 
            //Console.WriteLine("{0} Server Starting...", TravoxIP.ToString());

            //Listen = new TcpListener(new IPEndPoint(TravoxIP, TravoxPort));
            //Listen.Start();
            //TaskListen = new Task()
            //App.ServerConnected = true;
            //while (App.ServerConnected)
            //{
            //    if (!Listen.Pending()) continue; 

            //    TcpClient client = Listen.AcceptTcpClient();
            //    Thread clientThread = new Thread(new ParameterizedThreadStart(OnClientConnect));
            //    clientThread.Start(client.GetStream());
            //    client.Close();
            //}
        }

        private void OnClientConnect(object asyn)
        {
            String a = (String)asyn;
        }

        private Version GetPublishedVersion()
        {
            return new Version(1, 4, 0, 5);
        }

        private void Notify_OnClick(object sender, EventArgs e)
        {
            if(isOnline)
            {
                if (WindowContext.IsShow)
                {
                    WindowContext.ContextHide();
                }
                else
                {
                    WindowContext.ContextShow();
                    WindowContext.Activate();
                }
            }

        }
        private void NotifyIcon(Object image)
        {
            Uri NotifyOnStop = new System.Uri(((System.Windows.Controls.Image)image).Source.ToString());
            NotifySentinal.Icon = System.Drawing.Icon.FromHandle(new System.Drawing.Bitmap(System.Windows.Application.GetResourceStream(NotifyOnStop).Stream).GetHicon());
        }

        
        private static void Task_OutputCMD(object sender, DataReceivedEventArgs e)
        {
            App.WebCrawlerRestarted = true;
            if (!MBOS.Null(e.Data))
            {
                Match Node = Regex.Match(e.Data, @"^CRAWLER >> \[(?<command>.*?)\]$");
                if (Node.Success)
                {
                    switch (Node.Groups["command"].Value.ToUpper())
                    {
                        case "START": App.WebCrawlerConnected = true; break;
                        default: break;
                    }
                }
                else
                {

                }
            }
        }
        private static void Task_ErrorCMD(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
            App.WebCrawlerConnected = false;
            App.WebCrawlerRestarted = true;
        }
        void InitNodeJS_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker init = sender as BackgroundWorker;
            init.ReportProgress(0, StatePanelProgress.Load);
            // NodeWeb.Stop();
            // NodeWeb.Start(Config.CrawlerScript, Task_OutputCMD, Task_ErrorCMD);
            do { Thread.Sleep(100); } while (!App.WebCrawlerRestarted);
            init.ReportProgress(0, StatePanelProgress.Close);
            App.WebCrawlerRestarted = false;
        }
        void InitNodeJS_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch((StatePanelProgress)e.UserState)
            {
                case StatePanelProgress.Load:
                    App.WebCrawlerRestarted = false;
                    //WaitNodeJS = new PanelProgress();
                    //WaitNodeJS.Show();
                    break;
                case StatePanelProgress.Update:

                    break;
                case StatePanelProgress.Close:
                    //WaitNodeJS.Close();
                    break;
            }
        }

        void WriteLineConsoleDispose()
        {
            if(logWrite != null)
            {
                logWrite.Flush();
                logWrite.Close();
                logWrite.Dispose();
            }

            if (logWatch != null)
            {
                logWatch.EnableRaisingEvents = false;
                logWatch.Changed -= new FileSystemEventHandler(OnLogChanged);
                logWatch.Dispose();
            }
        }
        void WriteLineConsoleCheck()
        {
            String OnDate = DateTime.Now.ToString("yyyy-MM-dd");
            String LogFilename = Module.TravoxSentinel + "log\\" + OnDate + Module.File_Log;
            String ErrorFilename = Module.TravoxSentinel + "error" + Module.File_Log;

            if (!File.Exists(LogFilename))
            {
                this.WriteLineConsoleDispose();
                if (!Directory.Exists(Path.GetDirectoryName(LogFilename))) Directory.CreateDirectory(Path.GetDirectoryName(LogFilename));

                //using (logWrite = new ConsoleWriter(LogFilename, FileMode.Create))
                //{
                //    logWrite.WriteEvent += txtLogMessage_WriteEvent;
                //    logWrite.WriteLineEvent += txtLogMessage_WriteLineEvent;

                //    Console.SetOut(logWrite);
                //}

            }

            if (!File.Exists(ErrorFilename))
            {
                StreamWriter errorWrite = new StreamWriter(new FileStream(ErrorFilename, FileMode.Create));
                errorWrite.AutoFlush = true;
                Console.SetError(errorWrite);
            }
        }
        public static void txtLogMessage_WriteLineEvent(object sender, ConsoleWriterEventArgs e)
        {
            Log.AppendLine(e.Value);
        }
        public static void txtLogMessage_WriteEvent(object sender, ConsoleWriterEventArgs e)
        {
            Log.Append(e.Value);
        }
        private static void OnLogChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine(File.ReadAllText(e.FullPath));
        }

    }

    public class HandlerItems
    {
        public List<HandlerCrawler> Items;
    }

    public class HandlerCrawler
    {
        public Task Task;
        public Controller Crawler;
        public Boolean IsStarted = false;
        public override string ToString()
        {
            StringBuilder arg = new StringBuilder();
            arg.AppendFormat("Task ID: {0} ", Task.Id);
            arg.AppendFormat("{0}", Task.Status);
            return arg.ToString();
        }
    }
    public class HandlerState
    {
        public String DatabaseName;
        public String CompanyName;
        public String CompanyCode;
        public String CompanyID;
        
        public Int32 ThreadId;
        public override string ToString()
        {
            StringBuilder arg = new StringBuilder();
            arg.AppendFormat("Thread:{1} In Database ({0})", new String[] { DatabaseName, CompanyName });
            return arg.ToString();
        }
    }
    public class ManualTask
    {
        public Boolean IsSuccess = false;
        public void Set() { IsSuccess = true; }
    }

}