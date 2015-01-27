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
    ///   表字段集合
    /// </summary>
    [Serializable]
    public class TableColumnConditionCollections : DictionaryBase
    {
        #region 字段

        private static readonly TableColumnConditionCollections tableColumns = new TableColumnConditionCollections();

        private readonly List<string> innerKeys = new List<string>();

        /// <summary>
        ///   值集合
        /// </summary>
        protected List<List<TableColumnCondition>> innerValues = new List<List<TableColumnCondition>>();

        #endregion

        #region 方法

        /// <summary>
        ///   通过表名获取字段集合的索引器
        /// </summary>
        /// <param name = "tableName">表名</param>
        /// <returns>字段集合</returns>
        public virtual List<TableColumnCondition> this[string tableName]
        {
            get { return Get(tableName); }
            set { Add(tableName, value); }
        }

        /// <summary>
        ///   通过索引获取字段集合的索引器
        /// </summary>
        /// <param name = "index">索引</param>
        /// <returns>字段集合</returns>
        public virtual List<TableColumnCondition> this[int index]
        {
            get { return innerValues[index]; }
        }

        /// <summary>
        ///   获取表名集合
        /// </summary>
        public virtual string[] FieldNames
        {
            get { return innerKeys.ToArray(); }
        }

        /// <summary>
        ///   获取字段表集合
        /// </summary>
        public virtual List<TableColumnCondition>[] Tables
        {
            get { return innerValues.ToArray(); }
        }

        public static TableColumnConditionCollections Instance()
        {
            return tableColumns;
        }

        /// <summary>
        ///   增加字段集合
        /// </summary>
        /// <param name = "tableName">表名</param>
        /// <param name = "tableColumnList">字段集合</param>
        public virtual void Add(string tableName, List<TableColumnCondition> tableColumnList)
        {
            if (InnerHashtable.Contains(tableName))
            {
                var index = innerValues.IndexOf((List<TableColumnCondition>) InnerHashtable[tableName]);
                innerValues[index] = tableColumnList;
                InnerHashtable[tableName] = tableColumnList;
            }
            else
            {
                innerKeys.Add(tableName);
                InnerHashtable[tableName] = tableColumnList;
                innerValues.Add(tableColumnList);
            }
        }

        /// <summary>
        ///   从集合增加字段集合
        /// </summary>
        /// <param name = "oc">集合</param>
        public virtual void AddRange(TableColumnConditionCollections oc)
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
        /// <param name = "tableName">表名</param>
        /// <returns>是否存在</returns>
        public virtual bool ContainsKey(string tableName)
        {
            return InnerHashtable.Contains(tableName);
        }

        /// <summary>
        ///   复制到数组
        /// </summary>
        /// <param name = "array">数组</param>
        public virtual void CopyTo(List<TableColumnCondition>[] array)
        {
            innerValues.CopyTo(array);
        }

        /// <summary>
        ///   复制到数组并制定起始索引
        /// </summary>
        /// <param name = "array">数组</param>
        /// <param name = "index">起始索引</param>
        public void CopyTo(List<TableColumnCondition>[] array, int index)
        {
            innerValues.CopyTo(array, index);
        }

        /// <summary>
        ///   根据索引获取字段集合
        /// </summary>
        /// <param name = "index">索引</param>
        /// <returns>字段集合</returns>
        public virtual List<TableColumnCondition> Get(int index)
        {
            return innerValues[index];
        }

        /// <summary>
        ///   根据表名获取字段集合
        /// </summary>
        /// <param name = "tableName">表名</param>
        /// <returns>字段集合</returns>
        public virtual List<TableColumnCondition> Get(string tableName)
        {
            if (InnerHashtable.Contains(tableName))
            {
                return (List<TableColumnCondition>) InnerHashtable[tableName];
            }
            return new List<TableColumnCondition>();
        }

        /// <summary>
        ///   枚举元素
        /// </summary>
        /// <returns>枚举</returns>
        public new List<List<TableColumnCondition>>.Enumerator GetEnumerator()
        {
            return innerValues.GetEnumerator();
        }

        /// <summary>
        ///   删除字段排序
        /// </summary>
        /// <param name = "tableName">表名</param>
        public virtual void Remove(string tableName)
        {
            if (InnerHashtable.Contains(tableName))
            {
                var index = innerValues.IndexOf((List<TableColumnCondition>) InnerHashtable[tableName]);
                innerValues.RemoveAt(index);
                innerKeys.Remove(tableName);
                InnerHashtable.Remove(tableName);
            }
        }

        #endregion

        #region Nested type: SynTableColumnConditionCollections

        /// <summary>
        ///   异步排序集合
        /// </summary>
        internal class SynTableColumnConditionCollections : TableColumnConditionCollections
        {
            #region 字段

            private static readonly object syncRoot = new object();
            private readonly TableColumnConditionCollections innerNoC;

            #endregion

            #region 构造

            /// <summary>
            ///   构造
            /// </summary>
            /// <param name = "noc">排序集合</param>
            public SynTableColumnConditionCollections(TableColumnConditionCollections noc)
            {
                innerNoC = noc;
            }

            #endregion

            #region 方法

            /// <summary>
            ///   通过表名获取字段集合的索引器
            /// </summary>
            /// <param name = "tableName">表名</param>
            /// <returns>字段集合</returns>
            public override sealed List<TableColumnCondition> this[string tableName]
            {
                get
                {
                    lock (syncRoot)
                    {
                        return innerNoC.Get(tableName);
                    }
                }
                set
                {
                    lock (syncRoot)
                    {
                        innerNoC.Add(tableName, value);
                    }
                }
            }

            /// <summary>
            ///   通过索引获取字段集合的索引器
            /// </summary>
            /// <param name = "index">索引</param>
            /// <returns>字段集合</returns>
            public override sealed List<TableColumnCondition> this[int index]
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
            ///   获取表名集合
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
            ///   获取字段表集合
            /// </summary>
            public override sealed List<TableColumnCondition>[] Tables
            {
                get
                {
                    lock (syncRoot)
                    {
                        return innerNoC.Tables;
                    }
                }
            }

            /// <summary>
            ///   增加字段集合
            /// </summary>
            /// <param name = "key">字段集合</param>
            /// <param name = "tableColumnList">字段集合</param>
            public override sealed void Add(string key, List<TableColumnCondition> tableColumnList)
            {
                lock (syncRoot)
                {
                    innerNoC.Add(key, tableColumnList);
                }
            }

            /// <summary>
            ///   增加字段表集合
            /// </summary>
            /// <param name = "oc">字段表集合</param>
            public override sealed void AddRange(TableColumnConditionCollections oc)
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
            /// <param name = "tableName">表名</param>
            /// <returns>是否存在</returns>
            public override sealed bool ContainsKey(string tableName)
            {
                lock (syncRoot)
                {
                    return innerNoC.ContainsKey(tableName);
                }
            }

            /// <summary>
            ///   根据索引获取字段集合
            /// </summary>
            /// <param name = "index">索引</param>
            /// <returns>字段集合</returns>
            public override sealed List<TableColumnCondition> Get(int index)
            {
                lock (syncRoot)
                {
                    return innerNoC.Get(index);
                }
            }

            /// <summary>
            ///   根据表名获取字段集合
            /// </summary>
            /// <param name = "tableName">表名</param>
            /// <returns>字段集合</returns>
            public override sealed List<TableColumnCondition> Get(string tableName)
            {
                lock (syncRoot)
                {
                    return innerNoC.Get(tableName);
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
            ///   删除字段集合
            /// </summary>
            /// <param name = "tableName">字段集合</param>
            public override sealed void Remove(string tableName)
            {
                lock (syncRoot)
                {
                    innerNoC.Remove(tableName);
                }
            }

            #endregion
        }

        #endregion
    }
}