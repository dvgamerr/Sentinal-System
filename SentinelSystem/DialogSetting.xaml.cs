using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Travox.Sentinel.Setting;
using Travox.Systems;

namespace Travox.Sentinel
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class DialogSetting : Window
    {
        public Boolean IsShow = false;
        //OpenFileDialog Dialog;
        Configuration Config;

        public DialogSetting()
        {
            InitializeComponent();

            Config = new Configuration();
            if (!Config.Load()) Config.Default();

            this.WindowStartupLocation = WindowStartupLocation.Manual;

            Rect area = SystemParameters.WorkArea;
            Point position = new Point((area.Width - this.Width) / 2, (area.Height - this.Height) / 2);

            if (Config.PanelSetting.X != 0 || Config.PanelSetting.Y != 0) position = Config.PanelSetting;
            this.Left = position.X;
            this.Top = position.Y;
            this.Margin = new Thickness(position.X, position.Y, 0, 0);
            //Dialog = new OpenFileDialog();
        }

        private void ChangeConfig()
        {
            txtSentinelIP.Text = (App.DebugMode) ? Configuration.NetworkIP.ToString() : Configuration.InternetIP.ToString();
            txtSentinelPort.Text = Config.SentinelPort.ToString();

            txtNodeIP.Text = Config.API.IPAddress;
            txtNodePort.Text = Config.API.Port;
            txtSQLServerName.Text = Config.MSSQL.ServerName;
            txtSQLDatabaseName.Text = Config.MSSQL.Name;
            txtSQLUsername.Text = Config.MSSQL.Username;
            txtSQLPassword.Text = Config.MSSQL.Password;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtCrawlerScript.Text = "None";
            //if (File.Exists(Config.CrawlerScript)) txtCrawlerScript.Text = Path.GetFileName(Config.CrawlerScript);

            txtCrawlerConfig.Text = "None";
            ChangeConfig();
        }

        private void btnBrowseScript_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Dialog.DefaultExt = ".js";
            //Dialog.Filter = "Javascript file (*.js)|*.js";
            //Dialog.InitialDirectory = @"C:\";
            //Dialog.Title = "Please select NodeJS Javascript file.";

            //if (File.Exists(Config.CrawlerScript))
            //{
            //    Dialog.InitialDirectory = Path.GetDirectoryName(Config.CrawlerScript);
            //    Dialog.FileName = Path.GetFileName(Config.CrawlerScript);
            //}

            //if ((Boolean)Dialog.ShowDialog())
            //{
            //    txtCrawlerScript.Text = Path.GetFileName(Dialog.FileName);
            //    Config.CrawlerScript = Dialog.FileName;
            //}
        }

        private void btnBrowseConfig_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Dialog.DefaultExt = ".json";
            //Dialog.Filter = "Config JSON (*.json)|*.json";
            //Dialog.InitialDirectory = @"C:\";
            //Dialog.Title = "Please select an config.json file for NodeJS.";

            //if (File.Exists(Config.CrawlerConfig))
            //{
            //    Dialog.InitialDirectory = Path.GetDirectoryName(Config.CrawlerConfig);
            //    Dialog.FileName = Path.GetFileName(Config.CrawlerConfig);
            //}

            //if ((Boolean)Dialog.ShowDialog())
            //{
            //    txtCrawlerConfig.Text = Path.GetFileName(Dialog.FileName);
            //    Config.CrawlerConfig = Dialog.FileName;
            //    ChangeConnectionNodeJS();
            //}
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.IsShow = false;
            Config.SentinelPort = ushort.Parse(txtSentinelPort.Text);
            Config.API.IPAddress = txtNodeIP.Text;
            Config.API.Port = txtNodePort.Text;
            Config.MSSQL.ServerName = txtSQLServerName.Text;
            Config.MSSQL.Name = txtSQLDatabaseName.Text;
            Config.MSSQL.Username = txtSQLUsername.Text;
            Config.MSSQL.Password = txtSQLPassword.Text;
            Config.PanelSetting.X = this.Left;
            Config.PanelSetting.Y = this.Top;
            Config.Save();
        }

    }
}
