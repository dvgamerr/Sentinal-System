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
        OpenFileDialog Dialog;
        Configuration Config;

        public DialogSetting()
        {
            Config = new Configuration();
            Dialog = new OpenFileDialog();
            InitializeComponent();
        }

        private void ChangeConnectionNodeJS()
        {
            try
            {
                String JsonString = Module.ReadText(Config.CrawlerConfig);
                Config.NodeJS = JSON.Deserialize<NodeJSArgs>(JsonString);
                txtCrawlerConfig.Text = Path.GetFileName(Config.CrawlerConfig);
                GridNodeJS.Visibility = System.Windows.Visibility.Visible;
            }
            catch
            {
                MessageBox.Show("Config is not match.");
            }

            txtNodeIP.Text = Config.NodeJS.IPAddress;
            txtNodePort.Text = Config.NodeJS.Port;
            txtSQLServerName.Text = Config.NodeJS.Database.ServerName;
            txtSQLDatabaseName.Text = Config.NodeJS.Database.Name;
            txtSQLUsername.Text = Config.NodeJS.Database.Username;
            txtSQLPassword.Text = Config.NodeJS.Database.Password;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GridNodeJS.Visibility = System.Windows.Visibility.Hidden;
            txtSentinelIP.Text = (App.DebugMode) ? Config.NetworkIP.ToString() : Config.InternetIP.ToString();
            txtSentinelPort.Text = Config.SentinelPort.ToString();

            txtCrawlerScript.Text = "None";
            if (File.Exists(Config.CrawlerScript)) txtCrawlerScript.Text = Path.GetFileName(Config.CrawlerScript);

            txtCrawlerConfig.Text = "None";
            if (File.Exists(Config.CrawlerConfig)) ChangeConnectionNodeJS();
        }

        private void btnBrowseScript_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Dialog.DefaultExt = ".js";
            Dialog.Filter = "Javascript file (*.js)|*.js";
            Dialog.InitialDirectory = @"C:\";
            Dialog.Title = "Please select NodeJS Javascript file.";

            if (File.Exists(Config.CrawlerScript))
            {
                Dialog.InitialDirectory = Path.GetDirectoryName(Config.CrawlerScript);
                Dialog.FileName = Path.GetFileName(Config.CrawlerScript);
            }

            if ((Boolean)Dialog.ShowDialog())
            {
                txtCrawlerScript.Text = Path.GetFileName(Dialog.FileName);
                Config.CrawlerScript = Dialog.FileName;
            }
        }

        private void btnBrowseConfig_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Dialog.DefaultExt = ".json";
            Dialog.Filter = "Config JSON (*.json)|*.json";
            Dialog.InitialDirectory = @"C:\";
            Dialog.Title = "Please select an config.json file for NodeJS.";

            if (File.Exists(Config.CrawlerConfig))
            {
                Dialog.InitialDirectory = Path.GetDirectoryName(Config.CrawlerConfig);
                Dialog.FileName = Path.GetFileName(Config.CrawlerConfig);
            }

            if ((Boolean)Dialog.ShowDialog())
            {
                txtCrawlerConfig.Text = Path.GetFileName(Dialog.FileName);
                Config.CrawlerConfig = Dialog.FileName;
                ChangeConnectionNodeJS();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.IsShow = false;
            Config.SentinelPort = ushort.Parse(txtSentinelPort.Text);
            Config.NodeJS.IPAddress = txtNodeIP.Text;
            Config.NodeJS.Port = txtNodePort.Text;
            Config.NodeJS.Database.ServerName = txtSQLServerName.Text;
            Config.NodeJS.Database.Name = txtSQLDatabaseName.Text;
            Config.NodeJS.Database.Username = txtSQLUsername.Text;
            Config.NodeJS.Database.Password = txtSQLPassword.Text;

            Config.Save();
            if (File.Exists(Config.CrawlerConfig)) Module.Write(Config.CrawlerConfig, JSON.Serialize<NodeJSArgs>(Config.NodeJS));

        }

    }
}
