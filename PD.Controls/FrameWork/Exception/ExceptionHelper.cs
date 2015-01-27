using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
namespace PD.Controls
{
    /// <summary>
    /// 异常辅助类
    /// </summary>
    public class ExceptionHelper
    {
        #region ShowExceptionWindow


        /// <summary>
        /// 显示异常信息窗口
        /// </summary>
        /// <param name="ex">异常变量</param>
        /// <param name="ClosedMethod">窗口关闭事件</param>
        public static void ShowExceptionWindow(System.Exception ex, EventHandler ClosedMethod)
        {
            PopupExceptionWindow.ShowExceptionWindow(ex, ClosedMethod);
        }
       
        #endregion

        #region GetInnerException
        /// <summary>
        /// 得到内部的异常信息
        /// </summary>
        /// <param name="ex">指定的异常</param>
        /// <returns>返回最内部的异常值</returns>
        public static System.Exception GetInnerException(System.Exception ex)
        {
            System.Exception result;
            result = ex;
            while (result.InnerException != null)
            {
                result = result.InnerException;
            }
            return result;
        }
        #endregion
    }
}
