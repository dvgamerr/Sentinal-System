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

            Rect area = SystemParameters.WorkArea;
            Point position = new Point(area.Width - this.Width - 5, area.Bottom - this.Height - 5);

            //this.PanelStatus.Visibility = Visibility.Hidden;
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

        SolidColorBrush BorderAndText = new SolidColorBrush(Color.FromArgb(255, 203, 203, 203));
        SolidColorBrush BorderActive = new SolidColorBrush(Color.FromArgb(255, 112, 112, 112));
        SolidColorBrush TextActive = new SolidColorBrush(Color.FromArgb(255, 48, 48, 48));

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

        private void btnLog_Click(object sender, RoutedEventArgs e)
        {
            btnClose.Visibility = Visibility.Hidden;
            btnOption.Visibility = Visibility.Hidden;
            btnBack.Visibility = Visibility.Visible;
            PanelLog.Visibility = Visibility.Visible;
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogConfirm closeDialog = new DialogConfirm("Travox Sentinel", "You want to turn off the system?", "Not, now.", "Shutdown.");

            closeDialog.HandlerButton1 = (object s2, RoutedEventArgs e2) => {
                closeDialog.Hide();
            };

            closeDialog.HandlerButton2 = (object s2, RoutedEventArgs e2) => {
                App.CrawlerRunning = false;
                closeDialog.Hide();
                ContextHide();
            };

            closeDialog.ShowDialog();
        }

        private void btnOption_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            btnClose.Visibility = Visibility.Visible;
            btnOption.Visibility = Visibility.Visible;
            btnBack.Visibility = Visibility.Hidden;
            PanelLog.Visibility = Visibility.Hidden ;

        }
    }
}
