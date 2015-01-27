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
    ///   分页结构
    /// </summary>
    [Serializable]
    public class Pagination
    {
        #region 构造函数

        /// <summary>
        ///   初始化 <see cref = "Pagination" /> 类的新实例。
        /// </summary>
        public Pagination()
            : this(20)
        {
        }

        /// <summary>
        ///   初始化 <see cref = "Pagination" /> 类的新实例。
        /// </summary>
        /// <param name = "isPaging">设置是否需要分页。</param>
        public Pagination(bool isPaging)
            : this(isPaging, 0, 0, 0)
        {
        }

        /// <summary>
        ///   初始化 <see cref = "Pagination" /> 类的新实例。
        /// </summary>
        /// <param name = "pageSize">设置每页的显示的数据行数。</param>
        public Pagination(int pageSize)
            : this(true, pageSize, 1, 0)
        {
        }

        /// <summary>
        ///   初始化 <see cref = "Pagination" /> 类的新实例。
        /// </summary>
        /// <param name = "pageSize">设置每页的显示的数据行数。</param>
        /// <param name = "currentIndex">设置当前页的索引值。</param>
        public Pagination(int pageSize, int currentIndex)
            : this(true, pageSize, currentIndex, 0)
        {
        }

        /// <summary>
        ///   初始化 <see cref = "Pagination" /> 类的新实例。
        /// </summary>
        /// <param name = "isPaging">设置是否需要分页。</param>
        /// <param name = "pageSize">设置每页的显示的数据行数。</param>
        /// <param name = "currentIndex">设置当前页的索引值。</param>
        /// <param name = "totalRecords">设置全部数据的行数。</param>
        private Pagination(bool isPaging, int pageSize, int currentIndex, int totalRecords)
        {
            IsPaging = isPaging;
            PageSize = pageSize;
            CurrentPageIndex = currentIndex;
            TotalRecords = totalRecords;
        }

        #endregion

        #region 字段

        #endregion

        #region 属性

        /// <summary>
        ///   获取或者设置是否需要分页。
        /// </summary>
        public bool IsPaging { get; set; }

        /// <summary>
        ///   获取或者设置每页的显示的数据行数。
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        ///   获取或者设置当前页的索引值。
        /// </summary>
        public int CurrentPageIndex { get; set; }

        /// <summary>
        ///   获取或者设置全部数据的行数。
        /// </summary>
        public int TotalRecords { get; set; }

        #endregion
    }
}