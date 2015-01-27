#region Copyright Information

// ***********************************************************************************
// Permission Management System Source Code
// Chang Sha Aite Technology Development Co., Ltd. All rights reserved
// Written by R & D
// Last Updated:
// ***********************************************************************************

#endregion

namespace PD.Business
{
    /// <summary>
    ///   命令类型
    /// </summary>
    public enum UpdateCommandType
    {
        /// <summary>
        ///   增加
        /// </summary>
        Insert,
        /// <summary>
        ///   更新
        /// </summary>
        Update,
        /// <summary>
        ///   删除
        /// </summary>
        Delete,
        /// <summary>
        ///   全部操作
        /// </summary>
        All
    }
}