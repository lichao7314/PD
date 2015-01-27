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
using System.Windows.Data;

namespace PD.Controls.FrameWork.CommonControl
{
    public partial class PopupMessageControl : UserControl, IModule
    {
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer()
        {
            Interval = new TimeSpan(0, 0, 0, 0,30000)
        };

        public PopupMessageControl()
        {
            InitializeComponent();
            RegisterEvent();
            timer.Tick += new EventHandler((sender2, e2) =>
            {
                HiddenMessage();
            });
        }

        public void ShowMessage(UIElement element, int width, int height, Thickness th)
        {
            canvasPopup.Margin = th;
            this.Width =width;
            this.Height = height;
            (storyPopUp.Children[0] as DoubleAnimation).To = -this.Height;
            (storyPopDown.Children[0] as DoubleAnimation).From = -this.Height;
  
            if (contentPanel.Child is IDisposable)
            {
                (contentPanel.Child as IDisposable).Dispose();
            }
            (Application.Current as IApplicationFramework).HiddenHtmlView();
            if (element != null)
            {
                if ((element as FrameworkElement).Parent != null)
                    MessageBox.Show("当前您这个消息内容已经存在于一个父容器中");
                else
                    contentPanel.Child = element;
                (element as FrameworkElement).HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                (element as FrameworkElement).VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            }
            if (storyPopDown.GetCurrentState() != ClockState.Stopped)
                storyPopDown.Stop();
            storyPopUp.Begin();
            timer.Start();
        }

        public void ShowTitle(Uri imageUrl, string title)
        {
            imageTitle.Source = new BitmapImage(imageUrl);
            tbTitle.Text = title;
        }

        public void HiddenMessage()
        {
            (Application.Current as IApplicationFramework).ShowHtmlView();
            timer.Stop();
            if (storyPopUp.GetCurrentState() != ClockState.Stopped)
                storyPopUp.Stop();
            storyPopDown.Begin();
        }

        void CloseClick(object sender, MouseButtonEventArgs e)
        {
            HiddenMessage();
            (Application.Current as  IApplicationMessage).ExcuteClosePopup();
        }

        public void RegisterEvent()
        {
            btnClose.MouseLeftButtonUp += (CloseClick);
        }

        public void UnRegisterEvent()
        {
            btnClose.MouseLeftButtonUp -= (CloseClick);
            timer.Stop();
        }

        public void Dispose()
        {
            UnRegisterEvent();
        }
    }

     
}
