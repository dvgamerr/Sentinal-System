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
        private Boolean onMove = false;
        public delegate void Handler(object sender, RoutedEventArgs e);
        public Handler HandlerButton1 = null;
        public Handler HandlerButton2 = null;
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

        private void MoveBar_MouseMove(object sender, MouseEventArgs e)
        {
            //var relativePosition = e.GetPosition(this);
            //var point = PointToScreen(relativePosition);
            //this..HorizontalOffset = point.X;
            //_x.VerticalOffset = point.Y;
        }

        private void MoveBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            onMove = true;
        }

        private void MoveBar_MouseLeave(object sender, MouseEventArgs e)
        {
            onMove = false;
        }
    }
}
