//Copyright © 2013 Drew Rathbone.
//drewrathbone@gmail.com 
//
//This file is part of Maverick.
//
//Maverick is free software, you can redistribute it and/or modify it under the terms of GNU Affero General Public License 
//as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. 
//You should have received a copy of the the GNU Affero General Public License, along with Maverick. 
//
//Maverick is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty 
//of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
//Additional permission under the GNU Affero GPL version 3 section 7: 
//If you modify this Program, or any covered work, by linking or combining it with other code, such other code is not for 
//that reason alone subject to any of the requirements of the GNU Affero GPL version 3.
//
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
