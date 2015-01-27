#region Copyright Information

// ***********************************************************************************
// Permission Management System Source Code
// Chang Sha Aite Technology Development Co., Ltd. All rights reserved
// Written by R & D
// Last Updated:
// ***********************************************************************************

#endregion

#region Using Section

using System.Collections.Generic;
using System.Data;
using PD.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;

#endregion

namespace PD.Business
{
    /// <summary>
    ///   更新命令结构集合
    /// </summary>
    public class UpdateCommandConditionCollections : List<UpdateCommandCondition>
    {
        #region 构造

        /// <summary>
        ///   构造
        /// </summary>
        public UpdateCommandConditionCollections()
        {
            UpdateBehavior = DataUpdateBehavior.Standard;
        }

        /// <summary>
        ///   构造
        /// </summary>
        /// <param name = "updateBehavior">更新行为</param>
        public UpdateCommandConditionCollections(DataUpdateBehavior updateBehavior)
        {
            UpdateBehavior = updateBehavior;
        }

        #endregion

        #region 字段

        #endregion

        #region 属性
        /// <summary>
        ///   更新行为
        /// </summary>
        public DataUpdateBehavior UpdateBehavior { get; set; }

        #endregion

        #region 方法

        /// <summary>
        ///   增加
        /// </summary>
        /// <param name = "dataSource">数据源</param>
        /// <param name = "commandType">命令类型</param>
        public void Add(DataTable dataSource, UpdateCommandType commandType)
        {
            var upCmd = new UpdateCommandCondition(dataSource, commandType, DataUpdateBehavior.Standard);
            Add(upCmd);
        }

        /// <summary>
        ///   增加
        /// </summary>
        /// <param name = "dataSource">数据源</param>
        /// <param name = "commandType">命令类型</param>
        /// <param name = "updateBehavior">更新行为</param>
        public void Add(DataTable dataSource, UpdateCommandType commandType, DataUpdateBehavior updateBehavior)
        {
            var upCmd = new UpdateCommandCondition(dataSource, commandType, updateBehavior);
            Add(upCmd);
        }

        #endregion
    }
}