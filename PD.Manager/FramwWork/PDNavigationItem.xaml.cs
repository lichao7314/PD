using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using PD.ServiceClient.PDService;

namespace PD.Manager
{
    public partial class PDNavigationItem : UserControl, IDisposable
    {
        public event EventHandler<MenuNavigationEventArgs> NavigatonClick;

        public PDNavigationItem()
        {
            InitializeComponent();
        }

        public void InitNavigationItem(T_BASE_MENU item)
        {
            image.Source = new BitmapImage(new Uri(item.MenuImagePath, UriKind.Relative));
            this.DataContext = item;
            image.MouseLeftButtonUp += (image_MouseLeftButtonUp);
            tbMenuName.Text = item.MenuName;
        }

        void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (NavigatonClick != null)
            {
                NavigatonClick(this, new MenuNavigationEventArgs { Menu = this.DataContext as T_BASE_MENU });
            }
        }

        public void Dispose()
        {
            image.MouseLeftButtonUp -= (image_MouseLeftButtonUp);
        }
    }

    public class MenuNavigationEventArgs : EventArgs
    {
        public T_BASE_MENU Menu { get; set; }
    }
}
