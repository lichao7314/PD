using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;using PD.ServiceClient.PDService;

namespace PD.Controls
{
    /// <summary>
    /// 框架应用程序通用函数接口
    /// </summary>
    public interface IApplicationFramework  
    {
        /// <summary>
        /// 运行或者置顶(如果已经运行)一个模块功能（非窗口模式）
        /// </summary>
        /// <param name="menuCode">菜单代码</param>
        void ShowModule(string menuCode);

        /// <summary>
        /// 运行或者置顶(如果已经运行)一个模块功能（非窗口模式）
        /// </summary>
        /// <param name="menuCode">菜单代码</param>
        void ShowNavigation(T_BASE_MENU menu);
        /// <summary>
        ///  运行或者置顶(如果已经运行)一个模块功能（窗口模式）
        /// </summary>
        /// <param name="menuCode">菜单代码</param>
        //void ShowDialogModule(string menuCode);
        /// <summary>
        /// 开始绘制等待指示器在整个框架上
        /// </summary>
        void BeginLoading();
        /// <summary>
        /// 结束绘制等待指示器在整个框架上
        /// </summary>
        void EndLoading();
        /// <summary>
        /// 开始绘制等待指示器在当前页面上
        /// </summary>
        /// <param name="element">当前窗体上的一个控件</param>
        void BeginWindowLoading(DependencyObject element);
        /// <summary>
        /// 结束绘制等待指示器在当前页面上
        /// </summary>
        /// <param name="element">当前窗体上的一个控件</param>
        void EndWindowLoading(DependencyObject element);

        string IPAddress { get;  }

        void HiddenHtmlView();

        void ShowHtmlView();

        T_PB_USER Profile { get; set; }

        DateTime ServerTime { get; }

        /// <summary>
        /// 最大化窗体
        /// </summary>
        void MaxWindow();

        /// <summary>
        /// 恢复默认窗体
        /// </summary>
        void DefaultWindow();
        /// <summary>
        /// 更新数据完成
        /// </summary>
        event EventHandler UpdateDataCompleted;
    }

}
