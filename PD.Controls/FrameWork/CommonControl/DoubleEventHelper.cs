///开发人员:李超 
///
///时间:2010-12-23
///
///功能描述:为元素提供双击支持(silverlight 5中提供双击事件支持 后续可以不是使用此类) 
///
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

namespace PD.Controls
{
    /// <summary>
    /// 为元素提供双击支持(silverlight 5中提供双击事件支持) 
    /// </summary>
    public class DoubleEventHelper
    {
        /*上一次按下事件的时间 为双击事件触发提供一个有效地时间差*/
        DateTime previousTime=default(DateTime);

        /*两次点击的时间间隔*/
        int timeDifference = 300;

        public event EventHandler DoubleEvent;
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="element"></param>
        public void RegisterEvent(UIElement element)
        {
            if (element != null)
            {

                element.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(ElementMouseDown), true);
            }
        }
        /// <summary>
        /// 注销事件
        /// </summary>
        /// <param name="element"></param>
        public void RemoveEvent(UIElement element)
        {
            if (element != null)
            {
                element.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(ElementMouseDown));
            }
        }

        void ElementMouseDown(object sender, MouseButtonEventArgs e)
        {
            var now = DateTime.Now;

            if (previousTime == default(DateTime))
            {
                previousTime = now;
            }

            if (now != previousTime)
            {
                var timeSpan = (now - previousTime).TotalMilliseconds;

                if (timeSpan <= timeDifference)
                {

                    if (DoubleEvent != null)
                    {
                        DoubleEvent(sender, e);
                    }

                    previousTime = default(DateTime);

                }
                else
                {
                    previousTime = now;
                }
            }
        }
    }
}
