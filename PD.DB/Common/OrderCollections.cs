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
using System.Collections;
using System.Collections.Generic;
using PD.Business;

#endregion

namespace PD.Business
{
    /// <summary>
    ///   排序集合
    /// </summary>
    [Serializable]
    public class OrderCollections : DictionaryBase
    {
        #region 字段

        private readonly List<string> innerKeys = new List<string>();

        /// <summary>
        ///   值集合
        /// </summary>
        protected List<OrderByType> innerValues = new List<OrderByType>();

        #endregion

        #region 方法

        /// <summary>
        ///   通过字段名获取排序类型的索引器
        /// </summary>
        /// <param name = "fieldName">字段名</param>
        /// <returns>排序类型</returns>
        public virtual OrderByType this[string fieldName]
        {
            get { return Get(fieldName); }
            set { Add(fieldName, value); }
        }

        /// <summary>
        ///   通过索引获取排序类型的索引器
        /// </summary>
        /// <param name = "index">索引</param>
        /// <returns>排序类型</returns>
        public virtual OrderByType this[int index]
        {
            get { return innerValues[index]; }
        }

        /// <summary>
        ///   获取字段名集合
        /// </summary>
        public virtual string[] FieldNames
        {
            get { return innerKeys.ToArray(); }
        }

        /// <summary>
        ///   获取排序类型集合
        /// </summary>
        public virtual OrderByType[] OrderByTypes
        {
            get { return innerValues.ToArray(); }
        }

        /// <summary>
        ///   增加排序字段
        /// </summary>
        /// <param name = "fieldName">字段名</param>
        /// <param name = "orderByType">排序类型</param>
        public virtual void Add(string fieldName, OrderByType orderByType)
        {
            if (InnerHashtable.Contains(fieldName))
            {
                var index = innerValues.IndexOf((OrderByType) InnerHashtable[fieldName]);
                innerValues[index] = orderByType;
                InnerHashtable[fieldName] = orderByType;
            }
            else
            {
                innerKeys.Add(fieldName);
                InnerHashtable[fieldName] = orderByType;
                innerValues.Add(orderByType);
            }
        }

        /// <summary>
        ///   从集合增加排序字段
        /// </summary>
        /// <param name = "oc">集合</param>
        public virtual void AddRange(OrderCollections oc)
        {
            if (oc != null)
            {
                foreach (var text in oc.FieldNames)
                {
                    Add(text, oc[text]);
                }
            }
        }

        /// <summary>
        ///   清空
        /// </summary>
        public new void Clear()
        {
            InnerHashtable.Clear();
            innerValues.Clear();
            innerKeys.Clear();
        }

        /// <summary>
        ///   是否存在
        /// </summary>
        /// <param name = "fieldName">字段名</param>
        /// <returns>是否存在</returns>
        public virtual bool ContainsKey(string fieldName)
        {
            return InnerHashtable.Contains(fieldName);
        }

        /// <summary>
        ///   复制到数组
        /// </summary>
        /// <param name = "array">数组</param>
        public virtual void CopyTo(OrderByType[] array)
        {
            innerValues.CopyTo(array);
        }

        /// <summary>
        ///   复制到数组并制定起始索引
        /// </summary>
        /// <param name = "array">数组</param>
        /// <param name = "index">起始索引</param>
        public void CopyTo(OrderByType[] array, int index)
        {
            innerValues.CopyTo(array, index);
        }

        /// <summary>
        ///   根据索引获取排序类型
        /// </summary>
        /// <param name = "index">索引</param>
        /// <returns>排序类型</returns>
        public virtual OrderByType Get(int index)
        {
            return innerValues[index];
        }

        /// <summary>
        ///   根据字段名获取排序类型
        /// </summary>
        /// <param name = "fieldName">字段名</param>
        /// <returns>排序类型</returns>
        public virtual OrderByType Get(string fieldName)
        {
            if (InnerHashtable.Contains(fieldName))
            {
                return (OrderByType) InnerHashtable[fieldName];
            }
            return OrderByType.Asc;
        }

        /// <summary>
        ///   枚举元素
        /// </summary>
        /// <returns>枚举</returns>
        public new List<OrderByType>.Enumerator GetEnumerator()
        {
            return innerValues.GetEnumerator();
        }

        /// <summary>
        ///   删除字段排序
        /// </summary>
        /// <param name = "fieldName">字段名</param>
        public virtual void Remove(string fieldName)
        {
            if (InnerHashtable.Contains(fieldName))
            {
                var index = innerValues.IndexOf((OrderByType) InnerHashtable[fieldName]);
                innerValues.RemoveAt(index);
                innerKeys.Remove(fieldName);
                InnerHashtable.Remove(fieldName);
            }
        }

        #endregion

        #region Nested type: SynOrderCollections

        /// <summary>
        ///   异步排序集合
        /// </summary>
        internal class SynOrderCollections : OrderCollections
        {
            #region 字段

            private static readonly object syncRoot = new object();
            private readonly OrderCollections innerNoC;

            #endregion

            #region 构造

            /// <summary>
            ///   构造
            /// </summary>
            /// <param name = "noc">排序集合</param>
            public SynOrderCollections(OrderCollections noc)
            {
                innerNoC = noc;
            }

            #endregion

            #region 方法

            /// <summary>
            ///   通过字段名获取排序类型的索引器
            /// </summary>
            /// <param name = "fieldName">字段名</param>
            /// <returns>排序类型</returns>
            public override sealed OrderByType this[string fieldName]
            {
                get
                {
                    lock (syncRoot)
                    {
                        return innerNoC.Get(fieldName);
                    }
                }
                set
                {
                    lock (syncRoot)
                    {
                        innerNoC.Add(fieldName, value);
                    }
                }
            }

            /// <summary>
            ///   通过索引获取排序类型的索引器
            /// </summary>
            /// <param name = "index">索引</param>
            /// <returns>排序类型</returns>
            public override sealed OrderByType this[int index]
            {
                get
                {
                    lock (syncRoot)
                    {
                        return innerNoC.Get(index);
                    }
                }
            }

            /// <summary>
            ///   获取字段名集合
            /// </summary>
            public override sealed string[] FieldNames
            {
                get
                {
                    lock (syncRoot)
                    {
                        return innerNoC.FieldNames;
                    }
                }
            }

            /// <summary>
            ///   获取排序类型集合
            /// </summary>
            public override sealed OrderByType[] OrderByTypes
            {
                get
                {
                    lock (syncRoot)
                    {
                        return innerNoC.OrderByTypes;
                    }
                }
            }

            /// <summary>
            ///   增加排序字段
            /// </summary>
            /// <param name = "key">排序字段</param>
            /// <param name = "orderByType">排序类型</param>
            public override sealed void Add(string key, OrderByType orderByType)
            {
                lock (syncRoot)
                {
                    innerNoC.Add(key, orderByType);
                }
            }

            /// <summary>
            ///   增加排序字段集合
            /// </summary>
            /// <param name = "oc">排序字段集合</param>
            public override sealed void AddRange(OrderCollections oc)
            {
                lock (syncRoot)
                {
                    innerNoC.AddRange(oc);
                }
            }

            /// <summary>
            ///   清空
            /// </summary>
            public new void Clear()
            {
                lock (syncRoot)
                {
                    innerNoC.Clear();
                }
            }

            /// <summary>
            ///   是否存在
            /// </summary>
            /// <param name = "fieldName">字段名</param>
            /// <returns>是否存在</returns>
            public override sealed bool ContainsKey(string fieldName)
            {
                lock (syncRoot)
                {
                    return innerNoC.ContainsKey(fieldName);
                }
            }

            /// <summary>
            ///   根据索引获取排序类型
            /// </summary>
            /// <param name = "index">索引</param>
            /// <returns>排序类型</returns>
            public override sealed OrderByType Get(int index)
            {
                lock (syncRoot)
                {
                    return innerNoC.Get(index);
                }
            }

            /// <summary>
            ///   根据字段名获取排序类型
            /// </summary>
            /// <param name = "fieldName">字段名</param>
            /// <returns>排序类型</returns>
            public override sealed OrderByType Get(string fieldName)
            {
                lock (syncRoot)
                {
                    return innerNoC.Get(fieldName);
                }
            }

            /// <summary>
            ///   枚举
            /// </summary>
            /// <returns>枚举</returns>
            public new IEnumerator GetEnumerator()
            {
                lock (syncRoot)
                {
                    return innerNoC.GetEnumerator();
                }
            }

            /// <summary>
            ///   删除排序字段
            /// </summary>
            /// <param name = "fieldName">排序字段</param>
            public override sealed void Remove(string fieldName)
            {
                lock (syncRoot)
                {
                    innerNoC.Remove(fieldName);
                }
            }

            #endregion
        }

        #endregion
    }
}