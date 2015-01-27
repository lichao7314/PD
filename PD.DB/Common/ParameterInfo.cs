using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PD.Business
{
    public class ParameterInfo : IConvertible
    {
        private string _ParameterName;
        private DbType _DbType;
        private object _Value;
        /// <summary>
        /// 构造参数信息
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbType">参数类型</param>
        /// <param name="value">参数值</param>
        public ParameterInfo(string parameterName, DbType dbType, object value)
        {
            this._ParameterName = parameterName;
            this._DbType = dbType;
            this._Value = value;
        }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName { get { return _ParameterName; } set { _ParameterName = value; } }
        /// <summary>
        /// 数据类型
        /// </summary>
        public DbType DbType { get { return _DbType; } set { _DbType = value; } }
        /// <summary>
        /// 参数值
        /// </summary>
        public object Value { get { return _Value; } set { _Value = value; } }

        #region IConvertible
        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return System.Convert.ToBoolean(this.Value);
        }

        public double GetDoubleValue()
        {
            return System.Convert.ToDouble(this.Value);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(this.Value);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(this.Value);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(this.Value);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(this.Value);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return System.Convert.ToDouble(this.Value);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(this.Value);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(this.Value);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(this.Value);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(this.Value);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(this.Value);
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return System.Convert.ToString(this.Value);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(this.Value, conversionType);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(this.Value);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this.Value);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(this.Value);
        }
        #endregion

    }
}
