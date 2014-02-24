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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;
using LuaInterface;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Dynamic;
using Lua511;


namespace Maverick
{
   
    public static class Utility
    {      
        public static IEnumerable<string> StringArrayFromTable(LuaTable t)
        {
            return ObjArrayFromTable(t).Cast<string>().ToArray();
        }

        public static object[] ObjArrayFromTable(LuaTable t)
        {
            object[] r = new object[t.Values.Count];
            int i = 0;
            foreach (object value in t.Values)
            {
                if (value is LuaTable) { r[i] = ObjFromTable((LuaTable)value); }
                else
                {
                    r[i] = value;
                }
                i++;
            }
            return r;
        }
        public static object ObjFromTable(LuaTable t)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            
            foreach (DictionaryEntry item in t)
            {
                object obj;
                if (item.Key is double) { return ObjArrayFromTable(t); }
                if (item.Value is LuaTable) { obj = ObjFromTable((LuaTable)item.Value); } else { obj = item.Value; }
                args.Add(item.Key.ToString(), obj);
            }
            return args;
        }
        public class LuaNameValueCollection
        {
            public LuaNameValueCollection(NameValueCollection nvc)
            {
                _nvc = nvc;
            }
            NameValueCollection _nvc = new NameValueCollection();
            public string this[string key] { get { return _nvc[key]; } }
            public void Add(string key,string value) { _nvc.Add(key,value);}
            public void Remove(string key) { _nvc.Remove(key); }
            
        }
        public static bool IsSet(object obj)
        {
            if (Object.Equals(obj, TypeDefault(obj))) return false;            
            return true;
        }
        public static object TypeDefault(object obj)
        {
            Type t = obj.GetType();
            if (t.IsValueType)
            {
                return Activator.CreateInstance(t);
            }            
            else return null;
        }       

        public static string ReplaceString(string target, string find, string replace)
        {
            return target.Replace(find, replace);
        }

        //unfortunately some lua exceptions do not implement .ToString() very well
        public static string ExceptionToString(Exception e)
        {
            Exception e_i = e;
            StringBuilder msgBuilder = new StringBuilder();
            while (e_i != null)
            {
                msgBuilder.Append(e_i.Message);
                msgBuilder.Append("\nStack trace:\n");
                msgBuilder.Append(e_i.StackTrace);
                if ((e_i = e_i.InnerException) != null) { msgBuilder.Append("\nInner exception:\n"); };
            }

            return msgBuilder.ToString();
        }

        public static string TypeName(object o)
        {
            return o.GetType().Name;
        }

        public static Type GetUnderlyingType(LuaInterface.ProxyType type)
        {
            return type.UnderlyingSystemType;
        }

        //public static class LuaDelegateConverter
        //{
        //    public static Action Action;
        //    public static Action<object> Action_1;
        //    public static Action<object,object> Action_2;
        //    public static Action<object, object, object> Action_3;
        //    public static Action<object, object, object, object> Action_4;
        //    public static Action<object, object, object, object, object> Action_5;
        //    public static Action<object, object, object, object, object, object> Action_6;
        //    public static Action<object, object, object, object, object, object, object> Action_7;
        //    public static Action<object, object, object, object, object, object, object, object> Action_8;
        //    public static Action<object, object, object, object, object, object, object, object, object> Action_9;
        //    public static Action<object, object, object, object, object, object, object, object, object, object> Action_10;
        //    public static Action<object, object, object, object, object, object, object, object, object, object, object> Action_11;
        //    public static Action<object, object, object, object, object, object, object, object, object, object, object, object> Action_12;
        //    public static Action<object, object, object, object, object, object, object, object, object, object, object, object, object> Action_13;
        //    public static Action<object, object, object, object, object, object, object, object, object, object, object, object, object, object> Action_14;
        //    public static Action<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> Action_15;
        //    public static Action<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object,object> Action_16;

        //    public static Func<object> Func;
        //    public static Func<object, object> Func_1;
        //    public static Func<object, object, object> Func_2;
        //    public static Func<object, object, object, object> Func_3;
        //    public static Func<object, object, object, object, object> Func_4;
        //    public static Func<object, object, object, object, object, object> Func_5;
        //    public static Func<object, object, object, object, object, object, object> Func_6;
        //    public static Func<object, object, object, object, object, object, object, object> Func_7;
        //    public static Func<object, object, object, object, object, object, object, object, object> Func_8;
        //    public static Func<object, object, object, object, object, object, object, object, object, object> Func_9;
        //    public static Func<object, object, object, object, object, object, object, object, object, object, object> Func_10;
        //    public static Func<object, object, object, object, object, object, object, object, object, object, object, object> Func_11;
        //    public static Func<object, object, object, object, object, object, object, object, object, object, object, object, object> Func_12;
        //    public static Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object> Func_13;
        //    public static Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> Func_14;
        //    public static Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> Func_15;
        //    public static Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> Func_16;

        //}


    }               

 
}
