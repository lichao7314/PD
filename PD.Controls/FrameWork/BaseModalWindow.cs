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
    public class BaseModalWindow : BaseModulePage, IModule
    {

        public BaseModalWindow() {
            ShowClose = true;
        }
        private ModalWindow _ModalWindow = new ModalWindow();

        public ModalWindow ModalWindow
        {
            get { return _ModalWindow; }
        }

        public bool ShowMinButton { get; set; }

        public bool ShowMaxButton { get; set; }

        public bool ShowClose { get; set; }

        private bool IsAutoDispose { get; set; }

        public double OverlayOpacity { get; set; }

        public bool Resize { get; set; }

        public Image Icon { get; set; }

        public event EventHandler CloseWindow;

        /// <summary>
        /// 显示弹出窗体
        /// </summary>
        /// <param name="_Width">弹出窗体的宽度</param>
        /// <param name="_Height">弹出窗体的高度</param>
        /// <param name="_Title">弹出窗体的标题</param>
        /// <param name="_IsShowMinButton">是否显示最小化按钮</param>
        /// <param name="_IsShowMaxButton">是否显示最大化按钮</param>
        /// <param name="_IsAutoDispose">是否需要自动调用Dispose方法</param>
        public void Show(double _Width, double _Height, string _Title, bool _IsShowMinButton, bool _IsShowMaxButton, bool _IsAutoDispose)
        {
            _ModalWindow.Closed += new EventHandler(_ModalWindow_Closed);
            _ModalWindow.Resize = Resize;
            if (Icon != null)
            {
                _ModalWindow.Icon = Icon;
            }
            _ModalWindow.OverlayOpacity = OverlayOpacity <= 0 ? 0.3 : OverlayOpacity;
            _ModalWindow.Width = _Width;
            _ModalWindow.Height = _Height;
            _ModalWindow.Title = _Title;
            _ModalWindow.ShowMinButton = _IsShowMinButton;
            _ModalWindow.HasCloseButton = ShowClose;
            _ModalWindow.ShowMaxButton = _IsShowMaxButton;
            _ModalWindow.Content = this;
            IsAutoDispose = _IsAutoDispose;
            _ModalWindow.Show();
        }

        public void Show(string _Title, bool _IsShowMinButton, bool _IsShowMaxButton, bool _IsAutoDispose)
        {
            _ModalWindow.Closed += (_ModalWindow_Closed);
            _ModalWindow.Resize = Resize;
            if (Icon != null)
            {
                _ModalWindow.Icon = Icon;
            }
            _ModalWindow.OverlayOpacity = OverlayOpacity <= 0 ? 0.3 : OverlayOpacity;
            _ModalWindow.Title = _Title;
            _ModalWindow.ShowMinButton = _IsShowMinButton;
            _ModalWindow.ShowMaxButton = _IsShowMaxButton;
            _ModalWindow.HasCloseButton = ShowClose;
            _ModalWindow.Content = this;
            IsAutoDispose = _IsAutoDispose;
            _ModalWindow.Show();
        }

        public void Show(string _Title, bool isAutoDispose)
        {
            _ModalWindow.Closed += (_ModalWindow_Closed);
            _ModalWindow.Resize = Resize;
            if (Icon != null)
            {
                _ModalWindow.Icon = Icon;
            }
            _ModalWindow.OverlayOpacity = OverlayOpacity <= 0 ? 0.3 : OverlayOpacity;
            _ModalWindow.Title = _Title;
            _ModalWindow.ShowMinButton = ShowMinButton;
            _ModalWindow.ShowMaxButton = ShowMaxButton;
            _ModalWindow.HasCloseButton = ShowClose;
            _ModalWindow.Content = this;
            IsAutoDispose = isAutoDispose;
            _ModalWindow.Show();
        }
        /// <summary>
        /// 关闭弹出窗体
        /// </summary>
        public void Close()
        {
            _ModalWindow.Close();
        }

        void _ModalWindow_Closed(object sender, EventArgs e)
        {
            _ModalWindow.Closed -= new EventHandler(_ModalWindow_Closed);
            if (IsAutoDispose)
            {
                Dispose();
            }
            if (CloseWindow != null)
            {
                CloseWindow(this, e);
            }
        }

        /// <summary>
        /// 消息对话框
        /// </summary>
        /// <param name="content"></param>
        protected void ShowMessage(string content)
        {
            MessageBox.Show(content, "系统提示", MessageBoxButton.OK);
        }

        #region IModule 成员

        public virtual void RegisterEvent()
        {

        }

        public virtual void UnRegisterEvent()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion
    }
}