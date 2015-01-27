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

namespace PD.Controls.FrameWork
{
    public enum SystemType
    {
        /// <summary>
        /// 墙扣
        /// </summary>
        QK = 0,
        /// <summary>
        /// 扣板
        /// </summary>
        KB = 1
    }

    public class CurrentSystemType
    {
        public static CurrentSystemType Instance
        {
            get;
            set;
        }

        private SystemType Type
        {
            get;
            set;
        }
        public CurrentSystemType(SystemType type)
        {
            Type = type;
            SystemType = Convert.ToInt32(Type);
        }
        /// <summary>
        /// 系统类型
        /// </summary>
        public int SystemType
        {
            get;
            private set;
        }
    }
}
