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
using System.Collections.Generic;
using System.Linq;

namespace PD.ServiceClient
{
    #region Wcf Configuration class.

    /// <summary>
    /// Wcf配置信息集合
    /// </summary>
    public class WcfConfigs : List<WcfConfig>
    {
        #region Constructor

        private static volatile WcfConfigs _instance;
        private static readonly object SyncObj = new object();
        private WebClient _client;

        /// <summary>
        /// 
        /// </summary>
        private bool _downloadStringSucceed;

        /// <summary>
        /// Prevents a default instance of the <see cref="WcfConfigs"/> class from being created.
        /// </summary>
        private WcfConfigs()
        {
        }

        /// <summary>
        /// 下载Wcf配置是否成功
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [download string succeed]; otherwise, <c>false</c>.
        /// </value>
        public bool DownloadStringSucceed
        {
            get { return _downloadStringSucceed; }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static WcfConfigs Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncObj)
                    {
                        if (_instance == null)
                            _instance = new WcfConfigs();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Occurs when [initialize completed].
        /// </summary>
        public event EventHandler InitializeCompleted;

        #endregion

        #region Get config file from web

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            _client = new WebClient();

            if (Application.Current.Host.Source == null)
            {
                throw new ArgumentNullException("Application.Current.Host.Source");
            }
            var url = Application.Current.Host.Source.ToString();
            url = url.Substring(0, url.IndexOf('/', 7) + 1);

            //var configUri =
            //    new Uri(url + "Infotech.MES.Framework.Web/Resource/Config/WcfConfiguration.txt?n=" + Guid.NewGuid().ToString("N"),
            //            UriKind.Absolute);
            #region remark by add //  configUri变量没有取相对Web地址
            url = Application.Current.Host.Source.ToString().Substring(0, Application.Current.Host.Source.ToString().LastIndexOf("/ClientBin"));
            var configUri =
              new Uri(url + "/Resource/Config/WcfConfiguration.txt?n=" + Guid.NewGuid().ToString("N"),
                      UriKind.Absolute);
            #endregion
            if (Application.Current.IsRunningOutOfBrowser)
            {
                Application.Current.RootVisual.Dispatcher.BeginInvoke(() =>
                {
                    _client.DownloadStringCompleted +=
                        ClientDownloadStringCompleted;
                    _client.DownloadStringAsync(configUri);
                });
            }
            else
            {
                _client.DownloadStringCompleted += ClientDownloadStringCompleted;
                _client.DownloadStringAsync(configUri);
            }

            //Application.Current.RootVisual = new UserControl();
            //Application.Current.RootVisual.Dispatcher.BeginInvoke(() =>
            //                                                          {

            //});
        }

        /// <summary>
        /// Clients the download string completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Net.DownloadStringCompletedEventArgs"/> instance containing the event data.</param>
        private void ClientDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _downloadStringSucceed = false;
                //应用程序级别处理该消息  MessageBox.Show("下载文件 WcfConfiguration.txt 失败。" + e.Error.Message, "错误", MessageBoxButton.OK);
            }
            else
            {
                try
                {
                    var result = e.Result.Split('\r', '\n');
                    var qry = (from l in result
                               where l.Trim() != "" && l.Trim() != "\r\n" && l.Trim().Substring(0, 1) != "#"
                               select l).ToArray();
                    foreach (var line in qry)
                    {
                        var l = line.Replace("\t\t", "\t").Replace("\t", " ").Replace("  ", " ");
                        while (l.IndexOf("  ") != -1)
                            l = l.Replace("  ", " ");
                        var col = l.Split(' ');
                        Add(col[0].Trim(), col[1].Trim(), col[2].Trim());
                    }
                    _downloadStringSucceed = true;
                }
                catch (Exception ex)
                {
                    _downloadStringSucceed = false;
                    MessageBox.Show("分析文件 WcfConfiguration.txt 失败。" + ex.Message, "错误", MessageBoxButton.OK);
                }
            }

            _client = null;

            if (InitializeCompleted != null)
            {
                InitializeCompleted(this, null);
            }
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="description">The description.</param>
        private void Add(string key, string uri, string description)
        {
            Add(new WcfConfig { Key = key, Uri = uri, Description = description });
        }

        #endregion

        /// <summary>
        /// 获取指定的Wcf配置
        /// </summary>
        /// <param name="key">某个Wcf配置的Key值</param>
        /// <returns>
        /// 返回Wcf配置
        /// </returns>
        public WcfConfig GetConfig(string key)
        {
            foreach (var c in this.Where(c => c.Key.Equals(key)))
                return c;

            return new WcfConfig { Uri = "" };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WcfConfig
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>
        /// The URI.
        /// </value>
        public string Uri { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }
    }

    #endregion
}
