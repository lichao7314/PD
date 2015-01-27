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

#endregion

namespace PD.Business
{
    /// <summary>
    ///   更新字段结构集合
    /// </summary>
    [Serializable]
    public class UpdateFieldConditionCollections : List<FieldCondition>
    {
        #region 方法

        /// <summary>
        ///   增加
        /// </summary>
        /// <param name = "fieldName">字段名</param>
        /// <param name = "value">值</param>
        public void Add(string fieldName, object value)
        {
            var fieldObject = new FieldCondition(fieldName, OperatorType.Equal, value);
            Add(fieldObject);
        }

        #endregion
    }
}