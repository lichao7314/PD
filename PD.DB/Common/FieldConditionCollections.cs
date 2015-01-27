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
using System.Collections.Generic;
using PD.Business;

#endregion

namespace PD.Business
{
    /// <summary>
    ///   字段结构集合
    /// </summary>
    [Serializable]
    public class FieldConditionCollections : List<FieldCondition>
    {
        #region 方法

        /// <summary>
        ///   增加
        /// </summary>
        /// <param name = "fieldName">字段名</param>
        /// <param name = "operator">表达式操作符</param>
        /// <param name = "value">值</param>
        /// <param name = "nextUnionType">与下一个表达式的联合类型<see cref = "WhereUnionType" /></param>
        public void Add(string fieldName, OperatorType @operator, object value, WhereUnionType @nextUnionType)
        {
            var fieldObject = new FieldCondition(fieldName, @operator, value, @nextUnionType);
            Add(fieldObject);
        }

        /// <summary>
        ///   增加
        /// </summary>
        /// <param name = "fieldName">字段名</param>
        /// <param name = "operator">表达式操作符</param>
        /// <param name = "value">值</param>
        public void Add(string fieldName, OperatorType @operator, object value)
        {
            var fieldObject = new FieldCondition(fieldName, @operator, value);
            Add(fieldObject);
        }

        #endregion
    }
}