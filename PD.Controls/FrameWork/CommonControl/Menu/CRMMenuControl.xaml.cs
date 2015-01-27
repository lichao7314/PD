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
using System.Collections;
using System.Windows.Data;

namespace PD.Controls 
{
    public partial class CRMMenuControl : UserControl, IDisposable
    {
        public CRMMenuControl()
        {
            InitializeComponent();
        }

        public event MenuEventHandler TopMenuClick;

        #region 菜单数据源改变提供通知
        /// <summary>
        /// 菜单数据源
        /// </summary>
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register(
            "ItemSource",
            typeof(IEnumerable),
            typeof(CRMMenuControl),
            new PropertyMetadata(OnItemSourceChanged));
        /// <summary>
        /// 显示的成员
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
          DependencyProperty.Register(
          "DisplayMemberPath",
          typeof(string),
          typeof(CRMMenuControl),
          new PropertyMetadata("", OnItemSourceChanged));
        /// <summary>
        /// 获取或设置需要显示的成员
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public IEnumerable ItemSource
        {
            get { return (IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        protected static void OnItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CRMMenuControl)
            {
                var topMenuControl = (CRMMenuControl)d;
                if (topMenuControl.ItemSource == null)
                    return;
                if (topMenuControl.rootMenuPanel.Children.Count > 0)
                {
                    topMenuControl.rootMenuPanel.Children.Clear();
                }
                var topMenuGroupName = Guid.NewGuid().ToString();
                var topMenuEnumerator = topMenuControl.ItemSource.GetEnumerator();
                while (topMenuEnumerator.MoveNext())
                {
                    var topMenuButton = new RadioButton()
                    {
                        Style = Application.Current.Resources["topMenuButtonStyle"] as Style,
                        DataContext = topMenuEnumerator.Current,
                        GroupName = topMenuGroupName,
                        Margin = new Thickness(0, 0, 1, 0),
                        Padding = new Thickness(10, 5, 10, 1),
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    topMenuButton.SetBinding(RadioButton.ContentProperty, new System.Windows.Data.Binding()
                    {
                        Converter = new TopMenuContentValueConverter(),
                        ConverterParameter = topMenuControl.DisplayMemberPath
                    });
                    topMenuButton.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(topMenuControl.MenuItemButtonClick), true);
                    topMenuControl.rootMenuPanel.Children.Add(topMenuButton);
                }
            }
        }

        private void MenuItemButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (TopMenuClick != null)
            {
                if (sender is RadioButton)
                {
                    (sender as RadioButton).IsChecked = true;
                    TopMenuClick(this, new MenuEventArgs()
                    {
                        DataContext = (sender as RadioButton).DataContext
                    });
                }
            }
        }

        public void SelectMenuItem(int i)
        {
            if (rootMenuPanel.Children.Count > i && rootMenuPanel.Children[i] is RadioButton)
            {
                var button = (rootMenuPanel.Children[i] as RadioButton);

                button.IsChecked = true;

                MenuItemButtonClick(button, null);
            }
        }
        #endregion

        public void Dispose()
        {
            foreach (var child in rootMenuPanel.Children)
            {
                if (child is RadioButton)
                {
                    (child as RadioButton).RemoveHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(MenuItemButtonClick));
                }
            }
        }
    }
    /// <summary>
    /// 获取value中某个成员的值
    /// </summary>
    public class TopMenuContentValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType().GetProperty(parameter + "") != null)
            {
                return value.GetType().GetProperty(parameter + "").GetValue(value, null);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public delegate void MenuEventHandler(object sender, MenuEventArgs e);

    public class MenuEventArgs : EventArgs
    {
        public object DataContext { get; set; }
    }

}
