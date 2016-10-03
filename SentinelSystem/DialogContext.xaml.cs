using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Travox.Sentinel
{
    /// <summary>
    /// Interaction logic for PanelContext.xaml
    /// </summary>
    public partial class DialogContext : Window
    {
        public Boolean IsShow = false;

        private DialogInitialize about;
        private DialogSetting config;
       
        public DialogContext()
        {
            InitializeComponent();
        }

        public void ContextShow()
        {

            IsShow = true;
            ResetContextButton();

            Rect area = SystemParameters.WorkArea;
            Point position = new Point(area.Width - this.Width - 5, area.Bottom - this.Height - 5);

            this.btnStatus.BorderBrush = BorderActive;
            this.btnStatus.Foreground = TextActive;
            this.PanelStatus.Visibility = Visibility.Hidden;
            this.PanelLog.Visibility = Visibility.Hidden;
            this.PanelSetting.Visibility = Visibility.Hidden;
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = position.X;
            this.Top = position.Y;
            this.Margin = new Thickness(position.X, position.Y, 0, 0);
            this.Show();

        }

        public void ContextHide()
        {
            IsShow = false;
            this.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.ContextHide();
        }
        
        private void btnStatus_Click(object sender, RoutedEventArgs e)
        {
            ResetContextButton();
            PanelStatus.Visibility = Visibility.Visible;
            PanelSetting.Visibility = Visibility.Hidden;
            PanelLog.Visibility = Visibility.Hidden;

            btnStatus.BorderBrush = BorderActive;
            btnStatus.Foreground = TextActive;
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            ResetContextButton();
            PanelStatus.Visibility = Visibility.Hidden;
            PanelSetting.Visibility = Visibility.Visible;
            PanelLog.Visibility = Visibility.Hidden;

            btnSetting.BorderBrush = BorderActive;
            btnSetting.Foreground = TextActive;
        }

        private void btnLog_Click(object sender, RoutedEventArgs e)
        {
            ResetContextButton();
            PanelStatus.Visibility = Visibility.Hidden;
            PanelSetting.Visibility = Visibility.Hidden;
            PanelLog.Visibility = Visibility.Visible;

            btnLog.BorderBrush = BorderActive;
            btnLog.Foreground = TextActive;
        }

        SolidColorBrush BorderAndText = new SolidColorBrush(Color.FromArgb(255, 203, 203, 203));
        SolidColorBrush BorderActive = new SolidColorBrush(Color.FromArgb(255, 112, 112, 112));
        SolidColorBrush TextActive = new SolidColorBrush(Color.FromArgb(255, 48, 48, 48));

        private void ResetContextButton()
        {
            btnStatus.BorderBrush = BorderAndText;
            btnStatus.Foreground = BorderAndText;

            btnSetting.BorderBrush = BorderAndText;
            btnSetting.Foreground = BorderAndText;

            btnLog.BorderBrush = BorderAndText;
            btnLog.Foreground = BorderAndText;
        }

        private void btnShutdown_Click(object sender, RoutedEventArgs e)
        {
            Travox.Sentinel.App.CrawlerRunning = false;
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            if (about == null || !about.IsShow)
            {
                about = new DialogInitialize(true);
                about.IsShow = true; 
                about.Show();
            }
            about.Topmost = true;
            about.Activate();
            about.Topmost = false;
        }

        private void btnServerSentinel_Click(object sender, RoutedEventArgs e)
        {
            btnServerSentinel.IsEnabled = false;
            btnServerSentinel.Content = "Restarting...";
            btnServerSentinel.Background = (ImageBrush)FindResource("ClientWait");
        }

        private void btnWebServer_Click(object sender, RoutedEventArgs e)
        {
            btnWebServer.IsEnabled = false;
            btnWebServer.Content = "Restarting...";
            btnWebServer.Background = (ImageBrush)FindResource("ClientWait");
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            if (config == null || !config.IsShow)
            {
                config = new DialogSetting();
                config.IsShow = true; 
                config.Show();
            }
            config.Topmost = true;
            config.Activate();
            config.Topmost = false;
        }

        DialogConfirm closeDialog = new DialogConfirm("Travox Sentinel", "You want to turn off the system?", "Not, now.", "Shutdown.");

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            closeDialog.HandlerButton1 = btnClose_No;
            closeDialog.HandlerButton2 = btnClose_Yes;
            closeDialog.ShowDialog();
        }
        private void btnClose_Yes(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("click yes");
        }
        private void btnClose_No(object sender, RoutedEventArgs e)
        {
            closeDialog.Hide();

        }
    }
}
