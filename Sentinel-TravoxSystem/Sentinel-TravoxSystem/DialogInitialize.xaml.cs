using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Travox.Sentinel
{

    public partial class DialogInitialize : Window
    {
        private Boolean AboutMode = false;
        public Boolean IsShow = false;
        public Boolean ShutdownEvent = false;

        public DialogInitialize(Boolean about = false)
        {
            AboutMode = about;
            InitializeComponent();
            Version app = App.PublishVersion;
            if (!AboutMode)
            {
                btnClose.Visibility = Visibility.Hidden;
                Topmost = (App.DebugMode) ? false : true;
                lblProgressTitle.Content = "Starting Travox Sentinal...";
                lblProgressDesc.Content = "";
                lblProgressPercent.Content = "";
                pgbProgress.IsIndeterminate = true;
                Show();
            } 
            else
            {
                this.Topmost = true;
                this.Cursor = Cursors.Arrow;
                PanelProgress.Visibility = Visibility.Hidden;
            }
            lblVersion.Content = string.Format("{0}.{1:00} builds {2:00}{3:00}", app.Major, app.Minor, app.Build, app.Revision);
        }

        public void StateInitSuccess()
        {
            lblProgressTitle.Content = "Initialize Travox Sentinal...";
            lblProgressDesc.Content = "";
            pgbProgress.IsIndeterminate = true;
            lblProgressPercent.Content = "";
            Hide();
        }
        public void StateInitProgress(Int32 percent, Int32 total = 0,String db_desc = null)
        {
            if (db_desc != null) lblProgressDesc.Content = db_desc;
            if (total != 0) pgbProgress.Maximum = total;
            pgbProgress.Value = percent;
            if (total != 0)
            {
                lblProgressPercent.Content = ((Int32)(percent * 100 / total)).ToString() + '%';
            }
            else
            {
                lblProgressPercent.Content = "";
            }
        }

        public void StateShutdown()
        {
            Show();
            WindowMain.Cursor = System.Windows.Input.Cursors.Wait;
            lblProgressTitle.Content = "Shutdown Sentinal...";
            lblProgressDesc.Content = "";
            pgbProgress.IsIndeterminate = false;
            lblProgressPercent.Content = "";
        }


        public void InitializeVisible()
        {
            this.IsShow = true;
            WindowMain.Cursor = System.Windows.Input.Cursors.Arrow;
            Show();
        }
        public void InitializeHidden()
        {
            this.IsShow = false;
            Hide();
            WindowMain.Cursor = System.Windows.Input.Cursors.Wait;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
