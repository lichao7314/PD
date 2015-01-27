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
    ///   查询组集合
    /// </summary>
    [Serializable]
    public class QueryGroupConditionCollections : List<QueryGroupCondition>
    {
        /// <summary>
        ///   增加查询组
        /// </summary>
        /// <param name = "fieldConditions">字段结构集合<see cref = "FieldConditionCollections" /></param>
        public void Add(FieldConditionCollections fieldConditions)
        {
            var queryGroupObject = new QueryGroupCondition(fieldConditions);
            Add(queryGroupObject);
        }

        /// <summary>
        ///   增加查询组
        /// </summary>
        /// <param name = "fieldConditions">字段结构集合<see cref = "FieldConditionCollections" /></param>
        /// <param name = "nextUnionType">下个查询组的条件连接</param>
        public void Add(FieldConditionCollections fieldConditions, WhereUnionType @nextUnionType)
        {
            var queryGroupObject = new QueryGroupCondition(fieldConditions, @nextUnionType);
            Add(queryGroupObject);
        }
    }
}