using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Data;

namespace PD.Controls.Media
{
    public partial class Player : UserControl
    { 
        #region 成员
        private IList<PlayListItem> playList = null;
        private Popup FullPopup = null;

        private bool isListBoxShow = false;
        private List<DateTime> mouseClickList = null;
        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置一个值，是否单文件循环播放
        /// </summary>
        public bool IsReplay { get; set; }

        /// <summary>
        /// 获取或设置一个值，是否自动播放
        /// </summary>
        public bool IsAutoPaly
        {
            get
            {
                return this.mediaElement.AutoPlay;
            }
            set
            {
                this.mediaElement.AutoPlay = value;
            }
        }

        /// <summary>
        /// 播放列表
        /// </summary>
        public IList<PlayListItem> PlayList
        {
            get
            {
                return this.playList;
            }
            set
            {
                this.playList = value;
                this.playListBox.PlayListSource = value;
            }
        }
        #endregion

        #region 构造函数
        public Player()
        {
            // Required to initialize variables
            InitializeComponent();
            mouseClickList = new List<DateTime>();

            this.mediaElement.MediaOpened += new RoutedEventHandler(mediaElement_MediaOpened);
            this.mediaElement.MediaEnded += new RoutedEventHandler(mediaElement_MediaEnded);
            this.mediaElement.CurrentStateChanged += new RoutedEventHandler(mediaElement_CurrentStateChanged);
            this.mediaElement.BufferingProgressChanged += new RoutedEventHandler(mediaElement_BufferingProgressChanged);
            this.mediaElement.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(mediaElement_MediaFailed);
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);

            this.playTools.ClickFullScreenButton += new Action<bool>(playTools_ClickFullScreenButton);
            this.playTools.ClickPlayPauseStopButton += new Action<MediaElementState>(playTools_ClickPlayPauseStopButton);
            this.playTools.ClickProcessBar += new Action<TimeSpan>(playTools_ClickProcessBar);
            this.playTools.ChangeVolumeValue += new Action<double>(playTools_ChangeVolumeValue);
            this.playTools.ClickListButton += new Action(playTools_ClickListButton);
            this.playListBox.SelectPlayItemChanged += new Action<PlayListItem>(playListBox_SelectPlayItemChanged);

            Application.Current.Host.Content.FullScreenChanged += new EventHandler(Content_FullScreenChanged);

            this.mediaElement.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(mediaElement_MouseLeftButtonDown);
        }
        #endregion

        #region 事件处理
        void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            this.playTools.CurrentMessage = e.ErrorException.Message;
        }

        void mediaElement_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            if (this.mediaElement.CurrentState == MediaElementState.Buffering)
            {
                this.playTools.CurrentMessage = this.mediaElement.CurrentState + " " + Math.Round(this.mediaElement.BufferingProgress * 100, 0).ToString() + "%";
            }
        }

        void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            this.playTools.CurrentState = this.mediaElement.CurrentState;
        }

        void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.mediaElement.Stop();
            if (this.IsReplay)
            {
                this.mediaElement.Play();
            }
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            int currentTime = (int)this.mediaElement.Position.TotalSeconds;
            this.playTools.CurrentPlayTime = currentTime;
        }

        void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            this.playTools.TotaPlayTime = (int)this.mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
        }

        void playTools_ChangeVolumeValue(double value)
        {
            this.mediaElement.Volume = value;
        }

        void playTools_ClickPlayPauseStopButton(MediaElementState state)
        {
            switch (state)
            {
                case MediaElementState.Playing:
                    this.mediaElement.Play();
                    break;
                case MediaElementState.Paused:
                    this.mediaElement.Pause();
                    break;
                case MediaElementState.Stopped:
                    this.mediaElement.Stop();
                    break;
            }
        }

        void playTools_ClickProcessBar(TimeSpan time)
        {
            this.mediaElement.Position = time;
        }

        void playTools_ClickFullScreenButton(bool isScreenFull)
        {
            Application.Current.Host.Content.IsFullScreen = isScreenFull;
            if (isScreenFull)
            {
                this.FullPopup = new Popup();

                double width = Application.Current.Host.Content.ActualWidth;
                double height = Application.Current.Host.Content.ActualHeight;
                VideoFullPlayer videoFullPlayer = new VideoFullPlayer();
                videoFullPlayer.FullScreenChange += new Action<bool>(videoFullPlayer_FullScreenChange);
                videoFullPlayer.Width = width;
                videoFullPlayer.Height = height;
                videoFullPlayer.SetVideoBrush(this.mediaElement);

                PlayTools playTools = this.playTools;
                this.LayoutRoot.Children.Remove(this.playTools);
                videoFullPlayer.CurrentPlayTools = playTools;
                FullPopup.Child = videoFullPlayer;
                FullPopup.IsOpen = true;
            }
        }

        void videoFullPlayer_FullScreenChange(bool obj)
        {
            this.playTools.IsScreenFull = obj;
            Application.Current.Host.Content.IsFullScreen = obj;
        }

        void Content_FullScreenChanged(object sender, EventArgs e)
        {
            if (!Application.Current.Host.Content.IsFullScreen)
            {
                if (this.FullPopup != null)
                {
                    PlayTools playTools = (this.FullPopup.Child as VideoFullPlayer).CurrentPlayTools;
                    playTools.IsScreenFull = false;
                    this.LayoutRoot.Children.Add(playTools);
                    Grid.SetRow(playTools, 1);
                    this.FullPopup.IsOpen = false;
                    this.FullPopup = null;
                }
            }
        }

        void playTools_ClickListButton()
        {
            if (this.isListBoxShow)
            {
                this.isListBoxShow = false;
                this.playListBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.isListBoxShow = true;
                this.playListBox.Visibility = Visibility.Visible;
            }
        }

        void playListBox_SelectPlayItemChanged(PlayListItem listItem)
        {
            this.PlayListItem(listItem);
        }

        void mediaElement_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (mouseClickList.Count == 0)
            {
                mouseClickList.Add(DateTime.Now);
            }
            else
            {
                if (mouseClickList.Count == 1)
                {
                    if (DateTime.Now.Subtract(mouseClickList[0]).TotalMilliseconds <= 300)
                    {
                        this.playTools.IsScreenFull = true;
                        playTools_ClickFullScreenButton(true);
                    }
                    mouseClickList.Clear();
                }
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 顺序播放列表文件
        /// </summary>
        public void Play()
        {
            if (this.playList != null && this.playList.Count > 0)
            {
                this.mediaElement.Source = this.playList[0].MediaSource;
                this.playTools.CurrentPlayingMediaName = this.playList[0].DisplayName;
            }
        }

        /// <summary>
        /// 播放指定文件
        /// </summary>
        /// <param name="playListItem"></param>
        public void Play(PlayListItem playListItem)
        {
            if (playListItem != null)
            {
                this.playList.Add(playListItem);
                this.mediaElement.Source = playListItem.MediaSource;
                this.playTools.CurrentPlayingMediaName = playListItem.DisplayName;
            }
        }
        #endregion

        #region 私有方法
        private void PlayListItem(PlayListItem playListItem)
        {
            this.mediaElement.Source = playListItem.MediaSource;
            this.mediaElement.Play();
            this.playTools.CurrentPlayingMediaName = playListItem.DisplayName;
        }
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }

    public class SubStringConvert : IValueConverter
    {
        #region IValueConverter 成员

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string strValue = value.ToString();
                if (strValue.Length > 6)
                {
                    strValue = strValue.Substring(0, 6) + "…";
                }

                return strValue;
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}