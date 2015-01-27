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
    ///   查询组
    /// </summary>
    [Serializable]
    public class QueryGroupCondition
    {
        #region 构造

        /// <summary>
        ///   构造
        /// </summary>
        public QueryGroupCondition()
        {
            FieldConditions = new FieldConditionCollections();
        }

        /// <summary>
        ///   构造
        /// </summary>
        /// <param name = "fieldConditions">字段结构集合<see cref = "FieldConditionCollections" /></param>
        public QueryGroupCondition(FieldConditionCollections fieldConditions)
            : this(fieldConditions, WhereUnionType.And)
        {
        }

        /// <summary>
        ///   构造
        /// </summary>
        /// <param name = "fieldConditions">字段结构集合<see cref = "FieldConditionCollections" /></param>
        /// <param name = "nextUnionType">下个查询组的条件连接</param>
        public QueryGroupCondition(FieldConditionCollections fieldConditions, WhereUnionType @nextUnionType)
        {
            FieldConditions = fieldConditions;
            NextUnionType = @nextUnionType;
        }

        #endregion

        #region 字段

        #endregion

        #region 属性

        /// <summary>
        ///   字段结构集合
        /// </summary>
        public FieldConditionCollections FieldConditions { get; set; }

        /// <summary>
        ///   下个查询组的条件连接
        /// </summary>
        public WhereUnionType NextUnionType { get; set; }

        #endregion
    }
}