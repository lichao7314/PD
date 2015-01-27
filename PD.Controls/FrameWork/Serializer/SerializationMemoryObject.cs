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
using System.Collections.Generic;

namespace PD.Controls
{
    public class SerializationMemoryObject<Source>
    {
        private ICustomSerializer customDataContractSerializer = new CustomXmlSerializer();

        public string SerializerData { get; set; }

        public void Serializer(Source sourceData)
        {
            SerializerData = customDataContractSerializer.Serializer<Source, string>(sourceData);
        }

        public Source Deserialize()
        {
            return customDataContractSerializer.Deserialize<string, Source>(SerializerData);
        }
         
    }

    public class MemorySourceRevokeManager<Source>
    {
        private SerializationMemoryObject<List<Source>> serialization = new SerializationMemoryObject<List<Source>>();
     

        /// <summary>
        /// 设置初始数据源
        /// </summary>
        /// <param name="source"></param>
        public void SetSource(List<Source> source)
        {
            if (source != null)
                serialization.Serializer(source);
        }

        /// <summary>
        /// 更新当前数据源字段值根据原始值
        /// </summary>
        /// <param name="field"></param>
        /// <param name="currentSource"></param>
        public void UpdateSourceToInitially(string field, List<Source> currentSource)
        {
            var initiallySource = serialization.Deserialize();
            if (currentSource == null||initiallySource==null)
            {
                return;
            }
            if(currentSource.Count!=initiallySource.Count)
                throw new NullReferenceException("原始集合值和当前集合值条数不匹配");
            var currentProperty = typeof(Source).GetProperty(field);
            if (currentProperty == null)
                throw new NullReferenceException("没有在数据模型中找到当前字段"+field);
            currentSource.ForEach(item => 
            { 
                var index=currentSource.IndexOf(item);
                currentProperty.SetValue(item, currentProperty.GetValue(initiallySource[index], null),null);
            });
        }

        public static Source Copy(Source source)
        { 
             SerializationMemoryObject<Source> objectSerialization = new SerializationMemoryObject<Source>();
             objectSerialization.Serializer(source);
             return objectSerialization.Deserialize();
        }
 
    }
}
