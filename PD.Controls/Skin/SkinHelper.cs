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

namespace PD.Controls
{
    /// <summary>
    /// 更换主题辅助函数
    /// </summary>
    public class SkinHelper
    {
        private static SkinDictionary skinFile = new SkinDictionary();
        /// <summary>
        /// 皮肤文件集合信息
        /// </summary>
        public static SkinDictionary SkinDictionary
        {
            get
            {
                return skinFile;
            }
        }
        /// <summary>
        /// 清空资源
        /// </summary>
        public static void ClearResource()
        {
            Application.Current.Resources.MergedDictionaries.Clear();

            Application.Current.Resources.Clear();
        }

        /// <summary>
        /// 添加一个资源字典到Appresource中
        /// </summary>
        /// <param name="resourceDictionary">资源路径</param>
        public static void AddResource(string resourceDictionary)
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(resourceDictionary, UriKind.Relative)
            });
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">资源的唯一标识键</param>
        /// <returns></returns>
        public static T Resource<T>(string key)
        {
            var resource = Application.Current.Resources[key];
            if (resource != null && resource is T)
            {
                return (T)resource;
            }
            else
            {
                return default(T);
            }
        }
       
        /// <summary>
        /// 写当前皮肤信息到隔离缓存
        /// </summary>
        public static void WirteCurrentSkin()
        {
            throw new Exception("方法未实现");
        }  /// <summary>
        /// 读取隔离缓存中当前皮肤信息
        /// </summary>
        public static Skin ReadCurrentSkin()
        {

            return new Skin {  SkinFontFamily= SkinFontFamily.DefaultFont, SkinFontSize= SkinFontSize.Default, SkinType= SkinType.Blue};
        }
    }

    public class Skin
    {
        public SkinType SkinType { get; set; }
        public SkinFontFamily SkinFontFamily { get; set; }
        public SkinFontSize SkinFontSize { get; set; }
    }

    public enum SkinType
    {
        /// <summary>
        /// 蓝色
        /// </summary>
        Blue = 0,
    }

    /// <summary>
    /// 字体
    /// </summary>
    public enum SkinFontFamily
    {
        DefaultFont=1
    }

    /// <summary>
    /// 字号
    /// </summary>
    public enum SkinFontSize:int
    {
        Default=12,
        Medium=13,
        Big=14
    }

    public class SkinDictionary
    {
        /// <summary>
        /// 皮肤颜色映射表
        /// </summary>
        public Dictionary<SkinType, string> ColorDictionary
        {
            get;
            private set;
        }
        /// <summary>
        /// 皮肤控件映射表
        /// </summary>
        public Dictionary<SkinType, string> ControlDictionary
        {
            get;
            private set;
        }
        /// <summary>
        /// 皮肤名称映射表
        /// </summary>
        public Dictionary<SkinType, string> SkinNameDictionary
        {
            get;
            private set;
        }
        /// <summary>
        /// 皮肤图标背景颜色映射表
        /// </summary>
        public Dictionary<SkinType, Brush> SkinColorDictionary;

        public Dictionary<SkinFontFamily, string> SkinFontFamilyDictionary;

        public Dictionary<SkinFontSize, string> SkinFontSizeDictionary;

        public SkinDictionary()
        {
            SkinFontSizeDictionary = new Dictionary<SkinFontSize, string>();
            SkinFontFamilyDictionary = new Dictionary<SkinFontFamily, string>();
            ColorDictionary = new Dictionary<SkinType, string>();
            ControlDictionary = new Dictionary<SkinType, string>();
            SkinNameDictionary = new Dictionary<SkinType, string>();
            SkinColorDictionary = new Dictionary<SkinType, Brush>();
            //字体
            SkinFontFamilyDictionary.Add(SkinFontFamily.DefaultFont, "/PD.Controls;component/Skin/Font/FontFamily/FontFamily.xaml");
            //字号
            SkinFontSizeDictionary.Add(SkinFontSize.Big, "/PD.Controls;component/Skin/Font/FontSize/BigFontSize.xaml");
            SkinFontSizeDictionary.Add(SkinFontSize.Default, "/PD.Controls;component/Skin/Font/FontSize/DefaultFontSize.xaml");
            SkinFontSizeDictionary.Add(SkinFontSize.Medium, "/PD.Controls;component/Skin/Font/FontSize/MediumFontSize.xaml");

            //皮肤
            SkinColorDictionary.Add(SkinType.Blue, new SolidColorBrush { Color = Color.FromArgb(255, 102, 161, 237) });

            SkinNameDictionary.Add(SkinType.Blue, "天蓝");
         
            ColorDictionary.Add(SkinType.Blue, "/PD.Controls;component/Skin/Color/Blue.xaml");
      
            ControlDictionary.Add(SkinType.Blue, "/PD.Controls;component/Skin/Control/BureauBlueTheme.xaml");
        }
    }
}
