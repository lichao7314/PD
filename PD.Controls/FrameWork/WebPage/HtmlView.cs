using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Infragistics.Controls.Interactions;

namespace Infotech.MES.Controls.Common
{
    public class HtmlView
    {
        /// <summary>
        /// 创建Html视图
        /// </summary>
        /// <param name="url">绝对URL</param>
        /// <returns></returns>
        public static UIElement CreateHtmlView(string url)
        {
            UIElement view = null;
            //var errorPage = HostAddress.Host() + "NavigateError.htm";
            if (Application.Current.IsRunningOutOfBrowser)
            {
                view = new WebBrowser() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                try
                {
                    /*下面这样写的原因是因为WebBrowser中URI不支持中文参数 ，所以先将URL编码*/
                    var taskUrl = new Uri(url, UriKind.Absolute);
                    (view as WebBrowser).Navigate(new Uri(taskUrl.AbsoluteUri, UriKind.Absolute));
                }
                catch
                {
                    view = new TextBlock { Text = "对不起,系统无法找到您需要导航的页面", Foreground = new SolidColorBrush() { Color = Colors.Red } };
                }
            }
            else
            {
                view = new XamHtmlViewer() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                try
                {
                    (view as XamHtmlViewer).SourceUri = new Uri(url, UriKind.Absolute);
                }
                catch
                {
                    view = new TextBlock { Text = "对不起,系统无法找到您需要导航的页面", Foreground = new SolidColorBrush() { Color = Colors.Black }, FontSize = 13 };
                }
            }
            return view;
        }

        public static UIElement CreateHtmlView()
        {
            UIElement view = null;
            if (Application.Current.IsRunningOutOfBrowser)
            {
                view = new WebBrowser() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            }
            else
            {
                view = new XamHtmlViewer() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            }
            return view;
        }

        public static void Navigate(UIElement view, string url)
        {
            if (view is WebBrowser)
            {
                var taskUrl = new Uri(url, UriKind.Absolute);
                (view as WebBrowser).Navigate(new Uri(taskUrl.AbsoluteUri, UriKind.Absolute));
            }
            else
            {
                (view as XamHtmlViewer).SourceUri = new Uri(url, UriKind.Absolute);
            }
        }
    }
}
