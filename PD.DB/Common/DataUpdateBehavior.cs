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
    public enum DataUpdateBehavior
    {
        /// <summary>
        /// 标准
        /// </summary>
        Standard,
        /// <summary>
        /// 忽略错误、继续执行
        /// </summary>
        Continue,
        /// <summary>
        /// 事务回滚
        /// </summary>
        Transactional
    }
}