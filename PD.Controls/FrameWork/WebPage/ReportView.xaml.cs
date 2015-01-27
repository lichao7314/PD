using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using Infotech.MES.Controls.Common;
using PD.ServiceClient;

namespace PD.Controls
{
    public partial class ReportView : UserControl
    {
        private FrameworkElement reportContent = null;

        private FrameworkElement reportExportContent = null;

        private ReportViewParamCollection paramCollection = new ReportViewParamCollection();

        /// <summary>
        ///  视图类型
        /// </summary>
        public ReportViewType ReportViewType { get; set; }

        /// <summary>
        /// 访问绝对地址
        /// </summary>
        public string ReportAbsoluteUrl
        {
            get
            {
                if (string.IsNullOrEmpty(SystemConstantConfig.ReportAddress))
                    throw new NullReferenceException("没有配置Report地址前缀在ConstantConfiguration.txt中");
                if (string.IsNullOrEmpty(ReportPath))
                    throw new NullReferenceException("没有配置Report页面路径在CognosPath属性中");
                return SystemConstantConfig.ReportAddress + ConvertReportPath(ReportPath) + ParamStartDelimiter() + paramCollection.ParamInStr();
            }
        }

        /// <summary>
        /// 页面路径
        /// </summary>
        public string ReportPath { get; set; }

        public ReportView()
        {
            InitializeComponent();

            
            reportContent = HtmlView.CreateHtmlView() as FrameworkElement;

            reportExportContent = HtmlView.CreateHtmlView() as FrameworkElement;

            reportExportContent.Height = 0;

            reportExportContent.Width = 0;

            Grid.SetRow(reportExportContent, 1);

            root.Children.Add(reportExportContent);

            Grid.SetRow(reportContent, 0);

            root.Children.Add(reportContent);
        }

        #region 参数控制
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddParam(string key, string value)
        {
            if (paramCollection.VailUnionKey(key))
            {
                paramCollection.Add(new ReportViewParam
                {
                    Key = key,

                    Value = value
                });
            }
            else
            {
                throw new ArgumentException("参数集合中已经存在为" + key + "的参数,请使用ChangeParamValue方法更改值");
            }
        }

        /// <summary>
        /// 移除参数值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void RemoveParam(string key)
        {
            var currentParam = paramCollection[key];
            if (currentParam != null)
            {
                paramCollection.Remove(currentParam);
            }
        }
        /// <summary>
        /// 移除所有参数
        /// </summary>
        public void RemoveParamAll()
        {
            List<string> keys = new List<string>();
            foreach (var s in paramCollection)
                keys.Add(s.Key);
            foreach (var s in keys)
            {
                var currentParam = paramCollection[s];
                if (currentParam != null)
                    paramCollection.Remove(currentParam);
            }

        }

        /// <summary>
        /// 根据Key改变参数值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetParamValue(string key, string value)
        {
            var currentParam = paramCollection[key];
            //if (currentParam != null)
            //    currentParam.Value = value;
            //else
            AddParam(key, value);
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="key"></param>
        public string GetParamValue(string key)
        {
            var currentParam = paramCollection[key];
            if (currentParam != null)
            {
                return currentParam.Value;
            }
            return string.Empty;
        }

        /// <summary>
        ///是否包含key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsParamKey(string key)
        {
            return paramCollection[key] != null;
        }
        #endregion

        /// <summary>
        /// 刷新页面
        /// </summary>
        public void Navigate()
        {
            if (ReportViewType == ReportViewType.Export)
                HtmlView.Navigate(reportExportContent, ReportAbsoluteUrl);
            else
                HtmlView.Navigate(reportContent, ReportAbsoluteUrl);

        }
        /// <summary>
        ///  导航绝对页面
        /// </summary>
        /// <param name="url"></param>
        public void Navigate(string url)
        {
            HtmlView.Navigate(reportContent, url);
        }

        /// <summary>
        /// 隐藏HtmlView
        /// </summary>
        public void HiddenHtmlView()
        {
            if (reportContent != null)
            {
                reportContent.Visibility = System.Windows.Visibility.Collapsed;
                LayoutRoot.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// 显示HtmlView
        /// </summary>
        public void ShowHtmlView()
        {
            if (reportContent != null)
            {
                reportContent.Visibility = System.Windows.Visibility.Visible;
                LayoutRoot.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 根据类型转换
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string ConvertReportPath(string path)
        {
            switch (ReportViewType)
            {
                case ReportViewType.View:
                    return path;
                case ReportViewType.Export:
                    int count = (path.IndexOf("&run.prompt") - path.IndexOf("&run.outputFormat") - "&run.outputFormat=".Length);
                    if (count == 0)
                    {
                        path = path.Insert(path.IndexOf("&run.outputFormat") + "&run.outputFormat=".Length, "XLWA");
                    }
                    else
                    {
                        path = path.Replace(path.Substring(path.IndexOf("&run.outputFormat") + "&run.outputFormat=".Length, count), "XLWA");
                    }
                    return path;
                default: return path;
            }
        }

        /// <summary>
        /// 参数起始分割符号
        /// </summary>
        /// <returns></returns>
        private string ParamStartDelimiter()
        {
            if (ReportPath.IndexOf("?", StringComparison.InvariantCultureIgnoreCase) > 0)
                return "&";
            else if (paramCollection.Count > 0)
                return "?";
            else
                return "";
            
        }
    }

    public class ReportViewParamCollection : List<ReportViewParam>
    {
        public ReportViewParamCollection()
        {
            //this.Add(new CognosViewParam { Key = "cv.header", Value = "false", Fixed = true });
            //this.Add(new CognosViewParam { Key = "cv.toolbar", Value = "false", Fixed = true });
        }

        public ReportViewParam this[string key]
        {
            get
            {
                return this.FirstOrDefault(item => item.Key.Equals(key));
            }
        }

        public bool VailUnionKey(string key)
        {
            return this.Count(item => item.Key.Equals(key)) < 1;
        }

        /// <summary>
        /// 参数转Url字符串
        /// </summary>
        /// <returns></returns>
        public string ParamInStr()
        {
            if (Count <= 0)
                return string.Empty;

            var paramStr = new StringBuilder();

            ForEach(item =>
            {
                paramStr.Append(item.ToString() + "&");
            });

            paramStr.Remove(paramStr.Length - 1, 1);

            return paramStr.ToString();

        }

    }

    /// <summary>
    /// congos参数
    /// </summary>
    public class ReportViewParam
    {
        public ReportViewParam()
        {
            Fixed = false;
        }

        public string Key { get; set; }

        public string Value { get; set; }

        public bool Fixed { get; set; }

        public override string ToString()
        {
            if (Fixed)
                return Key + "=" + Value;
            else
                return Key + "=" + Value;
        }
    }

    /// <summary>
    /// cognos视图类型
    /// </summary>
    public enum ReportViewType
    {
        /// <summary>
        /// 查询
        /// </summary>
        View = 0,
        /// <summary>
        /// 导出
        /// </summary>
        Export = 1,
        /// <summary>
        /// 打印
        /// </summary>
        Print = 2
    }
}
