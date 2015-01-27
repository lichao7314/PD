using System.Collections.Generic;
using System.Data.Common; 
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.OracleClient;

namespace PD.Business
{
    /// <summary>
    /// 参数映射类
    /// </summary>
    public class ParameterMapper : IParameterMapper
    {
        // 缓存参数集合
        List<DbParameterStruct> paraList = new List<DbParameterStruct>();
        List<OutOracleCursorPara> cursorList = new List<OutOracleCursorPara>();

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbtype">数据类型</param>
        /// <param name="parameterValue">参数值</param>
        public void AddParameters(string parameterName, DbType dbtype, object parameterValue)
        {
            CreateParameters(dbtype, parameterName, parameterValue, ParameterDirection.Input);
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbtype">数据类型</param>
        /// <param name="parameterValue">参数值</param>
        /// <param name="parameterDirection">参数类型(输入还是输出)</param>
        public void AddParameters(string parameterName, DbType dbtype, object parameterValue, ParameterDirection parameterDirection)
        { 
            CreateParameters(dbtype, parameterName, parameterValue, parameterDirection);
        }
        /// <summary>
        /// 添加Oracle数据库存储过程的输出参数
        /// </summary>
        /// <param name="oracleType">数据类型</param>
        /// <param name="parameterName">参数名称</param>
        public void AddOutOracleCurSorPara(OracleType oracleType, string parameterName)
        {
            OutOracleCursorPara tt = new OutOracleCursorPara();
            tt.ParameterName = parameterName;
            tt.Oracletype = oracleType;
            tt.Parameterdirection = ParameterDirection.Output;
            cursorList.Add(tt);
        }
        // 添加到集合中
        private void CreateParameters(DbType dbtype, string parameterName, object parameterValue, ParameterDirection parameterDirection)
        {
            DbParameterStruct structs = new DbParameterStruct();
            structs.DBtype = dbtype;
            structs.parameterDirection = parameterDirection;
            structs.ParameterName = parameterName;
            structs.ParameterValue = parameterValue;
            paraList.Add(structs);
        }
        /// <summary>
        /// 参数结构
        /// </summary>
        private struct DbParameterStruct
        {

            public DbType DBtype { get; set; }
            public ParameterDirection parameterDirection { get; set; }
            public string ParameterName { get; set; }
            public object ParameterValue { get; set; }
        }
        /// <summary>
        /// 
        /// </summary>
        private struct OutOracleCursorPara
        {
            public string ParameterName { get; set; }
            public OracleType Oracletype { get; set; }
            public ParameterDirection Parameterdirection { get; set; }
        }
        /// <summary>
        /// 实现的接口，必须的
        /// </summary>
        /// <param name="command">Dbcommand</param>
        /// <param name="parameterValues">参数值列表</param>
        public void AssignParameters(DbCommand command, object[] parameterValues)
        {
            foreach (var s in paraList)
            {
                DbParameter parameter = command.CreateParameter();
                parameter.Direction = s.parameterDirection;
                parameter.DbType = s.DBtype;
                parameter.ParameterName = s.ParameterName;
                parameter.Value = s.ParameterValue;
                command.Parameters.Add(parameter);
            }
            foreach (var s in cursorList)
            {
                OracleParameter oraPara1 = new OracleParameter(s.ParameterName, s.Oracletype);
                //指明为输出参数
                oraPara1.Direction = s.Parameterdirection;
                //添加参数到cmd
                command.Parameters.Add(oraPara1);
            }
        }

    }
}
