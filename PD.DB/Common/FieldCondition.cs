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
    ///   字段结构
    /// </summary>
    [Serializable]
    public class FieldCondition
    {
        #region 构造

        /// <summary>
        ///   构造
        /// </summary>
        public FieldCondition()
        {
        }

        /// <summary>
        ///   构造
        /// </summary>
        /// <param name = "fieldName">字段名</param>
        /// <param name = "operator">表达式操作符</param>
        /// <param name = "value">值</param>
        public FieldCondition(string fieldName, OperatorType @operator, object value)
            : this(fieldName, @operator, value, WhereUnionType.And)
        {
        }

        /// <summary>
        ///   构造
        /// </summary>
        /// <param name = "fieldName">字段名</param>
        /// <param name = "operator">表达式操作符</param>
        /// <param name = "value">值</param>
        /// <param name = "nextUnionType">与下一个表达式的联合类型<see cref = "WhereUnionType" /></param>
        public FieldCondition(string fieldName, OperatorType @operator, object value, WhereUnionType @nextUnionType)
        {
            FieldName = fieldName;
            Operator = @operator;
            Value = value;
            NextUnionType = nextUnionType;
        }

        #endregion

        #region 字段

        #endregion

        #region 属性

        /// <summary>
        ///   字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        ///   下个条件的连接
        /// </summary>
        public WhereUnionType NextUnionType { get; set; }

        /// <summary>
        ///   表达式
        /// </summary>
        public OperatorType Operator { get; set; }

        /// <summary>
        ///   值
        /// </summary>
        public object Value { get; set; }

        #endregion
    }
}