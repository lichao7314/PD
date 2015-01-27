using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PD.Business
{
    // 摘要:
    //     指定字段或属性的数据类型，以用于 System.Data.OracleClient.OracleParameter。
    public enum DataOracleType
    {
        // 摘要:
        //     Oracle BFILE 数据类型，它包含存储在外部文件中的最大为 4 GB 的二进制数据的引用。使用具有 System.Data.OracleClient.OracleParameter.Value
        //     属性的 OracleClient System.Data.OracleClient.OracleBFile 数据类型。
        BFile = 1,
        //
        // 摘要:
        //     包含二进制数据的 Oracle BLOB 数据类型，其最大大小为 4 GB。使用 System.Data.OracleClient.OracleParameter.Value
        //     中的 OracleClient System.Data.OracleClient.OracleLob 数据类型。
        Blob = 2,
        //
        // 摘要:
        //     Oracle CHAR 数据类型，它包含最大为 2,000 字节的定长字符串。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework System.String 或 OracleClient System.Data.OracleClient.OracleString
        //     数据类型。
        Char = 3,
        //
        // 摘要:
        //     包含字符数据的 Oracle CLOB 数据类型，根据服务器的默认字符集，其最大大小为 4 GB。使用 System.Data.OracleClient.OracleParameter.Value
        //     中的 OracleClient System.Data.OracleClient.OracleLob 数据类型。
        Clob = 4,
        //
        // 摘要:
        //     Oracle REF CURSOR。System.Data.OracleClient.OracleDataReader 对象不可用。
        Cursor = 5,
        //
        // 摘要:
        //     An Oracle DATE data type that contains a fixed-length representation of a
        //     date and time, ranging from January 1, 4712 B.C.to December 31, A.D.默认格式为
        //     dd-mmm-yy。For A.D.dates, DATE maps to System.DateTime.To bind B.C.dates,
        //     use a String parameter and the Oracle TO_DATE or TO_CHAR conversion functions
        //     for input and output parameters respectively.在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework System.DateTime 或 OracleClient System.Data.OracleClient.OracleDateTime
        //     数据类型。
        DateTime = 6,
        //
        // 摘要:
        //     Oracle INTERVAL DAY TO SECOND 数据类型（Oracle 9i 或更高版本），它包含以天、小时、分钟和秒为计量单位的时间间隔，大小固定，为
        //     11 字节。在 System.Data.OracleClient.OracleParameter.Value 中使用 .NET Framework
        //     System.TimeSpan 或 OracleClient System.Data.OracleClient.OracleTimeSpan 数据类型。
        IntervalDayToSecond = 7,
        //
        // 摘要:
        //     Oracle INTERVAL YEAR TO MONTH 数据类型（Oracle 9i 或更高版本），它包含以年和月为单位的时间间隔，大小固定，为
        //     5 字节。在 System.Data.OracleClient.OracleParameter.Value 中使用 .NET Framework
        //     System.Int32 或 OracleClient System.Data.OracleClient.OracleMonthSpan 数据类型。
        IntervalYearToMonth = 8,
        //
        // 摘要:
        //     包含变长二进制数据的 Oracle LONGRAW 数据类型，其最大大小为 2 GB。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework Byte[] 或 OracleClient System.Data.OracleClient.OracleBinary
        //     数据类型。
        LongRaw = 9,
        //
        // 摘要:
        //     Oracle LONG 数据类型，它包含最大为 2 GB 的变长字符串。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework System.String 或 OracleClient System.Data.OracleClient.OracleString
        //     数据类型。
        LongVarChar = 10,
        //
        // 摘要:
        //     Oracle NCHAR 数据类型，它包含要存储在数据库的区域字符集中的定长字符串，存储在数据库中时最大大小为 2,000 字节（不是字符）。值的大小取决于数据库的区域字符集。有关更多信息，请参见
        //     Oracle 文档。在 System.Data.OracleClient.OracleParameter.Value 中使用 .NET Framework
        //     System.String 或 OracleClient System.Data.OracleClient.OracleString 数据类型。
        NChar = 11,
        //
        // 摘要:
        //     Oracle NCLOB 数据类型，它包含要存储在数据库的区域字符集中的字符数据，存储在数据库中时最大大小为 4 GB（不是字符）。值的大小取决于数据库的区域字符集。有关更多信息，请参见
        //     Oracle 文档。在 System.Data.OracleClient.OracleParameter.Value 中使用 .NET Framework
        //     System.String 或 OracleClient System.Data.OracleClient.OracleString 数据类型。
        NClob = 12,
        //
        // 摘要:
        //     An Oracle NUMBER data type that contains variable-length numeric data with
        //     a maximum precision and scale of 38.This maps to System.Decimal.若要绑定超出 System.Decimal.MaxValue
        //     可包含的大小的 Oracle NUMBER，请使用 System.Data.OracleClient.OracleNumber 数据类型，或为输入参数和输出参数分别使用
        //     String 参数和 Oracle TO_NUMBER 或 TO_CHAR 转换函数。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework System.Decimal 或 OracleClient System.Data.OracleClient.OracleNumber
        //     数据类型。
        Number = 13,
        //
        // 摘要:
        //     Oracle NVARCHAR2 数据类型，它包含数据库的区域字符集中存储的变长字符串，存储在数据库中时最大大小为 4,000 字节（不是字符）。值的大小取决于数据库的区域字符集。有关更多信息，请参见
        //     Oracle 文档。在 System.Data.OracleClient.OracleParameter.Value 中使用 .NET Framework
        //     System.String 或 OracleClient System.Data.OracleClient.OracleString 数据类型。
        NVarChar = 14,
        //
        // 摘要:
        //     Oracle RAW 数据类型，它包含最大为 2,000 字节的变长二进制数据。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework Byte[] 或 OracleClient System.Data.OracleClient.OracleBinary
        //     数据类型。
        Raw = 15,
        //
        // 摘要:
        //     Oracle ROWID 数据类型的 base64 字符串表示形式。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework System.String 或 OracleClient System.Data.OracleClient.OracleString
        //     数据类型。
        RowId = 16,
        //
        // 摘要:
        //     Oracle TIMESTAMP（Oracle 9i 或更高版本），它包含日期和时间（包括秒），大小范围从 7 字节到 11 字节。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework System.DateTime 或 OracleClient System.Data.OracleClient.OracleDateTime
        //     数据类型。
        Timestamp = 18,
        //
        // 摘要:
        //     Oracle TIMESTAMP WITH LOCAL TIMEZONE（Oracle 9i 或更高版本），它包含日期、时间和对原始时区的引用，大小范围从
        //     7 字节到 11 字节。在 System.Data.OracleClient.OracleParameter.Value 中使用 .NET Framework
        //     System.DateTime 或 OracleClient System.Data.OracleClient.OracleDateTime 数据类型。
        TimestampLocal = 19,
        //
        // 摘要:
        //     Oracle TIMESTAMP WITH TIMEZONE（Oracle 9i 或更高版本），它包含日期、时间和指定时区，大小固定，为 13 字节。在
        //     System.Data.OracleClient.OracleParameter.Value 中使用 .NET Framework System.DateTime
        //     或 OracleClient System.Data.OracleClient.OracleDateTime 数据类型。
        TimestampWithTZ = 20,
        //
        // 摘要:
        //     Oracle VARCHAR2 数据类型，它包含最大为 4,000 字节的变长字符串。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework System.String 或 OracleClient System.Data.OracleClient.OracleString
        //     数据类型。
        VarChar = 22,
        //
        // 摘要:
        //     An integral type representing unsigned 8-bit integers with values between
        //     0 and 255.这不是本机的 Oracle 数据类型，但是提供此类型以提高绑定输入参数时的性能。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework System.Byte 数据类型。
        Byte = 23,
        //
        // 摘要:
        //     An integral type representing unsigned 16-bit integers with values between
        //     0 and 65535.这不是本机的 Oracle 数据类型，但是提供此类型以提高绑定输入参数时的性能。有关从 Oracle 数值转换为公共语言运行时
        //     (CLR) 数据类型的信息，请参见 System.Data.OracleClient.OracleNumber。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework System.UInt16 或 OracleClient System.Data.OracleClient.OracleNumber
        //     数据类型。
        UInt16 = 24,
        //
        // 摘要:
        //     An integral type representing unsigned 32-bit integers with values between
        //     0 and 4294967295.这不是本机的 Oracle 数据类型，但是提供此类型以提高绑定输入参数时的性能。有关从 Oracle 数值转换为公共语言运行时
        //     (CLR) 数据类型的信息，请参见 System.Data.OracleClient.OracleNumber。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework System.UInt32 或 OracleClient System.Data.OracleClient.OracleNumber
        //     数据类型。
        UInt32 = 25,
        //
        // 摘要:
        //     An integral type representing signed 8 bit integers with values between -128
        //     and 127.这不是本机的 Oracle 数据类型，但是提供此类型以提高绑定输入参数时的性能。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework System.SByte 数据类型。
        SByte = 26,
        //
        // 摘要:
        //     An integral type representing signed 16-bit integers with values between
        //     -32768 and 32767.这不是本机的 Oracle 数据类型，但是提供此类型以提高绑定输入参数时的性能。有关从 Oracle 数值转换为公共语言运行时
        //     (CLR) 数据类型的信息，请参见 System.Data.OracleClient.OracleNumber。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework System.Int16 或 OracleClient System.Data.OracleClient.OracleNumber
        //     数据类型。
        Int16 = 27,
        //
        // 摘要:
        //     An integral type representing signed 32-bit integers with values between
        //     -2147483648 and 2147483647.This is not a native Oracle data type, but is
        //     provided for performance when binding input parameters.有关从 Oracle 数值转换到公共语言运行时数据类型的信息，请参见
        //     System.Data.OracleClient.OracleNumber。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework System.Int32 或 OracleClient System.Data.OracleClient.OracleNumber
        //     数据类型。
        Int32 = 28,
        //
        // 摘要:
        //     单精度浮点值。这不是本机的 Oracle 数据类型，但是提供此类型以提高绑定输入参数时的性能。有关从 Oracle 数值转换到公共语言运行时数据类型的信息，请参见
        //     System.Data.OracleClient.OracleNumber。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework System.Single 或 OracleClient System.Data.OracleClient.OracleNumber
        //     数据类型。
        Float = 29,
        //
        // 摘要:
        //     一个双精度浮点值。这不是本机的 Oracle 数据类型，但是提供此类型以提高绑定输入参数时的性能。有关从 Oracle 数值转换为公共语言运行时
        //     (CLR) 数据类型的信息，请参见 System.Data.OracleClient.OracleNumber。在 System.Data.OracleClient.OracleParameter.Value
        //     中使用 .NET Framework System.Double 或 OracleClient System.Data.OracleClient.OracleNumber
        //     数据类型。
        Double = 30
    }
}
