///开发人员:李超 
///
///时间:2010-12-7
///
///功能描述: Serializer interface
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PD.Controls
{
    public interface ICustomSerializer
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="Source">需要序列化的源类型</typeparam>
        /// <typeparam name="returnType">序列化后的返回类型</typeparam>
        /// <param name="sourceData">源类型对象实例</param>
        /// <returns></returns>
        returnType Serializer<Source, returnType>(Source sourceData);
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="Source">需要反序列化的源类型</typeparam>
        /// <typeparam name="returnType">反序列化后的返回类型</typeparam>
        /// <param name="sourceData">源类型对象实例</param>
        /// <returns></returns>
        returnType Deserialize<Source, returnType>(Source sourceData);
    }
}
