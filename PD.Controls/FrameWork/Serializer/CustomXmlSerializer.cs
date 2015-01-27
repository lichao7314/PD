///开发人员:李超 
///
///时间:2010-12-7
///
///功能描述:xml Serializer 
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
using System.IO;
using System.Xml.Serialization;
using System.Text;

namespace PD.Controls
{ /// <summary>
    /// xml序列化
    /// </summary>
    public class CustomXmlSerializer : ICustomSerializer
    {
        public returnType Serializer<Source, returnType>(Source sourceData)
        {
            object returnValue = default(returnType);

            if (sourceData != null)
            {
                MemoryStream memoryStream = new MemoryStream();

                try
                {
                    XmlSerializer serialzer = new XmlSerializer(typeof(Source));

                    serialzer.Serialize(memoryStream, sourceData);

                    byte[] byteDataInSerialzer = memoryStream.ToArray();

                    object xmlStream = Encoding.UTF8.GetString(byteDataInSerialzer, 0, byteDataInSerialzer.Length);

                    if (typeof(returnType) == typeof(string))
                    {
                        returnValue = xmlStream;
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("错误:{0}类型序列化出错{1}", typeof(Source), ex.Message));
                }
            }
            return (returnType)returnValue;
        }

        public returnType Deserialize<Source, returnType>(Source sourceData)
        {
            object returnValue = default(returnType);

            if ((typeof(Source) == typeof(string)) && (!string.IsNullOrEmpty(sourceData + "")))
            {
                MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(sourceData.ToString()));

                try
                {
                    XmlSerializer serialzer = new XmlSerializer(typeof(returnType));

                    object xmlStream = serialzer.Deserialize(memoryStream);

                    if (typeof(returnType) == xmlStream.GetType())
                    {
                        returnValue = xmlStream;
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("错误:{0}数据反序列化出错{1}", typeof(Source), ex.Message));
                }
            }
            return (returnType)returnValue;
        }
    }
}
