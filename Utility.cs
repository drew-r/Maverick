using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;
using NLua;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Dynamic;



namespace Maverick
{
   
    public static class Utility
    { 
      
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

        public static Type GetUnderlyingType(NLua.ProxyType type)
        {
            return type.UnderlyingSystemType;
        }

        public static bool TryAdaptLuaValue(object value, Type targetType, out object result)
        {
            if (value.GetType() == typeof(double))
            {
                if (targetType == typeof(Int16))
                {
                    result = Convert.ToInt16(value);
                    return true;
                }
                if (targetType == typeof(Int32))
                {
                    result = Convert.ToInt32(value);
                    return true;
                }
            }

            result = value;
            return false;
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
