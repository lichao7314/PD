#region Copyright Information

// ***********************************************************************************
// Permission Management System Source Code
// Chang Sha Aite Technology Development Co., Ltd. All rights reserved
// Written by R & D
// Last Updated:
// ***********************************************************************************

#endregion

#region Using Section

using System;

#endregion

namespace PD.Business
{
    /// <summary>
    ///   表列属性
    /// </summary>
    [Serializable]
    public class TableColumnCondition
    {
        #region 构造

        /// <summary>
        ///   构造
        /// </summary>
        public TableColumnCondition()
        {
        }

        /// <summary>
        ///   构造
        /// </summary>
        /// <param name = "columnName">列名</param>
        public TableColumnCondition(string columnName)
        {
            ColumnName = columnName;
        }

        /// <summary>
        ///   构造
        /// </summary>
        /// <param name = "columnName">列名</param>
        /// <param name = "dataType">数据类型</param>
        public TableColumnCondition(string columnName, string dataType)
            : this(columnName)
        {
            DataType = dataType;
        }

        /// <summary>
        ///   构造
        /// </summary>
        /// <param name = "columnName">列名</param>
        /// <param name = "dataType">数据类型</param>
        /// <param name = "dataLength">数据长度</param>
        public TableColumnCondition(string columnName, string dataType, int dataLength)
            : this(columnName, dataType)
        {
            DataLength = dataLength;
        }

        /// <summary>
        ///   构造
        /// </summary>
        /// <param name = "columnName">列名</param>
        /// <param name = "dataType">数据类型</param>
        /// <param name = "dataLength">数据长度</param>
        /// <param name = "dataScale">数值范围</param>
        public TableColumnCondition(string columnName, string dataType, int dataLength, int dataScale)
            : this(columnName, dataType, dataLength)
        {
            DataScale = dataScale;
        }

        /// <summary>
        ///   构造
        /// </summary>
        /// <param name = "columnName">列名</param>
        /// <param name = "dataType">数据类型</param>
        /// <param name = "dataLength">数据长度</param>
        /// <param name = "dataScale">数值范围</param>
        /// <param name = "nullable">是否可空</param>
        public TableColumnCondition(string columnName, string dataType, int dataLength, int dataScale, bool nullable)
            : this(columnName, dataType, dataLength, dataScale)
        {
            Nullable = nullable;
        }

        /// <summary>
        ///   构造
        /// </summary>
        /// <param name = "columnName">列名</param>
        /// <param name = "dataType">数据类型</param>
        /// <param name = "dataLength">数据长度</param>
        /// <param name = "dataScale">数值范围</param>
        /// <param name = "nullable">是否可空</param>
        /// <param name = "comments">备注</param>
        public TableColumnCondition(string columnName, string dataType, int dataLength, int dataScale, bool nullable,
                                    string comments)
            : this(columnName, dataType, dataLength, dataScale, nullable)
        {
            Comments = comments;
        }

        #endregion

        #region 字段

        #endregion

        #region 属性

        /// <summary>
        ///   列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        ///   数据类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        ///   数据长度
        /// </summary>
        public int DataLength { get; set; }

        /// <summary>
        ///   数值范围
        /// </summary>
        public int DataScale { get; set; }

        /// <summary>
        ///   是否可空
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        ///   备注
        /// </summary>
        public string Comments { get; set; }

        #endregion
    }
}