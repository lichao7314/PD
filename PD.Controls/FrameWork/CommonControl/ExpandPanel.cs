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
using System.Windows.Interactivity;

namespace PD.Controls
{
    public class ExpandPanel : ContentControl
    {
        public event MouseButtonEventHandler ExpandPanelMouseDown;
        public event MouseButtonEventHandler ContractPanelMouseDown;

        public ExpandPanel()
        {
            this.DefaultStyleKey = typeof(ExpandPanel);
        }
        /// <summary>
        /// 头部说明
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        public object HeaderContent
        {
            get { return (object)GetValue(HeaderContentProperty); }
            set
            {
                SetValue(HeaderContentProperty, value);
            }
        }
        private static readonly DependencyProperty HeaderContentProperty =
         DependencyProperty.Register(
         "HeaderContent",
         typeof(object),
         typeof(ExpandPanel), null);



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TemplateControl<Image>("imageExpandPanel").MouseLeftButtonDown += ExpandPanel_MouseLeftButtonDown;//展开
            TemplateControl<Image>("imageCollPanel").MouseLeftButtonDown += Contract_MouseLeftButtonDown;//收缩
        }

        /// <summary>
        /// 展开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ExpandPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ExpandPanelMouseDown != null)
                ExpandPanelMouseDown(sender, e);
        }
        /// <summary>
        /// 收缩事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Contract_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ContractPanelMouseDown != null)
                ContractPanelMouseDown(sender, e);
        }

        ControlType TemplateControl<ControlType>(string name) where ControlType : FrameworkElement
        {
            return (ControlType)base.GetTemplateChild(name);
        }

        private static readonly DependencyProperty HeaderProperty =
          DependencyProperty.Register(
          "Header",
          typeof(string),
          typeof(ExpandPanel), null);
    }
}
