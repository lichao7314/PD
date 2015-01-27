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
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace PD.ServiceClient
{
    /// <summary>
    ///   通用配置信息集合
    /// </summary>
    public class GeneralConfigs
    {
        #region Constructor

        #region Delegates

        [ComVisible(true)]
        public delegate void InitializeCompletedEventHandler();

        #endregion

        private static WebClient _client;

        /// <summary>
        ///   Occurs when [initialize completed].
        /// </summary>
        public static event InitializeCompletedEventHandler InitializeCompleted;

        #endregion

        #region Get config file from web

        /// <summary>
        /// </summary>
        private static string _cacheContent = "";

        /// <summary>
        ///   Initializes this instance.
        /// </summary>
        /// <param name = "configFileName">Name of the config file.</param>
        /// <exception cref = "ArgumentNullException"></exception>
        public static void Initialize(string configFileName)
        {
            if (configFileName == null) throw new ArgumentNullException("configFileName");
            _client = new WebClient();

            if (Application.Current.Host.Source == null)
            {
                throw new ArgumentNullException("Application.Current.Host.Source");
            }
            var url = Application.Current.Host.Source.ToString();
            url = url.Substring(0, url.IndexOf('/', 7) + 1);
            url += Application.Current.Host.Source.AbsolutePath.Substring(0, Application.Current.Host.Source.AbsolutePath.IndexOf('/', 1));
            var configUri =
                new Uri(
                    url + "/Resource/Config/" + configFileName + "?n=" +
                    Guid.NewGuid().ToString("N"),
                    UriKind.Absolute);
            if (Application.Current.IsRunningOutOfBrowser)
            {
                if (Application.Current.RootVisual == null)
                {
                    Application.Current.RootVisual = new UserControl();
                }
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
        }

        public static string SeachConfigValue(string key)
        {
            var filterConfigs = GeneralConfigs.GetConfigList().Where(item => item[0].Equals(key));
            if (filterConfigs.Count() > 0)
                return filterConfigs.ElementAt(0)[1];
            return string.Empty;
        }

        /// <summary>
        ///   Gets the navigation config list.
        /// </summary>
        /// <returns></returns>
        public static List<List<string>> GetConfigList()
        {
            var content = _cacheContent;
            var result = content.Split('\r', '\n');
            var qry = (from l in result
                       where l.Trim() != "" && l.Trim() != "\r\n" && l.Trim().Substring(0, 1) != "#"
                       select l).ToArray();

            var resultList = new List<List<string>>();

            foreach (var line in qry)
            {
                var l = line.Trim().Replace("\t\t", "\t").Replace("\t", " ").Replace("  ", " ");
                while (l.IndexOf("  ") != -1)
                    l = l.Replace("  ", " ");
                var col = l.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var list = new List<string>();
                list.AddRange(col);
                resultList.Add(list);
            }
            return resultList;
        }

        /// <summary>
        ///   Clients the download string completed.
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.Net.DownloadStringCompletedEventArgs" /> instance containing the event data.</param>
        private static void ClientDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //应用程序级别处理该消息  MessageBox.Show("下载文件 NavigationConfiguration.txt 失败。" + e.Error.Message, "错误", MessageBoxButton.OK);
            }
            else
            {
                try
                {
                    _cacheContent = e.Result;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("分析文件 NavigationConfiguration.txt 失败。" + ex.Message, "错误", MessageBoxButton.OK);
                }
            }

            _client = null;

            if (InitializeCompleted != null)
                InitializeCompleted();
        }

        #endregion
    }
}
