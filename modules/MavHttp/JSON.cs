using Maverick;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MavHttp
{
    
    public static class JSON
    {
        static JsonSerializer __serializer;
        static JsonSerializer _serializer
        {
            get
            {
                return __serializer ?? (__serializer = new JsonSerializer()
                    {
                        Formatting = Formatting.Indented,
                        TypeNameHandling = TypeNameHandling.None,
                        TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        MaxDepth = 10
                    });
            }
        }

        public static void write(Stream stream, object data)
        {
            writePadded(stream, data, null);
        }
        public static void writePadded(Stream stream, object data, string callback)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                if (callback != null)
                {
                    writer.WriteLine(callback + "(");
                }
                _serializer.Serialize(writer, data);
                if (callback != null)
                {
                    writer.WriteLine(");");
                }
            }            
        }
        
        public static string stringify(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        
        static public dynamic read(Stream jsonStream)
        {
            return createObjFromJObj(_serializer.Deserialize(new JsonTextReader(new StreamReader(jsonStream))) as JObject);
        }

        public static object createObjFromJObj(JObject jObj)
        {
            return parseJObj(jObj);
        }

        static Dictionary<string, Object> parseJObj(JObject jObj)
        {
            Dictionary<string, Object> dict = new Dictionary<string, object>();
            foreach (KeyValuePair<string, JToken> kvp in jObj)
            {
                if (kvp.Value is JValue)
                {
                    dict.Add(kvp.Key, (kvp.Value as JValue).Value);
                }
                if (kvp.Value is JArray)
                {
                    dict.Add(kvp.Key, parseJArray(kvp.Value as JArray));
                }
                if (kvp.Value is JObject)
                {
                    dict.Add(kvp.Key, parseJObj(kvp.Value as JObject));
                }
            }
            return dict;
        }

        static object[] parseJArray(JArray a)
        {
            List<object> r = new List<object>();
            foreach (object obj in a)
            {
                if (obj is JValue)
                {
                    r.Add((obj as JValue).Value);
                }
                if (obj is JArray)
                {
                    r.Add(parseJArray((obj as JArray)));
                }
                if (obj is JObject)
                {
                    r.Add(parseJObj((obj as JObject)));
                }
            }
            return r.ToArray();
        }

    }
}
