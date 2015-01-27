using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PD.Controls.Media
{
    public partial class VideoFullPlayer : UserControl
    { 
        #region 成员
        private DispatcherTimer disTimer = null;
        private List<int> mouseClickList = null;
        #endregion

        #region 属性
        public PlayTools CurrentPlayTools
        {
            get
            {
                PlayTools playTools = this.borPlayTools.Child as PlayTools;
                this.borPlayTools.Child = null;
                return playTools;
            }
            set
            {
                this.borPlayTools.Child = value;
            }
        }
        #endregion

        #region 委托事件
        public event Action<bool> FullScreenChange;
        #endregion

        #region 构造函数
        public VideoFullPlayer()
        {
            InitializeComponent();
            mouseClickList = new List<int>();
            this.disTimer = new DispatcherTimer();
            this.disTimer.Interval = new System.TimeSpan(0, 0, 5);
            this.disTimer.Tick += new System.EventHandler(disTimer_Tick);
            this.disTimer.Start();

            this.borPlayTools.MouseEnter += new MouseEventHandler(borPlayTools_MouseEnter);
            this.borPlayTools.MouseLeave += new MouseEventHandler(borPlayTools_MouseLeave);

            this.LayoutRoot.MouseLeftButtonDown += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonDown);
        }
        #endregion

        #region 事件处理
        void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (mouseClickList.Count == 0)
            {
                mouseClickList.Add(DateTime.Now.Second);
            }
            else
            {
                if (mouseClickList.Count == 1)
                {
                    if (((int)mouseClickList[0]) == DateTime.Now.Second)
                    {
                        if (this.FullScreenChange != null)
                        {
                            this.FullScreenChange(false);
                        }
                    }
                    mouseClickList.Clear();
                }
            }
        }

        void disTimer_Tick(object sender, System.EventArgs e)
        {
            this.borPlayTools.Visibility = Visibility.Collapsed;
        }

        private void rectanglePanel_MouseEnter(object sender, MouseEventArgs e)
        {
            this.disTimer.Stop();
            this.borPlayTools.Visibility = Visibility.Visible;
        }

        private void rectanglePanel_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.disTimer.IsEnabled)
            {
                this.disTimer.Start();
            }
        }

        void borPlayTools_MouseEnter(object sender, MouseEventArgs e)
        {
            this.disTimer.Stop();
        }

        void borPlayTools_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.disTimer.IsEnabled)
            {
                this.disTimer.Start();
            }
        }
        #endregion

        #region 公共方法
        public void SetVideoBrush(MediaElement mediaElement)
        {
            this.videoBrush.SetSource(mediaElement);
        }
        #endregion
    }
}
