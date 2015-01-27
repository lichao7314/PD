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
    ///   操作符
    /// </summary>
    public enum OperatorType
    {
        /// <summary>
        ///   等于。
        /// </summary>
        Equal,
        /// <summary>
        ///   不等于。
        /// </summary>
        Unequal,
        /// <summary>
        ///   大于。
        /// </summary>
        Greater,
        /// <summary>
        ///   大于等于。
        /// </summary>
        GreaterAndEqual,
        /// <summary>
        ///   小于。
        /// </summary>
        LessThan,
        /// <summary>
        ///   小于等于。
        /// </summary>
        LessThanAndEqual,
        /// <summary>
        ///   像...一样。
        /// </summary>
        Like,
        /// <summary>
        ///   不像...一样。
        /// </summary>
        NotLike,
        /// <summary>
        ///   是。
        /// </summary>
        Is,
        /// <summary>
        ///   不是
        /// </summary>
        IsNot,
        /// <summary>
        ///   在...之中
        /// </summary>
        In,
        /// <summary>
        ///   不在...之中
        /// </summary>
        NotIn,
        /// <summary>
        ///   Sql 文本拼接
        /// </summary>
        SqlText
    }
}