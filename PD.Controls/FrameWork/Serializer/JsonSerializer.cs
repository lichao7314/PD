using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Runtime.Serialization;

namespace PD.Controls
{
    public class JsonSerializer
    {
        public static string ToJson(object value)
        {
            DataContractSerializer json = new DataContractSerializer(value.GetType());
            
            string szJson = "";

            //序列化

            using (MemoryStream stream = new MemoryStream())
            {
                json.WriteObject(stream, value);
                var data=stream.ToArray();
                szJson = Encoding.UTF8.GetString(stream.ToArray(), 0, data.Length);
            }
            return szJson;
        }
        public static object ToObject(string value, Type type)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(value)))
            {

                DataContractSerializer serializer = new DataContractSerializer(type);

                return serializer.ReadObject(ms);

            }
        }

    }
}