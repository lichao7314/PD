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

namespace PD.Controls
{    
    /// <summary>
    /// 功能区域背景样式面板
    /// </summary>
    [TemplatePart(Name = PartBorder, Type = typeof(Border))]
    public class ModuleAreaPanel : ContentControl 
    {
        private const string PartBorder = "borderPanel";

        private Border _panel = null;

        private string _currentStyle = string.Empty;

        private string _currentPanelStyle = string.Empty;

        public ModuleAreaPanel()
        {
            DefaultStyleKey = typeof(ModuleAreaPanel);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _panel = GetTemplateChild(PartBorder) as Border;
            SetStyle();
        }

        private void SetStyle()
        {
            switch (PanelType)
            {
                case PanelType.MenuArea: _currentStyle = "MenuAreaStyle";  _currentPanelStyle = "MenuAreaPanelStyle"; break;
                case PanelType.DataArea:  _currentStyle = "DataAreaStyle";  _currentPanelStyle = "DataAreaPanelStyle"; break;
                case PanelType.QueryArea:  _currentStyle = "QueryAreaStyle";  _currentPanelStyle = "QueryAreaPanelStyle"; break;
                case PanelType.ToolBarArea:  _currentStyle = "ToolBarAreaStyle";  _currentPanelStyle = "ToolBarAreaPanelStyle"; break;
                default:
                    throw new ArgumentNullException("没有找到对应面板类型");
            }
            if ( _panel != null)
            {
                 _panel.Style = Application.Current.Resources[_currentStyle] as Style;
            }
            Style = Application.Current.Resources[_currentPanelStyle] as Style;
        }

        /// <summary>
        /// 面板区域类型
        /// </summary>
        public PanelType PanelType
        {
            get { return (PanelType)GetValue(PanelTypeProperty); }
            set { SetValue(PanelTypeProperty, value); }
        }

        public static readonly DependencyProperty PanelTypeProperty =
           DependencyProperty.Register(
           "PanelType",
           typeof(PanelType),
           typeof(ModuleAreaPanel),
           new PropertyMetadata(OnPanelTypePropertyChanged));

        private static void OnPanelTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ModuleAreaPanel)
            {
                var panel = d as ModuleAreaPanel;
                panel.SetStyle();
            }
        }
    }
    public enum PanelType
    {
        /// <summary>
        /// 数据区域
        /// </summary>
        DataArea = 0,
        /// <summary>
        /// 查询区域
        /// </summary>
        QueryArea = 1,
        /// <summary>
        /// 操作区域
        /// </summary>
        ToolBarArea = 2,
        /// <summary>
        /// 菜单区域
        /// </summary>
        MenuArea = 3
    }
}
