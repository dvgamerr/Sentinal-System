using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Travox.Sentinel
{
    public partial class DialogConfirm : Window
    {
        public delegate void Handler(object sender, RoutedEventArgs e);
        public Handler HandlerButton1;
        public Handler HandlerButton2;
        public DialogConfirm(String title, String description, String no = "OK", String yes = null)
        {
            InitializeComponent();
            lblTitle.Content = title;
            lblDesc.Content = description;
            btnNo.Content = no;
            btnYes.Content = yes;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            HandlerButton2(sender, e);
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            HandlerButton1(sender, e);
        }
    }
}
