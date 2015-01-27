using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace PD.Controls.Media
{
    public partial class PlayTools : UserControl
    {
        #region 成员
        private MediaElementState currentState = MediaElementState.Stopped;

        private int totalPlayTime = 0;
        private int currentPlayTime = 0;
        private bool isScreenFull = false;

        private Slider volumeSlider = null;
        #endregion

        #region 属性
        public bool IsScreenFull
        {
            get
            {
                return this.isScreenFull;
            }
            set
            {
                this.isScreenFull = value;
                this.btnFullScreen.IsChecked = value;
                if (value)
                {
                    this.btnList.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.btnList.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// 当前播放状态
        /// </summary>
        public MediaElementState CurrentState
        {
            get
            {
                return this.currentState;
            }
            set
            {
                this.currentState = value;
                this.txtBStatus.Text = value.ToString();
                if (value == MediaElementState.Stopped)
                {
                    this.btnPlayPuse.IsChecked = false;
                }
                else if (value == MediaElementState.Playing)
                {
                    this.btnPlayPuse.IsChecked = true;
                }
            }
        }

        /// <summary>
        /// 当前播放时间进度
        /// </summary>
        public int CurrentPlayTime
        {
            get
            {
                return this.currentPlayTime;
            }
            set
            {
                this.currentPlayTime = value;
                this.txtBCurrentTime.Text = FormatTime(value);
                if (this.totalPlayTime != 0)
                {
                    this.progressBar.Value = (100.0 * value / this.totalPlayTime) + 1.5;
                }
            }
        }

        /// <summary>
        /// 获取或设置总播放时间
        /// </summary>
        public int TotaPlayTime
        {
            get
            {
                return this.totalPlayTime;
            }
            set
            {
                this.totalPlayTime = value;
                this.txtBTotalTime.Text = FormatTime(value);
            }
        }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string CurrentMessage
        {
            get
            {
                return this.txtBStatus.Text;
            }
            set
            {
                this.txtBStatus.Text = value;
            }
        }

        /// <summary>
        /// 当前播放媒体名称
        /// </summary>
        public string CurrentPlayingMediaName
        {
            get
            {
                return this.txtBDisplayName.Text;
            }
            set
            {
                this.txtBDisplayName.Text = "【" + value + "】";
            }
        }
        #endregion

        #region 委托事件
        /// <summary>
        /// 点击播放、暂停、停止按钮事件
        /// </summary>
        public event Action<MediaElementState> ClickPlayPauseStopButton;

        /// <summary>
        /// 点击全屏按钮事件
        /// </summary>
        public event Action<bool> ClickFullScreenButton;

        /// <summary>
        /// 改变音量事件
        /// </summary>
        public event Action<double> ChangeVolumeValue;

        /// <summary>
        /// 点解列表按钮
        /// </summary>
        public event Action ClickListButton;

        public event Action<TimeSpan> ClickProcessBar;
        #endregion

        #region 构造函数
        public PlayTools()
        {
            // Required to initialize variables
            InitializeComponent();

            this.btnPlayPuse.Click += new RoutedEventHandler(btnPlayPuse_Click);
            this.btnStop.Click += new RoutedEventHandler(btnStop_Click);
            this.btnFullScreen.Click += new RoutedEventHandler(btnFullScreen_Click);
            this.progressBar.MouseLeftButtonUp += new MouseButtonEventHandler(progressBar_MouseLeftButtonUp);
            this.btnList.Click += new RoutedEventHandler(btnList_Click);
        }
        #endregion

        #region 事件处理
        void btnList_Click(object sender, RoutedEventArgs e)
        {
            if (this.ClickListButton != null)
            {
                this.ClickListButton();
            }
        }

        void progressBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ClickProcessBar != null)
            {
                double xOffset = e.GetPosition(this.rootLayout).X - 2;
                this.ClickProcessBar(ChangeProcessValue(xOffset));

                this.ChangeProcessValue(xOffset);
            }
        }

        void btnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (this.ClickFullScreenButton != null)
            {
                if ((bool)(sender as ToggleButton).IsChecked)
                {
                    this.IsScreenFull = true;
                }
                else
                {
                    this.IsScreenFull = false;
                }
                this.ClickFullScreenButton(this.IsScreenFull);
            }
        }

        void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (this.ClickPlayPauseStopButton != null)
            {
                this.currentState = MediaElementState.Stopped;
                this.ClickPlayPauseStopButton(this.currentState);
                this.btnPlayPuse.IsChecked = false;
            }
        }

        void btnPlayPuse_Click(object sender, RoutedEventArgs e)
        {
            if (this.ClickPlayPauseStopButton != null)
            {
                if ((bool)(sender as ToggleButton).IsChecked)
                {
                    this.currentState = MediaElementState.Playing;
                }
                else
                {
                    this.currentState = MediaElementState.Paused;
                }
                this.ClickPlayPauseStopButton(this.currentState);
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.ChangeVolumeValue != null)
            {
                this.ChangeVolumeValue(e.NewValue / 100.0);
            }
        }

        private void Slider_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.volumeSlider == null)
            {
                this.volumeSlider = sender as Slider;
                this.volumeSlider.MouseWheel += new MouseWheelEventHandler(volumeSlider_MouseWheel);
            }
        }

        void volumeSlider_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.volumeSlider != null)
            {
                if (e.Delta > 0)
                {
                    if (this.volumeSlider.Value < 100)
                    {
                        this.volumeSlider.Value = this.volumeSlider.Value + 1;
                    }
                }
                else
                {
                    if (this.volumeSlider.Value > 0)
                    {
                        this.volumeSlider.Value = this.volumeSlider.Value - 1;
                    }
                }
            }
        }
        #endregion

        #region 私有方法
        private string FormatTime(int Seconds)
        {
            return string.Format("{0}:{1}:{2}", (Seconds / 3600).ToString("00"), ((Seconds % 3600) / 60).ToString("00"), ((Seconds % 3600) % 60).ToString("00"));
        }

        private TimeSpan ChangeProcessValue(double xOffset)
        {
            TimeSpan mediaPosition = new TimeSpan();
            if (this.currentState == MediaElementState.Playing || this.currentState == MediaElementState.Paused)
            {
                double activeWidth = this.progressBar.ActualWidth;
                double position = (100 * xOffset) / activeWidth;
                double setTimeSecond = (position * this.totalPlayTime) / 100;
                mediaPosition = new TimeSpan((int)(setTimeSecond / 3600), (int)((setTimeSecond % 3600) / 60), (int)((setTimeSecond % 3600) % 60));
                this.progressBar.Value = position;
            }

            return mediaPosition;
        }
        #endregion
    }
}