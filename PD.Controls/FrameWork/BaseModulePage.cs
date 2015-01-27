using System;
using System.Collections;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Theming;
using System.Collections.Generic;
using PD.ServiceClient.PDService;
namespace PD.Controls
{
    /// <summary>
    /// 业务功能页面基类
    /// </summary>
    public class BaseModulePage : UserControl
    {
        private Theme theme;

        public T_BASE_MENU Menu { get; set; }

        /// <summary>
        /// 页面等待指示器
        /// </summary>
        public BusyIndicator Busy = new BusyIndicator();

        public string BusyText
        {
            get
            {
                return this.Busy.BusyContent + "";
            }
            set
            {
                this.Busy.BusyContent = value;
            }
        }

       // public Menu Menu { get; set; }

        /// <summary>
        /// 开始绘制等待指示器在整个框架上
        /// </summary>
        protected void BeginLoading()
        {
            var iApp = Application.Current as IApplicationFramework;
            if (iApp != null)
                iApp.BeginLoading();
        }

        /// <summary>
        /// 结束绘制等待指示器在整个框架上
        /// </summary>
        protected void EndLoading()
        {
            var iApp = Application.Current as IApplicationFramework;
            if (iApp != null)
                iApp.EndLoading();
        }

        /// <summary>
        /// 开始绘制等待指示器在当前页面上
        /// </summary>
        /// <param name="element">当前窗体上的一个控件</param>
        public void BeginWindowLoading(DependencyObject element)
        {
            //var iApp = Application.Current as IApplicationFramework;
            //if (iApp != null)
            this.Busy.IsBusy = true;
        }
        /// <summary>
        /// 结束绘制等待指示器在当前页面上
        /// </summary>
        /// <param name="element">当前窗体上的一个控件</param>
        public void EndWindowLoading(DependencyObject element)
        {
            this.Busy.IsBusy = false;
            this.BusyText = "页面处理中...";
            //var iApp = Application.Current as IApplicationFramework;
            //if (iApp != null)
            //    iApp.EndWindowLoading(element);
        }

        /// <summary>
        /// 
        /// </summary>
        public BaseModulePage()
        {
            var skinTheme = SkinHelper.SkinDictionary.ControlDictionary[SkinType.Blue];

            if (System.ComponentModel.DesignerProperties.IsInDesignTool == false)
            {
                skinTheme = SkinHelper.SkinDictionary.ControlDictionary[SkinType.Blue];
            }

            Busy.BusyContent = "页面处理中...";

            Busy.Style = Application.Current.Resources["frameworkLoadModuleStyle"] as Style;

            theme = new Theme()
            {
                ThemeUri = new Uri(skinTheme, UriKind.Relative)
            };

            theme.Content = Busy;

            base.Content = theme;
        }

        /// <summary>
        /// 背景颜色
        /// </summary>
        public new Brush Background
        {
            get
            {
                return base.Background;
            }
            set
            {
                base.Background = value;
                theme.Background = value;
            }
        }

        /// <summary>
        /// 页面内容
        /// </summary>
        public new UIElement Content
        {
            get
            {
                return Busy.Content as UIElement;
            }
            set
            {
                Busy.Content = value;
            }
        }

        /// <summary>
        /// 菜单路径
        /// </summary>
        public string MenuPathName { get; set; }

        /// <summary>
        /// 更新样式
        /// </summary>
        /// <param name="url">样式文件相对路径</param>
        public void UpdateSkin(string url)
        {
            theme.ThemeUri = new Uri(url, UriKind.Relative);
            Busy.Style = Application.Current.Resources["frameworkLoadModuleStyle"] as Style;
        }

        public DateTime ServerTime
        {
            get
            {
                return (Application.Current as IApplicationFramework).ServerTime;
            }
        }
    }
}
