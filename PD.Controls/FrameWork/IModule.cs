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
    /// 功能模块接口约定
    /// </summary>
    public interface IModule : IDisposable
    {
        /// <summary>
        /// 注册事件
        /// </summary>
        void RegisterEvent();
        /// <summary>
        /// 注销事件
        /// </summary>
        void UnRegisterEvent();
       
    }
}
