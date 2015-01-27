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
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;

#endregion

namespace PD.Business
{
    /// <summary>
    ///   更新命令结构
    /// </summary>
    [Serializable]
    public class UpdateCommandCondition
    {
        #region 构造

        /// <summary>
        ///   构造
        /// </summary>
        public UpdateCommandCondition()
        {
            UpdateBehavior = DataUpdateBehavior.Standard;
        }

        /// <summary>
        ///   构造
        /// </summary>
        /// <param name = "dataSource">数据源</param>
        /// <param name = "commandType">命令类型</param>
        public UpdateCommandCondition(DataTable dataSource, UpdateCommandType commandType)
            : this()
        {
            this.dataSource = dataSource;
            //.GetChanges(DataRowState.Added | DataRowState.Modified | DataRowState.Deleted);
            UpdateCommandType = commandType;
        }

        /// <summary>
        ///   构造
        /// </summary>
        /// <param name = "dataSource">数据源</param>
        /// <param name = "commandType">命令类型</param>
        /// <param name = "updateBehavior">更新行为</param>
        public UpdateCommandCondition(DataTable dataSource, UpdateCommandType commandType, DataUpdateBehavior updateBehavior)
            : this(dataSource, commandType)
        {
            UpdateBehavior = updateBehavior;
        }

        #endregion

        #region 字段

        private DataTable dataSource;

        #endregion

        #region 属性

        /// <summary>
        ///   命令类型
        /// </summary>
        public UpdateCommandType UpdateCommandType { get; set; }

        /// <summary>
        ///   更新行为
        /// </summary>
        public DataUpdateBehavior UpdateBehavior { get; set; }

        /// <summary>
        ///   数据源
        /// </summary>
        public DataTable DataSource
        {
            get { return dataSource; }
            set { dataSource = value.GetChanges(DataRowState.Added | DataRowState.Modified | DataRowState.Deleted); }
        }

        #endregion
    }
}