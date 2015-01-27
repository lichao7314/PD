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
using Infotech.MES.Controls;

namespace PD.Controls 
{
    public partial class WebPage : BaseModulePage, IModule
    {
        private UIElement element = null;

        public WebPage()
        {
            InitializeComponent();
            RegisterEvent();
        }

        void WebPage_Loaded(object sender, RoutedEventArgs e)
        {
            LayoutRoot.Children.Add(element);
        }

        /// <summary>
        /// 隐藏控件
        /// </summary>
        public void Hiddle()
        {
            element.Visibility = System.Windows.Visibility.Collapsed;
        }
        /// <summary>
        /// 显示控件
        /// </summary>
        public void Show()
        {
            element.Visibility = System.Windows.Visibility.Visible;
        }
        /// <summary>
        /// 呈现HTMLVIEW
        /// </summary>
        /// <param name="element"></param>
        public void DisplayHtmlView(UIElement element)
        {
            this.element = element;
        }
        public void RegisterEvent()
        {
            this.Loaded +=  WebPage_Loaded;
           // this.SizeChanged += new SizeChangedEventHandler(WebPage_SizeChanged);
        }

        void WebPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (element != null && (element is FrameworkElement))
            //{
            //    double x = Math.Round(Math.Max(0, 0d));
            //    double y = Math.Round(Math.Max(0, -40d));
            //    element.RenderTransform = new TransformGroup();
            //    var transformGroup = element.RenderTransform as TransformGroup;
            //    var translateTransform = transformGroup.Children.OfType<TranslateTransform>().FirstOrDefault();
            //    if (translateTransform == null)
            //    {
            //        transformGroup.Children.Add(new TranslateTransform() { X = x, Y = y });
            //    }
            //    else
            //    {
            //        translateTransform.X = x;
            //        translateTransform.Y = y;
            //    }
                 
            //    element.UpdateLayout();
            //}
        }

        public void UnRegisterEvent()
        {
            this.Loaded -=  WebPage_Loaded;
            this.SizeChanged-=(WebPage_SizeChanged);
        }

        public void Dispose()
        {
            UnRegisterEvent();
        }
    }
}
