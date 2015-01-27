using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Collections;
using PD.ServiceClient;
using System;

namespace PD.Controls
{
    public static class CommonMethod
    {
        /// <summary>
        /// 从对象中获取其中的button
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="index">索引</param>
        /// <param name="keyName">ButtonKey</param>
        /// <returns></returns>
        public static DependencyObject GetChildIsButtonType(DependencyObject obj, int index, string keyName)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                try
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if ((child as Button) != null)
                    {
                        if ((child as Button).Name == keyName)
                            return child;
                    }
                    else
                    {
                        DependencyObject obj1 = GetChildIsButtonType(child, i, keyName);
                        if ((obj1 as Button) != null && (obj1 as Button).Name == keyName)
                        {
                            return obj1;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
            return VisualTreeHelper.GetParent(VisualTreeHelper.GetChild(obj, index));
        }

        public static TChild GetElementByName<TChild>(DependencyObject owner, string name) where TChild : class
        {
            int icount = VisualTreeHelper.GetChildrenCount(owner);
            if (icount == 0)
                return null;
            for (int n = 0; n < icount; n++)
            {
                DependencyObject el = VisualTreeHelper.GetChild(owner, n);
                if (((el as FrameworkElement) != null && (el as FrameworkElement).Name == name) &&
                    ((el as TChild) != null))
                    return (el as TChild);
                var f = GetElementByName<TChild>(el, name);
                if (f != null) return f;
            }
            return null;
        }

        public static void GetElementCollection<TChild>(DependencyObject owner, List<TChild> results) where TChild : class
        {
            int icount = VisualTreeHelper.GetChildrenCount(owner);
            if (icount == 0)
                return;
            for (int n = 0; n < icount; n++)
            {
                DependencyObject el = VisualTreeHelper.GetChild(owner, n);
                if ((el as TChild) != null)
                    results.Add(el as TChild);
                GetElementCollection(el, results);
            }
            return;
        }

        /// <summary>
        /// 使用可视化树查找对象中的FrameworkElement元素
        /// </summary>
        /// <typeparam name="TChild">目标对象类型（必须为FrameworkElement的子类）</typeparam>
        /// <param name="owner">用于搜索的集合</param>
        /// <param name="name">对象名（Name属性值）</param>
        /// <returns></returns>
        public static TChild GetFrameworkElementByName<TChild>(DependencyObject owner, string name) where TChild : FrameworkElement
        {
            for (int index = 0; index < VisualTreeHelper.GetChildrenCount(owner); index++)
            {
                DependencyObject el = VisualTreeHelper.GetChild(owner, index);
                if (el is TChild && ((TChild)el).Name == name)
                {
                    return (TChild)el;
                }
                else
                {
                    TChild findedControl = GetFrameworkElementByName<TChild>(el, name);
                    if (findedControl != null) return findedControl;
                }
            }
            return null;
        }

        public static Size DialogSize(FrameworkElement element)
        {
            return new Size()
            {
                Height = element.ActualHeight - DialogPaddingTop * 2,
                Width = element.ActualWidth - DialogPaddingLeft * 2
            };
        }

        public static double DialogPaddingTop
        {
            get
            {
                return 0;
            }
        }
        public static double DialogPaddingLeft
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 返回包含中文字符的字符串长度
        /// C# 的string.Length中中文字只做1位统计,所以要将其转换为2位
        /// </summary>
        /// <param name="strSource">要统计长度的字符串变量</param>
        /// <returns>字符串长度</returns>
        public static int GetLength(string strSource)
        {
            if (string.IsNullOrEmpty(strSource))
                return 0;
            Regex regex = new Regex("[\u4e00-\u9fa5]+", RegexOptions.Multiline);

            int nLength = strSource.Length;

            for (int i = 0; i < strSource.Length; i++)
            {
                if (regex.IsMatch(strSource.Substring(i, 1)))
                {
                    nLength++;
                }
            }

            return nLength;
        }

        /// <summary>
        /// 字符串是否为空
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool NullMessage(string strSource, string message)
        {
            if (string.IsNullOrEmpty(strSource))
            {
                MessageBox.Show(message);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置数据源
        /// </summary>
        /// <param name="box"></param>
        /// <param name="source"></param>
        /// <param name="isSelect"></param>
        public static void SetSource(this ComboBox box, IList source, bool isSelect)
        {
            if (source != null)
            {
                box.ItemsSource = source;
                if (isSelect && source.Count > 0)
                {
                    box.SelectedIndex = 0;
                }
            }
        }
        /// <summary>
        /// 字符转Double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToDouble(this string value)
        {
            var outValue = 0d;
            double.TryParse(value, out outValue);
            return outValue;
        }  /// <summary>
        /// 字符转Double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this string value)
        {
            var outValue = 0;
            int.TryParse(value, out outValue);
            return outValue;
        }

        /// <summary>
        /// 设置数据源
        /// </summary>
        /// <param name="box"></param>
        /// <param name="source"></param>
        /// <param name="isSelect"></param>
        public static void SetSource(this ComboBox box, IList source, string selectedValuePath, string displayMemberPath, bool isSelect)
        {
            if (source != null)
            {
                box.SelectedValuePath = selectedValuePath;
                box.DisplayMemberPath = displayMemberPath;
                box.ItemsSource = source;
                if (isSelect && source.Count > 0)
                {
                    box.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// 获取silverlight当前域的宽度
        /// </summary>
        /// <returns></returns>
        public static double GetRenderSizeWidth()
        {
            return (Application.Current.RootVisual).RenderSize.Width;
        }
        /// <summary>
        /// 获取silverlight当前域的高度
        /// </summary>
        /// <returns></returns>
        public static double GetRenderSizeHeight()
        {
            return (Application.Current.RootVisual).RenderSize.Height;
        }

        public static void BeginWindowLoading(this DependencyObject ui)
        {
            (Application.Current as IApplicationFramework).BeginWindowLoading(ui);
        }
        public static void EndWindowLoading(this DependencyObject ui)
        {
            (Application.Current as IApplicationFramework).EndWindowLoading(ui);
        }

        public static string FileAddress
        {
            get
            {
                if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
                    return WcfConfigs.Instance.GetConfig("PDService").Uri.Replace("PDService.svc", "")+@"PDUpLoadFile/";
                else
                    return "";
            }
        }
        /// <summary>
        /// 获取上传后的文件的网络地址   
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Uri FileNetAddress(string path) {
            return new Uri(CommonMethod.FileAddress + path.Replace(@"\", "/")+"?time="+Guid.NewGuid().ToString());
        }
    }

}