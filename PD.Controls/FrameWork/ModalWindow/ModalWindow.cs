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
using System.ComponentModel;
using System.Linq;
using System.Windows.Automation;
namespace PD.Controls
{
    /// <summary>
    /// 对话框辅助
    /// </summary>
    public class ModalWindow : IDisposable
    {
        protected CustomChildWindow childWindow = null;

        public event EventHandler Closed;

        public event EventHandler<CancelEventArgs> Closing;

        public event RoutedEventHandler Loaded;

        /// <summary>
        /// 对话框结果
        /// </summary>
        public bool? DialogResult
        {
            get
            {
                return childWindow.DialogResult;
            }
            set
            {
                childWindow.DialogResult = value;
            }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public object Title
        {
            get
            {
                return childWindow.Title;
            }
            set
            {
                childWindow.Title = value;
            }
        }
        /// <summary>
        /// 是否能移动
        /// </summary>
        public bool Move
        {
            get { return childWindow.Move; }
            set
            {
                 
                childWindow.Move = value;
            }
        }

        /// <summary>
        /// 是否模态窗体
        /// </summary>
        public bool IsModal
        {
            get
            {
                return childWindow.Modal;
            }
            set
            {
                childWindow.Modal = value;
            }
        }

        /// <summary>
        /// 页面内容
        /// </summary>
        public object Content
        {
            get
            {
                return childWindow.Content;
            }
            set
            {
                childWindow.Content = value;
            }
        }

        /// <summary>
        /// 数据上下文
        /// </summary>
        public object DataContext
        {
            get
            {
                return childWindow.DataContext;
            }
            set
            {
                childWindow.DataContext = value;
            }
        }
        /// <summary>
        /// 最小化是距离底部的间距
        /// </summary>
        public double MinVertical
        {
            get
            {
                return childWindow.MinVertical;
            }
            set
            {
                childWindow.MinVertical = value;
            }
        }

        /// <summary>
        /// 是否显示关闭按钮
        /// </summary>
        public bool HasCloseButton
        {
            get
            {

                return childWindow.HasCloseButton;
            }
            set
            {
                childWindow.HasCloseButton = value;
            }
        }

        /// <summary>
        /// 模态窗体宽度
        /// </summary>
        public double Width
        {
            get
            {
                return childWindow.Width;
            }
            set
            {
                childWindow.Width = value;
            }
        }
        /// <summary>
        /// 图标
        /// </summary>
        public Image Icon
        {
            get
            {
                return childWindow.Icon;
            }
            set
            {
                childWindow.Icon = value;
            }
        }

        /// <summary>
        /// 模态窗体高度
        /// </summary>
        public double Height
        {
            get
            {
                return childWindow.Height;
            }
            set
            {
                childWindow.Height = value;
            }
        }
        /// <summary>
        /// 是否显示最大化按钮
        /// </summary>
        public bool ShowMaxButton
        {
            get
            {
                return childWindow.ShowMaxButton;
            }
            set
            {
                childWindow.ShowMaxButton = value;
            }
        }
        /// <summary>
        /// 是否显示最小化按钮
        /// </summary>
        public bool ShowMinButton
        {
            get
            {
                return childWindow.ShowMinButton;
            }
            set
            {
                childWindow.ShowMinButton = value;
            }
        }
        /// <summary>
        /// 移动大小
        /// </summary>
        public bool Resize
        {
            get
            {
                return childWindow.Resize;
            }
            set
            {
                childWindow.Resize = value;
            }
        }

        public WindowInteractionState WindowState
        {
            get
            {
                return childWindow.InteractionState;
            }
        }

        /// <summary>
        /// 模态窗口下背景透明度
        /// </summary>
        public double OverlayOpacity
        {
            get
            {
                return childWindow.OverlayOpacity;
            }
            set
            {
                childWindow.OverlayOpacity = value;
            }
        }

        public static string ChildWindowStyleKey = "CustomChildWindowStyle1";

        public ModalWindow()
        {
            childWindow = new CustomChildWindow()
            {
                ShowMaxButton = false,
                ShowMinButton = false,
                Modal = true,
                Resize = false,
                MinVertical = 30,
                OverlayOpacity = 0.65
            };
            childWindow.Style = Application.Current.Resources[ChildWindowStyleKey] as Style;
            RegisterEvent();
            childWindow.Tag = this;
        }

        /// <summary>
        /// 显示模态窗体
        /// </summary>
        public void Show()
        {
            childWindow.Show();
        }

        /// <summary>
        /// 关闭模态窗体
        /// </summary>
        public void Close()
        {
            childWindow.Close();
        }

        public void Show(bool show) 
        {
            childWindow.Popup(show);
        }

        public void Dispose()
        {
            UnRegisterEvent();
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (Closing != null)
            {
                Closing(this, e);
            }
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            if (Closed != null)
            {
                Closed(this, e);
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (Loaded != null)
            {
                Loaded(this, e);
            }
        }

        private void WindowMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void RegisterEvent()
        {
            childWindow.Closed += WindowClosed;
            childWindow.Closing += WindowClosing;
            childWindow.Loaded += WindowLoaded;
            childWindow.MouseRightButtonDown += WindowMouseRightButtonDown;
        }

        private void UnRegisterEvent()
        {
            childWindow.MouseRightButtonDown -= WindowMouseRightButtonDown;
            childWindow.Closed -= WindowClosed;
            childWindow.Closing -= WindowClosing;
            childWindow.Loaded -= WindowLoaded;
        }

        /// <summary>
        /// 获取当前对象的模态窗体对象
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static ModalWindow GetCurrentModalWindow(object content)
        {
            if (content is FrameworkElement)
            {
                DependencyObject element = (content as FrameworkElement).Parent;
                while (element != null)
                {
                    if ((element is CustomChildWindow) &&
                        ((element as CustomChildWindow).Tag is ModalWindow))
                    {
                        return (element as CustomChildWindow).Tag as ModalWindow;
                    }
                }
                return default(ModalWindow);

            }
            else
            {
                return default(ModalWindow);
            }
        }

    }
}
