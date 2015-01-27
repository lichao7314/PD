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
    public class EditColumn
    {
        public EditColumn()
        {
            MoveSpan = MoveSpan.Default;
        }
        /// <summary>
        /// 列方向移动的跨度的行为
        /// </summary>
        public MoveSpan MoveSpan { get; set; }
        /// <summary>
        /// 编辑列的索引
        /// </summary>
        public int ColumnIndex { get; set; }
        [System.ComponentModel.EditorBrowsable( System.ComponentModel.EditorBrowsableState.Never)]
        internal DataGridColumn Column { get; set; }
    }
    /// <summary>
    /// 当前列移动跳转类型
    /// </summary>
    public enum MoveSpan
    {
        /// <summary>
        /// 默认（左右移动自动移动到下一个列）
        /// </summary>
        Default = 0,
        /// <summary>
        ///左右移动自动移动到下一个设置的编辑列
        /// </summary>
        NextEditColumn = 1
    }
}
