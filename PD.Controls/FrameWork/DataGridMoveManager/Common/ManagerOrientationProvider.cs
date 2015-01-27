
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
    /// 控件方向管理
    /// </summary>
    public class ManagerOrientationProvider
    {
        public static IOrientationProvider InitControlPrivider(FrameworkElement control)
        {
            if (control == null)
                return null;
            if (ProviderCollection.Providers.ContainsKey(control.GetType()))
            {
                IOrientationProvider provider = typeof(IOrientationProvider).Assembly.CreateInstance(ProviderCollection.Providers[control.GetType()]) as IOrientationProvider;

                return provider;
            }
            else
            {
                IOrientationProvider provider = new ElementOrientationProvider();//如果找不到对于的Provider 则使用默认元素Provider

                return provider;
            }
        }
    }

    /// <summary>
    /// 控件Provider集合
    /// </summary>
    public class ProviderCollection
    {
        private static Dictionary<Type, string> provider = new Dictionary<Type, string>();

        public static Dictionary<Type, string> Providers
        {
            get
            {
                ControlTypeProvider();

                return ProviderCollection.provider;
            }
        }

        private static Dictionary<Type, string> ControlTypeProvider()
        {
            if (provider.Count <= 0)
            {
                provider.Add(typeof(TextBox), typeof(TextBoxOrientationProvider).FullName);
                provider.Add(typeof(ComboBox), typeof(ComboxOrientationProvider).FullName);
            }
            return provider;
        }
    }
}
