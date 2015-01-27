using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace Infotech.Mobile.Server.Common.Server
{
    public class JsonSerializer
    {
        public static string ToJson(object value)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(value.GetType());

            string szJson = "";

            //序列化

            using (MemoryStream stream = new MemoryStream())
            {
                json.WriteObject(stream, value);

                szJson = Encoding.UTF8.GetString(stream.ToArray());

            }
            return szJson;
        }
        public static object ToObject(string value, Type type)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(value)))
            {

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);

                return serializer.ReadObject(ms);

            }
        }

    }
}