

#region Using Section

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO;

#endregion

namespace PD.Business
{
    /// <summary>
    /// 数据层组件
    /// </summary>
    public class DataTransaction
    {
        private static DataTransaction _tran;

        public static bool isRegister
        {
            get;
            set;
        }

        static DataTransaction()
        {
            //isRegister = true;
            //return;

            string path = @"C:\PDSystem.db";

            isRegister = false;

            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    StreamReader strwriter = new StreamReader(fs);
                    try
                    {
                        isRegister = strwriter.ReadLine().Equals("register");
                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        strwriter.Close();
                        strwriter = null;
                    }
                }
            }
        }

        public bool Exists(string strSql, params DbParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString, params DbParameter[] cmdParms)
        {
            object ds;
            using (var conn = CreateConnection())
            {
                try
                {
                    using (var dbCommand = conn.CreateCommand())
                    {
                        try
                        {
                            PrepareCommand(dbCommand, conn as MySqlConnection, null, SQLString, cmdParms);
                            ds = _db.ExecuteScalar(dbCommand);
                            return ds;
                        }
                        finally
                        {
                            dbCommand.Connection.Close();
                            dbCommand.Connection.Dispose();
                        }
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            throw new InvalidProgramException();
        }

        private void PrepareCommand(DbCommand cmd, DbConnection conn, SqlTransaction trans, string cmdText, DbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {

                foreach (DbParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        /// <summary>
        /// 创建数据连接，使用default Key
        /// </summary>
        /// <returns></returns>
        public static DataTransaction Create()
        {
            return Create("default");
        }
        /// <summary>
        /// 根据数据库连接的key，来创建数据连接
        /// </summary>
        /// <param name="vDBKey">数据库连接Key</param>
        /// <returns></returns>
        public static DataTransaction Create(string vDBKey)
        {
            if (isRegister)
            {
                var db = DatabaseFactory.CreateDatabase(vDBKey);
                _tran = new DataTransaction(db);
                return _tran;
            }
            throw new Exception("该软件没有授权，无法继续使用");
        }

        #region 字段
        /// <summary>
        /// 表字段集合
        /// </summary>
        public static TableColumnConditionCollections tabColumns = TableColumnConditionCollections.Instance();
        private readonly List<string> _sqlBufferList = new List<string>();
        private Database _db;
        #endregion

        #region 构造
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="db">数据库</param>
        public DataTransaction(Database db)
        {
            _db = db;

            IDbConnection dbConn = db.CreateConnection();
            SetConnectionMode(dbConn);
            dbConn.Dispose();
        }
        #endregion

        #region 属性
        /// <summary>
        ///   企业库5.0的数据库对象
        /// </summary>
        public Database Database
        {
            get
            {
                return _db;
            }
            set
            {
                _db = value;
            }
        }

        #region ConnectionMode

        #region ConnectionMode
        private ConnectionModes _ConnectionMode;//连接模式

        /// <summary>
        /// 连接模式
        /// </summary>
        public ConnectionModes ConnectionMode
        {
            get
            {
                return this._ConnectionMode;
            }
            set
            {
                this._ConnectionMode = value;
            }
        }
        #endregion

        #region SetConnectionMode
        /// <summary>
        /// 设置连接模式
        /// </summary>
        /// <param name="dbConn"></param>
        public void SetConnectionMode(IDbConnection dbConn)
        {
            if (dbConn.GetType().FullName.ToUpper().IndexOf("IBM.Data.DB2".ToUpper()) >= 0)
            {
                ConnectionMode = ConnectionModes.DB2;
            }
            else if (dbConn.GetType().FullName.ToUpper().IndexOf("System.Data.SqlClient".ToUpper()) >= 0)
            {
                ConnectionMode = ConnectionModes.MS_SQL_SERVER_2008;
            }
            else if (dbConn.GetType().FullName.ToUpper().IndexOf("System.Data.OleDb".ToUpper()) >= 0)
            {
                ConnectionMode = ConnectionModes.MS_ACCESS;
            }
            else
            {
                ConnectionMode = ConnectionModes.MICROSOFT_ORACLE9;
            }
        }
        #endregion

        #region ConnectionModeName
        public string ConnectionModeName
        {
            get
            {
                if (ConnectionMode == ConnectionModes.MICROSOFT_ORACLE || ConnectionMode == ConnectionModes.MICROSOFT_ORACLE9 ||
                    ConnectionMode == ConnectionModes.ORACLE || ConnectionMode == ConnectionModes.ORACLE9)
                {
                    return "Oracle";
                }
                else if (ConnectionMode == ConnectionModes.MS_SQL_SERVER_2000 || ConnectionMode == ConnectionModes.MS_SQL_SERVER_2005 ||
                         ConnectionMode == ConnectionModes.MS_SQL_SERVER_2008 || ConnectionMode == ConnectionModes.MS_SQL_SERVER_7)
                {
                    return "SQL Server";
                }
                else if (ConnectionMode == ConnectionModes.DB2)
                {
                    return "DB2";
                }
                else if (ConnectionMode == ConnectionModes.MS_ACCESS)
                {
                    return "Microsoft ACCESS";
                }
                else if (ConnectionMode == ConnectionModes.MYSQL)
                {
                    return "MySQL";
                }
                else if (ConnectionMode == ConnectionModes.ODBC)
                {
                    return "ODBC";
                }
                else
                {
                    return "Unknow";
                }
            }
        }
        #endregion

        #endregion

        #endregion

        #region 私有方法

        /// <summary>
        ///   增加sql命令到缓冲器
        /// </summary>
        /// <param name = "sql">sql</param>
        /// <returns>原sql</returns>
        private static string AddSQLBuffer(string sql)
        {
            //sqlBufferList.Add(sql);
            return sql;
        }

        /// <summary>
        ///   增加sql命令到缓冲器
        /// </summary>
        /// <returns>原sql集合</returns>
        private static string[] AddSQLBuffer(string[] sqls)
        {
            //sqlBufferList.AddRange(sqls);
            return sqls;
        }

        /// <summary>
        ///   从dataTable 内获取行记录转化为QueryGroup
        /// </summary>
        /// <param name = "dataTable">DataTable</param>
        /// <param name = "dr">行记录</param>
        /// <returns><see cref = "QueryGroupConditionCollections" /></returns>
        protected static QueryGroupConditionCollections GetKeyColumnToQueryGroup(DataTable dataTable, DataRow dr)
        {
            var queryGroup = new QueryGroupConditionCollections();
            var fields = new FieldConditionCollections();
            foreach (var dataCol in dataTable.PrimaryKey)
            {
                if (dr.RowState == DataRowState.Deleted)
                {
                    fields.Add(dataCol.ColumnName, OperatorType.Equal, dr[dataCol.ColumnName, DataRowVersion.Original]);
                }
                else
                {
                    if (dr[dataCol.ColumnName].GetType() == typeof(string))
                    {
                        if (!string.IsNullOrEmpty(dr[dataCol.ColumnName, DataRowVersion.Original].ToString()))
                            fields.Add(dataCol.ColumnName, OperatorType.Equal, dr[dataCol.ColumnName, DataRowVersion.Original]);
                        else if (!string.IsNullOrEmpty(dr[dataCol.ColumnName, DataRowVersion.Current].ToString()))
                            fields.Add(dataCol.ColumnName, OperatorType.Equal, dr[dataCol.ColumnName, DataRowVersion.Current]);
                        else if (!string.IsNullOrEmpty(dr[dataCol.ColumnName, DataRowVersion.Default].ToString()))
                            fields.Add(dataCol.ColumnName, OperatorType.Equal, dr[dataCol.ColumnName, DataRowVersion.Default]);
                    }
                    else
                    {
                        if (dr[dataCol.ColumnName, DataRowVersion.Original] != null)
                            fields.Add(dataCol.ColumnName, OperatorType.Equal, dr[dataCol.ColumnName, DataRowVersion.Original]);
                        else if (dr[dataCol.ColumnName, DataRowVersion.Current] != null)
                            fields.Add(dataCol.ColumnName, OperatorType.Equal, dr[dataCol.ColumnName, DataRowVersion.Current]);
                        else if (dr[dataCol.ColumnName, DataRowVersion.Default] != null)
                            fields.Add(dataCol.ColumnName, OperatorType.Equal, dr[dataCol.ColumnName, DataRowVersion.Default]);
                    }
                }
            }
            queryGroup.Add(fields);
            return queryGroup;
        }

        /// <summary>
        /// object to string
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        protected static string ObjectToString(object o)
        {
            return o + "";
        }

        /// <summary>
        ///   获取Int类型
        /// </summary>
        /// <param name = "objval">Object</param>
        /// <returns><see cref = "int" /></returns>
        protected static int GetDBInt32Value(object objval)
        {
            var val = ObjectToString(objval);
            if (string.IsNullOrEmpty(val.Trim()))
                return 0;
            return int.Parse(val);
        }

        /// <summary>
        ///   通过数据行创建<see cref = "TableColumnCondition" />
        /// </summary>
        /// <param name = "dr">数据行</param>
        /// <returns><see cref = "TableColumnCondition" /></returns>
        protected static TableColumnCondition DataRowToTableColumn(DataRow dr)
        {
            var tbCol = new TableColumnCondition
                            {
                                ColumnName = ObjectToString(dr["COLUMN_NAME"]).ToUpper(),
                                DataType = ObjectToString(dr["DATA_TYPE"]),
                                Comments = ObjectToString(dr["COMMENTS"]),
                                DataLength = GetDBInt32Value(dr["DATA_LENGTH"]),
                                DataScale = GetDBInt32Value(dr["DATA_SCALE"]),
                                Nullable = (ObjectToString(dr["NULLABLE"]) == "Y")
                            };
            return tbCol;
        }

        /// <summary>
        ///   根据表名称获取物理表的结构并放入Cache
        /// </summary>
        /// <param name = "tableName">表名称</param>
        public void InitTableColumnsList(string tableName)
        {
            tableName = tableName.Replace("[", "");
            tableName = tableName.Replace("]", "");
            Console.WriteLine(tabColumns.Count);
            if (tabColumns.ContainsKey(tableName))
                return;
            string sql = "";
            if (ConnectionMode == ConnectionModes.MICROSOFT_ORACLE || ConnectionMode == ConnectionModes.MICROSOFT_ORACLE9 ||
                ConnectionMode == ConnectionModes.ORACLE || ConnectionMode == ConnectionModes.ORACLE9)
            {
                sql = "select T.TABLE_NAME,\n" +
                "       T.COLUMN_NAME,\n" +
                "       T.DATA_TYPE,\n" +
                "       V.COMMENTS,\n" +
                "       T.DATA_LENGTH,\n" +
                "       T.DATA_SCALE,\n" +
                "       T.NULLABLE\n" +
                "  from sys.all_tab_cols t, SYS.ALL_COL_COMMENTS V\n" +
                " WHERE T.OWNER = (SELECT SYS_CONTEXT('USERENV', 'SESSION_USER') FROM DUAL)\n" +
                "   AND V.OWNER = (SELECT SYS_CONTEXT('USERENV', 'SESSION_USER') FROM DUAL)\n" +
                "   AND T.TABLE_NAME = V.TABLE_NAME\n" +
                "   AND T.OWNER = V.OWNER\n" +
                "   AND T.TABLE_NAME = '" + tableName.ToUpper() +
                "'  AND T.COLUMN_NAME = V.COLUMN_NAME\n" +
                " ORDER BY T.TABLE_NAME, T.COLUMN_ID";
            }
            else
            {
                sql = @"select t2.name TABLE_NAME,t1.name COLUMN_NAME, t3.name DATA_TYPE,t1.max_length DATA_LENGTH,t1.scale DATA_SCALE,
                    case t1.is_nullable when 1 then 'Y' else 'N' end NULLABLE,t1.column_id,
                    (select count(*) from sys.identity_columns a1
                     where a1.object_id= t1.object_id and t1.column_id = a1.column_id) as IS_IDENTITY,
                    t5.value COMMENTS,case t9.is_primary_key when 1 then 1 else 0 end PRIMARY_KEY
                    from sys.columns t1
                    left join sys.tables t2 on t1.object_id= t2.object_id
                    left join sys.types t3 on t1.system_type_id=t3.system_type_id
                    left join sys.extended_properties t5 on t5.major_id = t1.object_id and t5.minor_id = t1.column_id
                    left JOIN sys.objects t7 on t1.object_id=t7.object_id
                    left join sys.index_columns t8 on t1.object_id=t8.object_id and t1.column_id=t8.column_id
                    left join sys.indexes t9 on t8.object_id=t9.object_id 
                    where upper(t2.name)='" + tableName.ToUpper() + "'" +
                    "order by t1.column_id";
            }

            var colTab = DoGetDataTable(sql);

            if (colTab != null)
            {
                foreach (DataRow dr in colTab.Rows)
                {
                    //检查是否存在
                    if (tabColumns.ContainsKey(ObjectToString(dr["TABLE_NAME"]).ToUpper()))
                    {
                        var dataRow = dr;
                        var findCol =
                            tabColumns[ObjectToString(dr["TABLE_NAME"]).ToUpper()].Any(
                                col =>
                                col.ColumnName.Equals(ObjectToString(dataRow["COLUMN_NAME"]).ToUpper(),
                                                      StringComparison.CurrentCultureIgnoreCase));
                        //未找到列
                        if (!findCol)
                            tabColumns[ObjectToString(dr["TABLE_NAME"]).ToUpper()].Add(
                                DataRowToTableColumn(dr));
                    }
                    else
                    {
                        //增加到集合
                        var tabColList = new List<TableColumnCondition> { DataRowToTableColumn(dr) };
                        tabColumns.Add(ObjectToString(dr["TABLE_NAME"]).ToUpper(), tabColList);
                    }
                }
            }
            else
            {
                Console.WriteLine("警告: InitTableColumnsList 无法获取oracle 数据库表的结构！");
                throw new InvalidOperationException("警告: InitTableColumnsList 无法获取oracle 数据库表的结构！");
            }
        }

        /// <summary>
        /// 解析Update命令定义集合
        /// </summary>
        /// <param name="upcmd">更新命令结构</param>
        /// <returns>返回解析后的sql集合</returns>
        public static List<string> __ParseUpdateDataTable(UpdateCommandCondition upcmd, ConnectionModes cnnmode)
        {
            var sqlText = new List<string>();
            var upFields = new UpdateFieldConditionCollections();
            switch (upcmd.UpdateCommandType)
            {
                case UpdateCommandType.Insert:
                    //组合insert sql
                    var insertData = upcmd.DataSource.GetChanges(DataRowState.Added);
                    if (insertData != null)
                        foreach (DataRow dr in insertData.Rows)
                        {
                            upFields.Clear();
                            foreach (var tbCol in tabColumns[upcmd.DataSource.TableName.ToUpper()])
                                upFields.Add(tbCol.ColumnName, dr[tbCol.ColumnName]);
                            sqlText.Add(__ParseInsertSQL(upcmd.DataSource.TableName.ToUpper(), upFields, cnnmode));
                        }
                    break;
                case UpdateCommandType.Update:
                    //组合update sql
                    var updateData = upcmd.DataSource.GetChanges(DataRowState.Modified);
                    if (updateData != null)
                        foreach (DataRow dr in updateData.Rows)
                        {
                            upFields.Clear();
                            foreach (var tbCol in tabColumns[upcmd.DataSource.TableName.ToUpper()])
                                upFields.Add(tbCol.ColumnName, dr[tbCol.ColumnName]);
                            //获取主键
                            sqlText.Add(
                                __ParseUpdateSQL(upcmd.DataSource.TableName.ToUpper(), upFields,
                                                 GetKeyColumnToQueryGroup(upcmd.DataSource, dr), cnnmode));
                        }
                    break;
                case UpdateCommandType.Delete:
                    //组合delete sql
                    var deleteData = upcmd.DataSource.GetChanges(DataRowState.Deleted);
                    if (deleteData != null)
                    {
                        deleteData.RejectChanges();
                        sqlText.AddRange(from DataRow dr in deleteData.Rows
                                         select
                                             __ParseDeleteSQL(upcmd.DataSource.TableName,
                                                              GetKeyColumnToQueryGroup(upcmd.DataSource, dr), cnnmode));
                    }
                    break;
                default:
                    sqlText.Clear();
                    var allData = upcmd.DataSource.GetChanges();
                    if (allData != null)
                        foreach (DataRow dr in allData.Rows)
                        {
                            switch (dr.RowState)
                            {
                                case DataRowState.Added:
                                    upFields.Clear();
                                    foreach (var tbCol in tabColumns[upcmd.DataSource.TableName.ToUpper()])
                                        upFields.Add(tbCol.ColumnName, dr[tbCol.ColumnName]);
                                    sqlText.Add(__ParseInsertSQL(upcmd.DataSource.TableName.ToUpper(), upFields, cnnmode));
                                    break;
                                case DataRowState.Deleted:
                                    sqlText.Add(__ParseDeleteSQL(upcmd.DataSource.TableName,
                                                                 GetKeyColumnToQueryGroup(upcmd.DataSource, dr), cnnmode));
                                    break;
                                case DataRowState.Modified:
                                    upFields.Clear();
                                    foreach (var tbCol in tabColumns[upcmd.DataSource.TableName.ToUpper()])
                                        upFields.Add(tbCol.ColumnName, dr[tbCol.ColumnName]);
                                    //获取主键
                                    sqlText.Add(
                                        __ParseUpdateSQL(upcmd.DataSource.TableName.ToUpper(), upFields,
                                                         GetKeyColumnToQueryGroup(upcmd.DataSource, dr), cnnmode));
                                    break;
                            }
                        }
                    break;
            }
            return sqlText;
        }
        /// <summary>
        /// 解析Sql
        /// </summary>
        /// <param name = "tableName">物理表名称</param>
        /// <param name = "queryGroup">查询组</param>
        /// <param name = "orderBy">排序组</param>
        /// <returns>返回解析后的sql</returns>
        public static string __ParseSelectSQL(string tableName, QueryGroupConditionCollections queryGroup,
                                              OrderCollections orderBy, ConnectionModes cnnmode)
        {
            var sqlBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(tableName))
                throw new Exception("无效的表名称！");

            sqlBuilder.AppendFormat("select * from {0} ", tableName);
            if (queryGroup != null && queryGroup.Count > 0)
            {
                var queryCount = queryGroup.Sum(qry => qry.FieldConditions.Count);
                if (queryCount > 0)
                {
                    sqlBuilder.AppendFormat(" where ");
                    for (var i = 0; i < queryGroup.Count; i++)
                    {
                        var qry = queryGroup[i];
                        sqlBuilder.AppendFormat(" (");
                        for (var j = 0; j < qry.FieldConditions.Count; j++)
                        {
                            var field = qry.FieldConditions[j];

                            //支持sql 拼接
                            if (field.Operator == OperatorType.SqlText)
                                sqlBuilder.AppendFormat(" {0} ", field.Value);
                            else
                                sqlBuilder.AppendFormat(" {0} {1} {2} ", field.FieldName,
                                                        GetOperatorType(field.Operator),
                                                        FormatSQLValue(field.Value, cnnmode));
                            if (j < qry.FieldConditions.Count - 1)
                                sqlBuilder.AppendFormat(" {0} ", GetWhereUnionType(field.NextUnionType));
                        }
                        sqlBuilder.AppendFormat(") ");
                        if (i < (queryGroup.Count - 1))
                            sqlBuilder.AppendFormat(" {0} ", GetWhereUnionType(qry.NextUnionType));
                    }
                }
            }
            if (orderBy != null && orderBy.Count > 0)
            {
                sqlBuilder.Append(" order by ");
                for (var n = 0; n < orderBy.FieldNames.Length; n++)
                {
                    var fieldName = orderBy.FieldNames[n];
                    sqlBuilder.AppendFormat(" {0} {1} ", fieldName, GetOrderByType(orderBy[fieldName]));
                    if (n < (orderBy.FieldNames.Length - 1))
                        sqlBuilder.Append(",");
                }
            }
            Console.WriteLine("create sql:" + sqlBuilder);
            return sqlBuilder.ToString();
        }
        /// <summary>
        /// 解析insert
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="updateFields">更新字段结构集合</param>
        /// <returns>返回解析后的sql</returns>
        public static string __ParseInsertSQL(string tableName, UpdateFieldConditionCollections updateFields, ConnectionModes cnnmode)
        {
            var sqlBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(tableName))
                throw new Exception("无效的表名称！");

            sqlBuilder.AppendFormat("Insert Into {0} ", tableName);
            if (updateFields == null || updateFields.Count < 1)
                throw new Exception("无效的更新字段参数!");
            sqlBuilder.Append("(");
            for (var n = 0; n < updateFields.Count; n++)
            {
                var upField = updateFields[n];
                sqlBuilder.AppendFormat(" {0} ", upField.FieldName);
                if (n < updateFields.Count - 1)
                    sqlBuilder.Append(" , ");
            }
            sqlBuilder.Append(")values(");
            for (var n = 0; n < updateFields.Count; n++)
            {
                var upField = updateFields[n];
                sqlBuilder.AppendFormat(" {0} ", FormatSQLValue(upField.Value, cnnmode));
                if (n < updateFields.Count - 1)
                    sqlBuilder.Append(" , ");
            }
            sqlBuilder.Append(")");
            Console.WriteLine("create sql:" + sqlBuilder);
            return sqlBuilder.ToString();
        }

        /// <summary>
        ///   解析update
        /// </summary>
        /// <param name = "tableName">物理表名</param>
        /// <param name = "updateFields">更新字段</param>
        /// <param name = "queryGroup">查询集合</param>
        /// <returns>解析后的sql</returns>
        public static string __ParseUpdateSQL(string tableName, UpdateFieldConditionCollections updateFields,
                                             QueryGroupConditionCollections queryGroup, ConnectionModes cnnmode)
        {
            var sqlBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(tableName))
                throw new Exception("无效的表名称！");

            sqlBuilder.AppendFormat("Update {0} ", tableName);
            if (updateFields == null || updateFields.Count < 1)
                throw new Exception("无效的更新字段参数!");
            sqlBuilder.Append(" set ");
            for (var n = 0; n < updateFields.Count; n++)
            {
                var upField = updateFields[n];
                sqlBuilder.AppendFormat(" {0}{1}{2} ", upField.FieldName, GetOperatorType(upField.Operator),
                                        FormatSQLValue(upField.Value, cnnmode));
                if (n < updateFields.Count - 1)
                    sqlBuilder.AppendFormat(" , ");
            }
            if (queryGroup != null && queryGroup.Count > 0)
            {
                sqlBuilder.AppendFormat(" where ");
                for (var i = 0; i < queryGroup.Count; i++)
                {
                    var qry = queryGroup[i];
                    sqlBuilder.AppendFormat(" (");
                    for (var j = 0; j < qry.FieldConditions.Count; j++)
                    {
                        var field = qry.FieldConditions[j];
                        sqlBuilder.AppendFormat("( {0} {1} {2} )", field.FieldName, GetOperatorType(field.Operator),
                                                FormatSQLValue(field.Value, cnnmode));
                        if (j < qry.FieldConditions.Count - 1)
                            sqlBuilder.AppendFormat(" {0} ", GetWhereUnionType(field.NextUnionType));
                    }
                    sqlBuilder.AppendFormat(") ");
                    if (i < (queryGroup.Count - 1))
                        sqlBuilder.AppendFormat(" {0} ", GetWhereUnionType(qry.NextUnionType));
                }
            }
            Console.WriteLine("create sql:" + sqlBuilder);
            return sqlBuilder.ToString();
        }

        /// <summary>
        ///   解析delete
        /// </summary>
        /// <param name = "tableName">物理表名</param>
        /// <param name = "queryGroup">查询组</param>
        /// <returns>返回解析后的sql</returns>
        public static string __ParseDeleteSQL(string tableName, QueryGroupConditionCollections queryGroup, ConnectionModes cnnmode)
        {
            var sqlBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(tableName))
                throw new Exception("无效的表名称！");

            sqlBuilder.AppendFormat("delete from {0} ", tableName);
            if (queryGroup != null && queryGroup.Count > 0)
            {
                sqlBuilder.AppendFormat(" where ");
                for (var i = 0; i < queryGroup.Count; i++)
                {
                    var qry = queryGroup[i];
                    sqlBuilder.AppendFormat(" (");
                    for (var j = 0; j < qry.FieldConditions.Count; j++)
                    {
                        var field = qry.FieldConditions[j];
                        sqlBuilder.AppendFormat("( {0} {1} {2} )", field.FieldName, GetOperatorType(field.Operator),
                                                FormatSQLValue(field.Value, cnnmode));
                        if (j < qry.FieldConditions.Count - 1)
                            sqlBuilder.AppendFormat(" {0} ", GetWhereUnionType(field.NextUnionType));
                    }
                    sqlBuilder.AppendFormat(") ");
                    if (i < (queryGroup.Count - 1))
                        sqlBuilder.AppendFormat(" {0} ", GetWhereUnionType(qry.NextUnionType));
                }
            }
            Console.WriteLine("create sql:" + sqlBuilder);
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 格式化sql值
        /// </summary>
        /// <param name = "someValue">原始值</param>
        /// <returns>字符串形式</returns>
        public static string FormatSQLValue(object someValue, ConnectionModes cnnmode)
        {
            string formattedValue;
            if (someValue == null)
            {
                formattedValue = "NULL";
            }
            else
            {
                if (someValue is Array)
                {
                    formattedValue = (someValue as Array).Cast<object>().Aggregate(" (",
                                                                                   (current, sv) =>
                                                                                   current + (FormatSQLValue(sv, cnnmode) + ","));
                    return formattedValue.TrimEnd(',') + ") ";
                }
                switch (someValue.GetType().Name)
                {
                    case "String":
                        formattedValue = "'" + ((string)someValue).Replace("'", "''") + "'";
                        break;
                    case "DateTime":

                        if (cnnmode == ConnectionModes.MICROSOFT_ORACLE || cnnmode == ConnectionModes.MICROSOFT_ORACLE9 ||
               cnnmode == ConnectionModes.ORACLE || cnnmode == ConnectionModes.ORACLE9)
                        {
                            if (((DateTime)someValue).ToString("HH:mm:ss").Equals("00:00:00"))
                                formattedValue = "to_date('" + ((DateTime)someValue).ToString("yyyy-MM-dd") +
                                                 "','yyyy-MM-dd')";
                            else
                                formattedValue = "to_date('" + ((DateTime)someValue).ToString("yyyy-MM-dd HH:mm:ss") +
                                                 "','yyyy-MM-dd hh24:mi:ss')";
                        }
                        else
                        {
                            // for sql server

                            if (((DateTime)someValue).ToString("HH:mm:ss").Equals("00:00:00"))
                                formattedValue = "cast('" + ((DateTime)someValue).ToString("yyyy-MM-dd") +
                                                 "' as Date)";
                            else
                                formattedValue = "cast('" + ((DateTime)someValue).ToString("yyyy-MM-dd HH:mm:ss") +
                                                 "' as datetime)";
                        }

                        break;
                    case "DBNull":
                        formattedValue = "NULL";
                        break;
                    case "Boolean":
                        formattedValue = (bool)someValue ? "1" : "0";
                        break;
                    default:
                        formattedValue = someValue.ToString();
                        break;
                }
            }
            return formattedValue;
        }

        /// <summary>
        /// 获取查询条件逻辑符号
        /// </summary>
        /// <param name = "wut">逻辑类型</param>
        /// <returns>符号</returns>
        public static string GetWhereUnionType(WhereUnionType wut)
        {
            return wut == WhereUnionType.And ? "AND" : "OR";
        }

        /// <summary>
        ///   获取排序逻辑
        /// </summary>
        /// <param name = "obt">逻辑类型</param>
        /// <returns>符号</returns>
        public static string GetOrderByType(OrderByType obt)
        {
            return obt == OrderByType.Asc ? "ASC" : "DESC";
        }

        /// <summary>
        ///   获取操作符
        /// </summary>
        /// <param name = "op">操作符</param>
        /// <returns>操作符</returns>
        public static string GetOperatorType(OperatorType op)
        {
            var ret = string.Empty;

            switch (op)
            {
                case OperatorType.Equal:
                    ret = "=";
                    break;
                case OperatorType.Unequal:
                    ret = "<>";
                    break;
                case OperatorType.Greater:
                    ret = ">";
                    break;
                case OperatorType.GreaterAndEqual:
                    ret = ">=";
                    break;
                case OperatorType.LessThan:
                    ret = "<";
                    break;
                case OperatorType.LessThanAndEqual:
                    ret = "<=";
                    break;
                case OperatorType.Like:
                    ret = "like";
                    break;
                case OperatorType.NotLike:
                    ret = "not like";
                    break;
                case OperatorType.Is:
                    ret = "is";
                    break;
                case OperatorType.IsNot:
                    ret = "is not";
                    break;
                case OperatorType.In:
                    ret = "in";
                    break;
                case OperatorType.NotIn:
                    ret = "not in";
                    break;
            }

            return ret;
        }

        #endregion

        #region 执行查询或sql或存储过程

        #region DoGetDataTable
        /// <summary>
        /// 执行sql返回DataTable
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回DataTable</returns>
        public DataTable DoGetDataTable(string sql)
        {
            DataSet ds;
            using (var conn = CreateConnection())
            {
                try
                {
                    //conn.Open();
                    using (var dbCommand = conn.CreateCommand())
                    {
                        try
                        {
                            dbCommand.CommandText = sql;
                            dbCommand.CommandType = CommandType.Text;
                            ds = _db.ExecuteDataSet(dbCommand);
                            if (ds != null && ds.Tables.Count > 0)
                                return ds.Tables[0];
                        }
                        finally
                        {
                            dbCommand.Connection.Close();
                            dbCommand.Connection.Dispose();
                        }
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            throw new InvalidProgramException();
        }
        #endregion

        #region GetDataTableByStoredProcedure
        /// <summary>
        /// 执行存储过程返回DataTable(只支持一个返回游标)
        /// </summary>
        /// <param name = "storedProcedureName">存储过程名称</param>
        /// <param name = "parameterValues">参数集合</param>
        /// <returns>返回DataTable</returns>
        public DataTable GetDataTableByStoredProcedure(string storedProcedureName, object[] parameterValues)
        {
            DataSet ds;
            //using (var conn = CreateConnection())
            {
                try
                {
                    //conn.Open();
                    ds = _db.ExecuteDataSet(storedProcedureName, parameterValues);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        return ds.Tables[0];
                    }
                }
                finally
                {
                    //conn.Close();
                    //conn.Dispose();
                }
            }
            throw new InvalidProgramException();
        }
        #endregion

        #region ExecuteSql
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        public int ExecuteSql(string sqlText)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    conn.Open();
                    using (var dbCommand = conn.CreateCommand())
                    {
                        dbCommand.CommandText = sqlText;
                        dbCommand.CommandType = CommandType.Text;
                        return dbCommand.ExecuteNonQuery();
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        #endregion

        #region ExecuteSql
        /// <summary>
        /// 执行数据库脚本
        /// </summary>
        /// <param name="sqlText">带参数的数据库脚本</param>
        public void ExecuteSql(SqlParamterItem sql)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    conn.Open();
                    using (var dbCommand = conn.CreateCommand())
                    {
                        dbCommand.CommandText = sql.Sql;
                        sql.ParamterCollection.ForEach(item =>
                        {
                            _db.AddInParameter(dbCommand, item.ParameterName, item.DbType, item.Value);
                        });
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.ExecuteNonQuery();
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }


        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string SQLString, params DbParameter[] cmdParms)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    conn.Open();
                    using (var dbCommand = conn.CreateCommand())
                    {
                        PrepareCommand(dbCommand, conn, null, SQLString, cmdParms);
                        return _db.ExecuteDataSet(dbCommand);
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public int ExecuteSql(string sql, DbParameter[] parameter)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    conn.Open();
                    using (var dbCommand = conn.CreateCommand())
                    {
                        dbCommand.CommandText = sql;
                        parameter.ToList().ForEach(item =>
                        {
                            _db.AddInParameter(dbCommand, item.ParameterName, item.DbType, item.Value);
                        });
                        dbCommand.CommandType = CommandType.Text;
                        return dbCommand.ExecuteNonQuery();
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        #endregion

        #region ExecuteSqlForCmd
        /// <summary>
        /// 执行sqlFor DbCommand
        /// </summary>
        /// <param name="cmd">DbCommand</param>
        public void ExecuteSqlForCmd(DbCommand cmd)
        {
            //using (var conn = CreateConnection())
            {
                try
                {
                    //conn.Open();
                    if (cmd.Transaction != null)
                    {
                        _db.ExecuteNonQuery(cmd, cmd.Transaction);
                    }
                    else
                    {
                        _db.ExecuteNonQuery(cmd);
                    }
                }
                finally
                {
                    //conn.Close();
                    //conn.Dispose();
                }
            }
        }
        #endregion

        #region ExecuteSqlForParameterInfo
        /// <summary>
        /// 执行sqlFor参数信息(根据参数信息sql,更新行为默认为标准)
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="ParameterInfoList">参数信息集合(key：数据行号;value：参数集合)</param>
        public void ExecuteSqlForParameterInfo(string sql, Dictionary<int, List<ParameterInfo>> ParameterInfoList)
        {
            ExecuteSqlForParameterInfo(sql, ParameterInfoList, DataUpdateBehavior.Standard);
        }
        #endregion

        #region ExecuteSqlForParameterInfo
        /// <summary>
        /// 执行sqlFor参数信息(根据参数信息和更新行为执行sql)
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="ParameterInfoList">参数信息集合(key：数据行号;value：参数集合)</param>
        /// <param name="updateBehavior">更新模式</param>
        public void ExecuteSqlForParameterInfo(string sql, Dictionary<int, List<ParameterInfo>> ParameterInfoList, DataUpdateBehavior updateBehavior)
        {
            switch (updateBehavior)
            {
                case DataUpdateBehavior.Standard:
                    DbCommand cmd = GetSqlStringCommand(sql);
                    foreach (var i in ParameterInfoList)
                    {
                        cmd.Parameters.Clear();
                        foreach (var p in i.Value)
                            _db.AddInParameter(cmd, p.ParameterName, p.DbType, p.Value);
                        _db.ExecuteNonQuery(cmd);

                    }
                    if (cmd.Connection != null)
                    {
                        cmd.Connection.Close();
                        cmd.Connection.Dispose();
                    }
                    break;
                case DataUpdateBehavior.Continue:
                    Exception ex = null;
                    DbCommand cmd1 = GetSqlStringCommand(sql);
                    foreach (var i in ParameterInfoList)
                    {
                        try
                        {
                            cmd1.Parameters.Clear();
                            foreach (var p in i.Value)
                                _db.AddInParameter(cmd1, p.ParameterName, p.DbType, p.Value);
                            _db.ExecuteNonQuery(cmd1);
                        }
                        catch (Exception execEx)
                        {
                            ex = execEx;
                        }
                        if (ex != null)
                            throw ex;
                    }
                    if (cmd1.Connection != null)
                    {
                        cmd1.Connection.Close();
                        cmd1.Connection.Dispose();
                    }
                    break;
                case DataUpdateBehavior.Transactional:
                    using (var conn = CreateConnection())
                    {
                        conn.Open();
                        DbTransaction dbTran = null;
                        try
                        {
                            dbTran = conn.BeginTransaction();
                            using (var dbCommand = conn.CreateCommand())
                            {
                                dbCommand.Transaction = dbTran;
                                dbCommand.CommandType = CommandType.Text;
                                dbCommand.CommandText = sql;
                                foreach (var i in ParameterInfoList)
                                {
                                    dbCommand.Parameters.Clear();
                                    foreach (var p in i.Value)
                                        _db.AddInParameter(dbCommand, p.ParameterName, p.DbType, p.Value);
                                    dbCommand.ExecuteNonQuery();
                                }
                            }
                            dbTran.Commit();
                        }
                        catch (Exception)
                        {
                            if (dbTran != null)
                                dbTran.Rollback();
                            throw;
                        }
                        finally
                        {
                            conn.Close();
                            conn.Dispose();
                        }
                    }
                    break;
            }
        }
        #endregion

        #region ExecuteStoredProcedure
        /// <summary>
        /// 执行存储过程不返回结果
        /// </summary>
        /// <param name = "storedProcedureName">存储过程名称</param>
        /// <param name = "parameterValues">参数值集合,参数必须实现IConvertible接口，集合按存储过程参数顺序赋值</param>
        public void ExecuteStoredProcedure(string storedProcedureName, object[] parameterValues)
        {
            //using (var conn=CreateConnection())
            {
                try
                {
                    //conn.Open();
                    _db.ExecuteNonQuery(storedProcedureName, parameterValues);
                }
                finally
                {
                    //conn.Close();
                }
            }
        }
        #endregion

        #region GetSqlStringCommand
        /// <summary>
        /// 获得sql的cmd
        /// </summary>
        /// <param name = "sql">sql语句</param>
        /// <returns>返回DbCommand</returns>
        public DbCommand GetSqlStringCommand(string sql)
        {
            //using (var conn=CreateConnection())
            {
                try
                {
                    //conn.Open();
                    return _db.GetSqlStringCommand(sql);
                }
                finally
                {
                    //conn.Open();
                }
            }
        }
        #endregion

        #region GetStoredProcCommand
        /// <summary>
        /// 获得存储过程的cmd
        /// </summary>
        /// <param name = "storedProcedureName">存储过程名称</param>
        /// <param name = "parameterValues">参数集合</param>
        /// <returns>返回DbCommand</returns>
        public DbCommand GetStoredProcCommand(string storedProcedureName, object[] parameterValues)
        {
            //using (var conn = CreateConnection())
            {
                try
                {
                    //conn.Open();
                    return _db.GetStoredProcCommand(storedProcedureName, parameterValues);
                }
                finally
                {
                    //conn.Close();
                    //conn.Dispose();
                }
            }
        }
        #endregion

        #region GetStoredProcCommand
        /// <summary>
        /// 获得存储过程的cmd
        /// </summary>
        /// <param name="storedProcedureName">存储过程名</param>
        /// <returns></returns>
        public DbCommand GetStoredProcCommand(string storedProcedureName)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    //conn.Open();
                    return _db.GetStoredProcCommand(storedProcedureName);
                }
                finally
                {
                    //conn.Close();
                    //conn.Dispose();
                }
            }
        }
        #endregion

        #region ExecuteStoredProcedure
        /// <summary>
        /// 根据cmd来执行存储过程
        /// </summary>
        /// <param name="cmd">DbCommand</param>
        public void ExecuteStoredProcedure(DbCommand cmd)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    //conn.Open();
                    _db.ExecuteNonQuery(cmd);
                }
                finally
                {
                    //conn.Close();
                    //conn.Dispose();
                }
            }
        }
        #endregion

        #region ExecuteDataSet
        /// <summary>
        /// 执行cmd，将结果集填充到DataSet中
        /// </summary>
        /// <param name="cmd">commond</param>
        /// <param name="result">返回的结果集</param>
        public void ExecuteDataSet(DbCommand cmd, out DataSet result)
        {
            //using (var conn = CreateConnection())
            {
                bool vHasConn = cmd.Connection != null;
                try
                {
                    //conn.Open();
                    result = _db.ExecuteDataSet(cmd);
                }
                finally
                {
                    if (vHasConn)
                    {
                        cmd.Connection.Close();
                        cmd.Connection.Dispose();
                    }
                    //conn.Close();
                    //conn.Dispose();
                }
            }
        }
        #endregion

        #region GetParameterValue
        /// <summary>
        /// 根据参数名返回cmd中的参数值
        /// </summary>
        /// <param name="cmd">DbCommand</param>
        /// <param name="paraName">参数名称</param>
        /// <returns>返回参数值</returns>
        public object GetParameterValue(DbCommand cmd, string paraName)
        {
            return cmd.Parameters[paraName].Value;
        }
        #endregion

        #region AddInParameter
        /// <summary>
        /// 添加传入参数到DbCommand
        /// </summary>
        /// <param name="cmd">DbCommand</param>
        /// <param name="paraName">输入参数名称</param>
        /// <param name="dbtype">输入参数类型</param>
        /// <param name="paraValue">输入参数值</param>
        public void AddInParameter(DbCommand cmd, string paraName, DbType dbtype, object paraValue)
        {
            _db.AddInParameter(cmd, paraName, dbtype, paraValue);
        }
        #endregion

        #region AddOutParameter
        /// <summary>
        /// 添加输出参数到DbCommand(非游标类)
        /// </summary>
        /// <param name="cmd">DbCommand</param>
        /// <param name="outParaName">输出参数名称</param>
        /// <param name="dbtype">输出参数类型</param>
        /// <param name="size">输出参数的最大尺寸(一般填写为:1000)</param>
        public void AddOutParameter(DbCommand cmd, string outParaName, DbType dbtype, Int32 size)
        {
            _db.AddOutParameter(cmd, outParaName, dbtype, size);
        }
        #endregion

        #region AddOracleOutParameter
        /// <summary>
        /// 添加oracle输出参数(只针对输出参数为Oracle的游标类(Cursor)参数时)
        /// </summary>
        /// <param name="cmd">DbCommand</param>
        /// <param name="paraName">输出的游标参数名称</param>
        /// <param name="oracleType">Oracle数据类型(Cursor)</param>
        public void AddOracleOutParameter(DbCommand cmd, string paraName, DataOracleType oracleType)
        {
            var oraPara = new OracleParameter(paraName, DataOracleTypeChange(oracleType))
            {
                Direction = ParameterDirection.Output
            };
            //指明为输出参数
            cmd.Parameters.Add(oraPara);
        }
        #endregion

        #region DataOracleTypeChange
        /// <summary>
        /// Datas the oracle type change.
        /// </summary>
        /// <param name="oracleType">Type of the oracle.</param>
        /// <returns></returns>
        private static OracleType DataOracleTypeChange(DataOracleType oracleType)
        {
            switch (oracleType)
            {
                case DataOracleType.BFile:
                    return OracleType.BFile;
                case DataOracleType.Blob:
                    return OracleType.Blob;
                case DataOracleType.Byte:
                    return OracleType.Byte;
                case DataOracleType.Char:
                    return OracleType.Char;
                case DataOracleType.Clob:
                    return OracleType.Clob;
                case DataOracleType.Cursor:
                    return OracleType.Cursor;
                case DataOracleType.DateTime:
                    return OracleType.DateTime;
                case DataOracleType.Double:
                    return OracleType.Double;
                case DataOracleType.Float:
                    return OracleType.Float;
                case DataOracleType.Int16:
                    return OracleType.Int16;
                case DataOracleType.Int32:
                    return OracleType.Int32;
                case DataOracleType.IntervalDayToSecond:
                    return OracleType.IntervalDayToSecond;
                case DataOracleType.IntervalYearToMonth:
                    return OracleType.IntervalYearToMonth;
                case DataOracleType.LongRaw:
                    return OracleType.LongRaw;
                case DataOracleType.LongVarChar:
                    return OracleType.LongVarChar;
                case DataOracleType.NChar:
                    return OracleType.NChar;
                case DataOracleType.NClob:
                    return OracleType.NClob;
                case DataOracleType.Number:
                    return OracleType.Number;
                case DataOracleType.NVarChar:
                    return OracleType.NVarChar;
                case DataOracleType.Raw:
                    return OracleType.Raw;
                case DataOracleType.RowId:
                    return OracleType.RowId;
                case DataOracleType.SByte:
                    return OracleType.SByte;
                case DataOracleType.Timestamp:
                    return OracleType.Timestamp;
                case DataOracleType.TimestampLocal:
                    return OracleType.TimestampLocal;
                case DataOracleType.TimestampWithTZ:
                    return OracleType.TimestampWithTZ;
                case DataOracleType.UInt16:
                    return OracleType.UInt16;
                case DataOracleType.UInt32:
                    return OracleType.UInt32;
                default:
                    return OracleType.VarChar;
            }
        }
        #endregion

        #region ExecuteMultiSql
        /// <summary>
        /// 执行多个sql(默认更新模式为Standard)
        /// </summary>
        /// <param name="sqlTextCollection">sql集合</param>
        public void ExecuteMultiSql(params string[] sqlTextCollection)
        {
            ExecuteMultiSql(DataUpdateBehavior.Standard, sqlTextCollection);
        }
        #endregion

        #region ExecuteMultiSql
        /// <summary>
        /// 根据更新模式，执行多个sql
        /// </summary>
        /// <param name="updateBehavior">更新模式</param>
        /// <param name="sqlTextCollection">sql集合</param>
        public void ExecuteMultiSql(DataUpdateBehavior updateBehavior, params string[] sqlTextCollection)
        {
            switch (updateBehavior)
            {
                case DataUpdateBehavior.Standard:
                    foreach (var sql in sqlTextCollection)
                        ExecuteSql(sql);
                    break;
                case DataUpdateBehavior.Continue:
                    Exception ex = null;
                    foreach (var sql in sqlTextCollection)
                        try
                        {
                            ExecuteSql(sql);
                        }
                        catch (Exception execEx)
                        {
                            ex = execEx;
                        }
                    if (ex != null)
                        throw ex;
                    break;
                case DataUpdateBehavior.Transactional:
                    using (var conn = CreateConnection())
                    {
                        conn.Open();
                        DbTransaction dbTran = null;
                        try
                        {
                            dbTran = conn.BeginTransaction();
                            using (var dbCommand = conn.CreateCommand())
                            {
                                dbCommand.Transaction = dbTran;
                                dbCommand.CommandType = CommandType.Text;
                                foreach (var sql in sqlTextCollection)
                                {
                                    dbCommand.CommandText = sql;
                                    dbCommand.ExecuteNonQuery();
                                }
                            }
                            dbTran.Commit();
                        }
                        catch (Exception)
                        {
                            if (dbTran != null)
                                dbTran.Rollback();
                            throw;
                        }
                        finally
                        {
                            conn.Close();
                            conn.Dispose();
                        }
                    }
                    break;
            }
        }
        #endregion

        #region ExecuteMultiSql
        /// <summary>
        /// 根据更新模式，执行多个sql
        /// </summary>
        /// <param name="updateBehavior">执行模式</param>
        /// <param name="scriptCollection">带参数的数据库脚本执行对象集合</param>
        public void ExecuteMultiSql(DataUpdateBehavior updateBehavior, List<SqlParamterItem> scriptCollection)
        {
            switch (updateBehavior)
            {
                case DataUpdateBehavior.Standard:
                    foreach (var sql in scriptCollection)
                    {
                        ExecuteSql(sql);
                    }
                    break;
                case DataUpdateBehavior.Continue:
                    Exception ex = null;
                    foreach (var sql in scriptCollection)
                        try
                        {
                            ExecuteSql(sql);
                        }
                        catch (Exception execEx)
                        {
                            ex = execEx;
                        }
                    if (ex != null)
                        throw ex;
                    break;
                case DataUpdateBehavior.Transactional:
                    using (var conn = CreateConnection())
                    {
                        conn.Open();
                        DbTransaction dbTran = null;
                        try
                        {
                            dbTran = conn.BeginTransaction();
                            foreach (var sql in scriptCollection)
                            {
                                using (var dbCommand = conn.CreateCommand())
                                {
                                    dbCommand.Transaction = dbTran;
                                    dbCommand.CommandType = CommandType.Text;

                                    dbCommand.CommandText = sql.Sql;
                                    sql.ParamterCollection.ForEach(item =>
                     {
                         _db.AddInParameter(dbCommand, item.ParameterName, item.DbType, item.Value);
                     });
                                    dbCommand.ExecuteNonQuery();
                                }
                            }
                            dbTran.Commit();
                        }
                        catch (Exception)
                        {
                            if (dbTran != null)
                                dbTran.Rollback();
                            throw;
                        }
                        finally
                        {
                            conn.Close();
                            conn.Dispose();
                        } 
                    }
                    break;
            }
        }

        internal void ExecuteMultiSql(List<SqlServerParamter> sqls)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                DbTransaction dbTran = null;
                try
                {
                    dbTran = conn.BeginTransaction();
                    foreach (var sql in sqls)
                    {
                        using (var dbCommand = conn.CreateCommand())
                        {
                            dbCommand.Transaction = dbTran;
                            dbCommand.CommandType = CommandType.Text;

                            dbCommand.CommandText = sql.Sql;
                            sql.ParamterCollection.ForEach(item =>
                            {
                                _db.AddInParameter(dbCommand, item.ParameterName, item.DbType, item.Value);
                            });
                            dbCommand.ExecuteNonQuery();
                        }
                    }
                    dbTran.Commit();
                }
                catch (Exception)
                {
                    if (dbTran != null)
                        dbTran.Rollback();
                    throw;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        #endregion

        #region GetPageSql
        /// <summary>
        ///  获取分页用的SQL(直接包装带where等条件的SQL语句)
        /// </summary>
        /// <param name = "sqls">可以带where等条件的SQL语句</param>
        /// <param name = "minRowNum">最小行号</param>
        /// <param name = "maxRowNum">最大行号</param>
        /// <returns>分页用的SQL语句</returns>
        protected static string GetPageSql(string sqls, int minRowNum, int maxRowNum, ConnectionModes ConnectionMode)
        {
            if (ConnectionMode == ConnectionModes.MICROSOFT_ORACLE || ConnectionMode == ConnectionModes.MICROSOFT_ORACLE9 ||
                ConnectionMode == ConnectionModes.ORACLE || ConnectionMode == ConnectionModes.ORACLE9)
            {
                if (sqls == "")
                    return sqls;
                string sqlstpl =
                    "SELECT Ttt.*\n" +
                    "  FROM (SELECT Tt.*, Rownum rowno FROM (" + sqls + ") Tt) Ttt\n" +
                    " WHERE rowno > " + minRowNum + "\n" +
                    "   AND rowno <= " + maxRowNum;
                return sqlstpl;
            }
            else
            {
                if (sqls == "")
                    return sqls;
                string sqlstpl = @" select t3.*
                    from (select row_number() over(order by t2.rowno_def) rowno,t2.* " +
                    "from (select null rowno_def,t1.* from (" + sqls + ") t1) t2) t3 " +
                    "where  rowno>" + minRowNum + " and rowno<=" + maxRowNum;
                return sqlstpl;
            }
        }
        #endregion

        #region GetPageCount
        /// <summary>
        ///   获取分页总数
        /// </summary>
        /// <param name = "sql">可以带where等条件的SQL语句</param>
        /// <returns></returns>
        public int GetPageCount(string sql)
        {
            var rowdt = DoGetDataTable("select Count(1) Cnt from (" + sql + " ) " + "TBCNT");
            return int.Parse(rowdt.Rows[0]["Cnt"].ToString());
        }
        #endregion

        #region DoGetPageDataTable
        /// <summary>
        ///   执行sql后台分页，返回DataTable
        /// </summary>
        /// <param name = "sqlText">sql语句</param>
        /// <param name = "pageIndex">页索引号</param>
        /// <param name = "pageSize">每页行数</param>
        /// <returns></returns>
        public DataTable DoGetPageDataTable(string sqlText, int pageIndex, int pageSize)
        {
            var beginRowNum = (pageIndex - 1) * pageSize; //开始行号
            if (beginRowNum < 0)
                beginRowNum = 1;
            var endRowNum = pageIndex * pageSize; //结束行号
            var sql = GetPageSql(sqlText, beginRowNum, endRowNum, this.ConnectionMode);
            DataTable dtTable = DoGetDataTable(sql);
            if (dtTable.Columns.IndexOf("rowno") >= 0)
            {
                dtTable.Columns.Remove("rowno");
            }
            if (dtTable.Columns.IndexOf("rowno_DEF") >= 0)
            {
                dtTable.Columns.Remove("rowno_DEF");
            }
            return dtTable;
        }
        #endregion

        #region SelectFromTable
        /// <summary>
        /// 获取表分页数据
        /// </summary>
        /// <param name="tableName">指定返回的表名称</param>
        /// <param name="sqlText">原生sql</param>
        /// <param name="pagination">分页结构</param>
        /// <returns>
        /// DataTable
        /// </returns>
        /// <exception cref="Exception">所有错误信息</exception>
        public DataTable SelectFromTable(string tableName, string sqlText, Pagination pagination)
        {
            var sql = sqlText;
            DataTable dt;

            if (pagination != null && pagination.IsPaging)
                dt =
                    DoGetPageDataTable(AddSQLBuffer(sql), pagination.CurrentPageIndex, pagination.PageSize);
            else
                dt = DoGetDataTable(AddSQLBuffer(sql));
            if (dt != null)
                dt.TableName = tableName;

            if (pagination != null && pagination.IsPaging)
            {
                var rowdt = DoGetDataTable("select Count(1) Cnt from (" + sql + " ) " + "TBCNT");
                pagination.TotalRecords = int.Parse(rowdt.Rows[0]["Cnt"].ToString());
            }
            else if (pagination != null)
                if (dt != null)
                    pagination.TotalRecords = dt.Rows.Count;
            return dt;
        }
        #endregion

        #region SelectFromTable
        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="queryGroup">查询组</param>
        /// <param name="orderBy">排序</param>
        /// <param name="pagination">分页结构</param>
        /// <returns>
        /// DataTable
        /// </returns>
        /// <exception cref="Exception">所有错误信息</exception>
        public DataTable SelectFromTable(string tableName, QueryGroupConditionCollections queryGroup,
                                         OrderCollections orderBy, Pagination pagination)
        {
            var sql = __ParseSelectSQL(tableName, queryGroup, orderBy, this._ConnectionMode);

            return SelectFromTable(tableName, sql, pagination);
        }
        #endregion

        #region SelectFromTable
        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="queryGroup">查询组</param>
        /// <param name="pagination">分页结构</param>
        /// <returns>
        /// DataTable
        /// </returns>
        public DataTable SelectFromTable(string tableName, QueryGroupConditionCollections queryGroup,
                                         Pagination pagination)
        {
            return SelectFromTable(tableName, queryGroup, null, pagination);
        }
        #endregion

        #region SelectFromTable
        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="query">单组查询</param>
        /// <param name="orderBy">排序</param>
        /// <param name="pagination">分页结构</param>
        /// <returns>
        /// DataTable
        /// </returns>
        public DataTable SelectFromTable(string tableName, QueryGroupCondition query, OrderCollections orderBy,
                                         Pagination pagination)
        {
            var queryGroup = new QueryGroupConditionCollections { query };
            return SelectFromTable(tableName, queryGroup, orderBy, pagination);
        }
        #endregion

        #region SelectFromTable
        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="query">单组查询</param>
        /// <param name="pagination">分页结构</param>
        /// <returns>
        /// DataTable
        /// </returns>
        public DataTable SelectFromTable(string tableName, QueryGroupCondition query, Pagination pagination)
        {
            return SelectFromTable(tableName, query, null, pagination);
        }
        #endregion

        #region SelectFromTable
        ///<summary>
        ///  获取表数据
        ///</summary>
        ///<param name = "tableName">表名称</param>
        ///<param name = "queryGroup">查询组</param>
        ///<param name = "orderBy">排序</param>
        ///<returns>DataTable</returns>
        public DataTable SelectFromTable(string tableName, QueryGroupConditionCollections queryGroup,
                                         OrderCollections orderBy)
        {
            return SelectFromTable(tableName, queryGroup, orderBy, null);
        }
        #endregion

        #region SelectFromTable
        ///<summary>
        ///  获取表数据
        ///</summary>
        ///<param name = "tableName">表名称</param>
        ///<param name = "queryGroup">查询组</param>
        ///<returns>DataTable</returns>
        public DataTable SelectFromTable(string tableName, QueryGroupConditionCollections queryGroup)
        {
            return SelectFromTable(tableName, queryGroup, null, null);
        }
        #endregion

        #region SelectFromTable
        ///<summary>
        ///  获取表数据
        ///</summary>
        ///<param name = "tableName">表名称</param>
        ///<param name = "query">单组查询</param>
        ///<param name = "orderBy">排序</param>
        ///<returns>DataTable</returns>
        public DataTable SelectFromTable(string tableName, QueryGroupCondition query, OrderCollections orderBy)
        {
            return SelectFromTable(tableName, query, orderBy, null);
        }
        #endregion

        #region SelectFromTable
        ///<summary>
        ///  获取表数据
        ///</summary>
        ///<param name = "tableName">表名称</param>
        ///<param name = "query">单组查询</param>
        ///<returns>DataTable</returns>
        public DataTable SelectFromTable(string tableName, QueryGroupCondition query)
        {
            return SelectFromTable(tableName, query, null, null);
        }
        #endregion

        #region SelectFromTable
        ///<summary>
        ///  获取表数据
        ///</summary>
        ///<param name = "tableName">表名称</param>
        ///<param name = "pagination">分页结构</param>
        ///<returns>DataTable</returns>
        public DataTable SelectFromTable(string tableName, Pagination pagination)
        {
            return SelectFromTable(tableName, new QueryGroupConditionCollections(), null, pagination);
        }
        #endregion

        #region SelectFromTable
        ///<summary>
        ///  获取表数据
        ///</summary>
        ///<param name = "tableName">表名称</param>
        ///<returns>DataTable</returns>
        public DataTable SelectFromTable(string tableName)
        {
            return SelectFromTable(tableName, new QueryGroupConditionCollections(), null, null);
        }
        #endregion

        #endregion

        #region Insert 公共方法

        #region InsertIntoTable
        ///<summary>
        ///  插入数据
        ///</summary>
        ///<param name = "updateFieldsDict">插入字段</param>
        ///<exception cref = "Exception">InsertIntoTable 方法插入数据必须参数更新字段</exception>
        public void InsertIntoTable(Dictionary<string, UpdateFieldConditionCollections> updateFieldsDict)
        {
            if (updateFieldsDict == null || updateFieldsDict.Count < 1)
                throw new Exception("InsertIntoTable 方法插入数据必须参数更新字段！");
            foreach (var tableName in updateFieldsDict.Keys)
                ExecuteSql(AddSQLBuffer(__ParseInsertSQL(tableName, updateFieldsDict[tableName], this._ConnectionMode)));
        }
        #endregion

        #region InsertIntoTable
        ///<summary>
        ///  插入数据
        ///</summary>
        ///<param name = "tableName">表名称</param>
        ///<param name = "updateFields">插入字段</param>
        ///<exception cref = "Exception">InsertIntoTable 方法插入数据必须参数更新字段</exception>
        public void InsertIntoTable(string tableName, UpdateFieldConditionCollections updateFields)
        {
            if (updateFields == null || updateFields.Count < 1)
                throw new Exception("InsertIntoTable 方法插入数据必须参数更新字段！");

            ExecuteSql(AddSQLBuffer(__ParseInsertSQL(tableName, updateFields, this._ConnectionMode)));
        }
        #endregion

        #endregion

        #region Update 公共方法

        ///<summary>
        ///  更新数据
        ///</summary>
        ///<param name = "tableName">表名称</param>
        ///<param name = "updateFields">更新字段</param>
        ///<param name = "queryGroup">查询组</param>
        ///<exception cref = "Exception">UpdateTable 方法删除数据必须传入查询组参数!</exception>
        public void UpdateTable(string tableName, UpdateFieldConditionCollections updateFields,
                                QueryGroupConditionCollections queryGroup)
        {
            if (queryGroup == null || queryGroup.Count < 1)
                throw new Exception("UpdateTable  方法更新数据必须传入查询组参数!");
            if (updateFields == null || updateFields.Count < 1)
                throw new Exception("UpdateTable 方法更新数据必须参数更新字段！");

            ExecuteSql(AddSQLBuffer(__ParseUpdateSQL(tableName, updateFields, queryGroup, this._ConnectionMode)));
        }

        ///<summary>
        ///  更新数据
        ///</summary>
        ///<param name = "tableName">表名称</param>
        ///<param name = "updateFields">更新字段</param>
        ///<param name = "query">查询组</param>
        public void UpdateTable(string tableName, UpdateFieldConditionCollections updateFields,
                                QueryGroupCondition query)
        {
            var queryGroup = new QueryGroupConditionCollections { query };
            //return 
            UpdateTable(tableName, updateFields, queryGroup);
        }

        ///<summary>
        ///  更新全部数据
        ///</summary>
        ///<param name = "tableName">表名称</param>
        ///<param name = "updateFields">更新字段</param>
        public void UpdateTableAllRows(string tableName, UpdateFieldConditionCollections updateFields)
        {
            ExecuteSql(AddSQLBuffer(__ParseUpdateSQL(tableName, updateFields, null, this._ConnectionMode)));
        }

        #endregion

        #region Delete 公共方法

        ///<summary>
        ///  删除数据
        ///</summary>
        ///<param name = "queryGroupDict">查询组集合(表名称，查询集合)</param>
        ///<exception cref = "Exception">DeleteFromTable 方法删除数据必须传入查询组参数!</exception>
        public void DeleteFromTable(Dictionary<string, QueryGroupConditionCollections> queryGroupDict)
        {
            if (queryGroupDict == null || queryGroupDict.Count < 1)
                throw new Exception("DeleteFromTable 方法删除数据必须传入查询组参数!");

            foreach (var tableName in queryGroupDict.Keys)
                ExecuteSql(AddSQLBuffer(__ParseDeleteSQL(tableName, queryGroupDict[tableName], this._ConnectionMode)));
        }

        ///<summary>
        ///  删除数据
        ///</summary>
        ///<param name = "tableName">表名称</param>
        ///<param name = "queryGroup">查询组集合</param>
        ///<exception cref = "Exception">DeleteFromTable 方法删除数据必须传入查询组参数!</exception>
        public void DeleteFromTable(string tableName, QueryGroupConditionCollections queryGroup)
        {
            if (queryGroup == null || queryGroup.Count < 1)
                throw new Exception("DeleteFromTable 方法删除数据必须传入查询组参数!");

            ExecuteSql(AddSQLBuffer(__ParseDeleteSQL(tableName, queryGroup, this._ConnectionMode)));
        }

        ///<summary>
        ///  删除数据
        ///</summary>
        ///<param name = "tableName">表名称</param>
        ///<param name = "query">查询组</param>
        public void DeleteFromTable(string tableName, QueryGroupCondition query)
        {
            var queryGroup = new QueryGroupConditionCollections { query };
            //return 
            DeleteFromTable(tableName, queryGroup);
        }

        ///<summary>
        ///  删除数据
        ///</summary>
        ///<param name = "tableName">表名称</param>
        public void EmptyTable(string tableName)
        {
            ExecuteSql(AddSQLBuffer(__ParseDeleteSQL(tableName, null, this._ConnectionMode)));
        }

        #endregion

        #region UpdataDataTable 公共方法

        ///<summary>
        ///  更新DataTable
        ///</summary>
        ///<param name = "updateCommandsList">更新命令集合列表</param>
        ///<exception cref = "Exception">1。表名称错误 2。表主键配置错误</exception>
        public void UpdateDataTable(List<UpdateCommandConditionCollections> updateCommandsList)
        {
            var sqlCommandLists = new List<string>();
            foreach (var updateCommands in updateCommandsList)
                foreach (var updateCommand in updateCommands)
                {
                    //获取一次初始化配置
                    InitTableColumnsList(updateCommand.DataSource.TableName);
                    if (!tabColumns.ContainsKey(updateCommand.DataSource.TableName.ToUpper()))
                    {
                        throw new Exception("物理表名称" + updateCommand.DataSource.TableName + "错误！");
                    }
                    if (updateCommand.DataSource.PrimaryKey.Length < 1)
                        throw new Exception("表" + updateCommand.DataSource.TableName +
                                            "无主键字段！请配置dataTable 的PrimaryKey属性！");
                    sqlCommandLists.AddRange(__ParseUpdateDataTable(updateCommand, this._ConnectionMode));
                }
            if (updateCommandsList.Count > 0)
                ExecuteMultiSql(updateCommandsList[updateCommandsList.Count - 1].UpdateBehavior,
                                AddSQLBuffer(sqlCommandLists.ToArray()));
        }

        ///<summary>
        ///  更新DataTable
        ///</summary>
        ///<param name = "updateCommands">更新命令集合</param>
        public void UpdateDataTable(UpdateCommandConditionCollections updateCommands)
        {
            var updateCommandColl = new List<UpdateCommandConditionCollections> { updateCommands };
            UpdateDataTable(updateCommandColl);
        }

        ///<summary>
        ///  更新DataTable
        ///</summary>
        ///<param name = "dataSource">数据源</param>
        ///<param name = "commandType">命令</param>
        ///<param name = "updateBehavior">更新行为</param>
        public void UpdateDataTable(DataTable dataSource, UpdateCommandType commandType,
                                    DataUpdateBehavior updateBehavior)
        {
            var updateCommandColl = new UpdateCommandConditionCollections(updateBehavior) { { dataSource, commandType, updateBehavior } };
            UpdateDataTable(updateCommandColl);
        }

        #endregion

        #region SQL缓冲方法

        #region ClearSqlBuffer
        /// <summary>
        ///   清空sql缓冲记录
        /// </summary>
        public void ClearSqlBuffer()
        {
            //sqlBufferList.Clear();
        }
        #endregion

        #region GetSQLBuffer
        /// <summary>
        ///   取sql缓冲记录
        /// </summary>
        /// <returns>sql集合</returns>
        public string[] GetSQLBuffer()
        {
            return _sqlBufferList.ToArray();
        }
        #endregion

        #endregion

        #region 事务处理

        /// <summary>
        /// 数据连接对象
        /// </summary>
        /// <value>
        ///   创建一个连接
        /// </value>
        public DbConnection CreatedConnection
        {
            get;
            set;
        }

        /// <summary>
        /// Occurs when [intercept action].
        /// </summary>
        protected event Action<DbConnection, DbTransaction> InterceptAction;

        /// <summary>
        /// Invokes the intercept action.
        /// </summary>
        /// <param name="conn">The conn.</param>
        /// <param name="tran">The tran.</param>
        protected void InvokeInterceptAction(DbConnection conn, DbTransaction tran)
        {
            Action<DbConnection, DbTransaction> handler = InterceptAction;
            if (handler != null)
                handler(conn, tran);
        }

        /// <summary>
        ///   Occurs when [on before action].
        /// </summary>
        protected event Action<DataTransaction> OnBeforeAction;

        /// <summary>
        ///   Invokes the on before action.
        /// </summary>
        protected void InvokeOnBeforeAction()
        {
            var handler = OnBeforeAction;
            if (handler != null)
                handler(this);
        }

        /// <summary>
        ///   Occurs when [on after action].
        /// </summary>
        protected event Action<DataTransaction> OnAfterAction;

        /// <summary>
        ///   Invokes the on after action.
        /// </summary>
        protected void InvokeOnAfterAction()
        {
            var handler = OnAfterAction;
            if (handler != null)
                handler(this);
        }


        /// <summary>
        ///   Occurs when [on catch action].
        /// </summary>
        protected event Action<DataTransaction, Exception> OnCatchAction;

        /// <summary>
        ///   Invokes the on catch action.
        /// </summary>
        /// <param name = "ex">The ex.</param>
        protected void InvokeOnCatchAction(Exception ex)
        {
            var handler = OnCatchAction;
            if (handler != null)
                handler(this, ex);
        }


        /// <summary>
        ///   Occurs when [on finally action].
        /// </summary>
        protected event Action<DataTransaction> OnFinallyAction;

        /// <summary>
        ///   Invokes the on finally action.
        /// </summary>
        protected void InvokeOnFinallyAction()
        {
            var handler = OnFinallyAction;
            if (handler != null)
                handler(this);
        }

        /// <summary>
        /// 执行过程
        /// </summary>
        /// <param name="action">动作(db连接和事务处理方式)</param>
        /// <returns></returns>
        public DataTransaction Intercept(Action<DbConnection, DbTransaction> action)
        {
            InterceptAction = action;
            return this;
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name = "action">动作(事务处理方式)</param>
        /// <returns></returns>
        public DataTransaction OnBefore(Action<DataTransaction> action)
        {
            OnBeforeAction = action;
            return this;
        }

        /// <summary>
        /// 执行后
        /// </summary>
        /// <param name = "action">动作(事务处理方式)</param>
        /// <returns></returns>
        public DataTransaction OnAfter(Action<DataTransaction> action)
        {
            OnAfterAction = action;
            return this;
        }

        /// <summary>
        /// 调用失败
        /// </summary>
        /// <param name = "action">动作(事务处理方式和异常信息)</param>
        /// <returns></returns>
        public DataTransaction OnCatch(Action<DataTransaction, Exception> action)
        {
            OnCatchAction = action;
            return this;
        }

        /// <summary>
        /// 调用结束
        /// </summary>
        /// <param name = "action">动作(事务处理方式)</param>
        /// <returns></returns>
        public DataTransaction OnFinally(Action<DataTransaction> action)
        {
            OnFinallyAction = action;
            return this;
        }

        /// <summary>
        /// 创建数据连接
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateConnection()
        {
            CreatedConnection = _db.CreateConnection();
            return CreatedConnection;
        }

        /// <summary>
        /// 打开数据连接
        /// </summary>
        public void OpenDb()
        {
            if (CreatedConnection.State == ConnectionState.Broken ||
                CreatedConnection.State == ConnectionState.Closed)
            {
                CreatedConnection.Open();//使用SQL SERVER数据库时，如果出现连接无效的错误，控件内部代码会把CreatedConnection.ConnectionString置空
            }
        }

        /// <summary>
        /// 关闭数据连接
        /// </summary>
        public void CloseDb()
        {
            CreatedConnection.Close();
        }

        /// <summary>
        /// 执行事务嵌套
        /// </summary>
        public void GoTransaction()
        {
            using (var conn = CreateConnection())
            {
                InvokeOnBeforeAction();
                OpenDb();
                var tran = conn.BeginTransaction();
                try
                {
                    InvokeInterceptAction(conn, tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    InvokeOnCatchAction(ex);
                }
                finally
                {
                    tran.Dispose();
                    CloseDb();
                    conn.Dispose();
                    InvokeOnAfterAction();
                    InvokeOnFinallyAction();

                    InterceptAction = null;
                    OnAfterAction = null;
                    OnBeforeAction = null;
                    OnCatchAction = null;
                    OnFinallyAction = null;
                }
            }
        }

        #endregion

    }


    #region ConnectionModes
    public enum ConnectionModes
    {
        NODATABASE = 0,
        ORACLE = 10,
        ORACLE9 = 11,
        MICROSOFT_ORACLE = 12,
        MICROSOFT_ORACLE9 = 13,
        MS_SQL_SERVER_7 = 20,
        MS_SQL_SERVER_2000 = 21,
        MS_SQL_SERVER_2005 = 22,
        MS_SQL_SERVER_2008 = 23,
        MS_ACCESS = 50,
        DB2 = 60,
        MYSQL = 70,
        ODBC = 80,
        SYBASE = 90,
    }

    public class SqlParamterItem
    {
        public SqlParamterItem()
        {
            ParamterCollection = new List<ParameterInfo>();
        }
        /// <summary>
        /// 执行脚本
        /// </summary>
        public string Sql
        {
            get;
            set;
        }
        /// <summary>
        /// 参数
        /// </summary>
        public List<ParameterInfo> ParamterCollection
        {
            get;
            set;
        }
    }

    public class SqlServerParamter
    {
        public SqlServerParamter()
        {
            ParamterCollection = new List<SqlParameter>();
        }
        /// <summary>
        /// 执行脚本
        /// </summary>
        public string Sql
        {
            get;
            set;
        }
        /// <summary>
        /// 参数
        /// </summary>
        public List<SqlParameter> ParamterCollection
        {
            get;
            set;
        }
    }

    #endregion
}