using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Maverick
{
    public class DynamicProxy<T> : DynamicObject
    {
        public DynamicProxy(T proxyObj)
        {
            _proxyObj = proxyObj;
        }
        protected T _proxyObj;        
        Dictionary<string, object> _idx = new Dictionary<string, object>();
        public object this[string idx]
        {
            get
            {
                object result;
                if (tryGetMember(idx, out result)) { return result; }
                throw new KeyNotFoundException();
            }
            set
            {
                trySetMember(idx, value);
            }
        }

        bool tryGetMember(string name, out object result)
        {
            if (_idx.ContainsKey(name)) { result = _idx[name]; return true; }
            if (_proxyType.GetMember("get_" + name).Count() > 0)
            {
                result = _proxyType.GetProperty(name).GetValue(_proxyObj, null);
                return true;
            }
            result = null;
            return false;
        }

        protected Type _proxyType = typeof(T);
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            bool tryGetSuccess;
            return (tryGetSuccess = tryGetMember(binder.Name, out result)) ? tryGetSuccess : base.TryGetMember(binder, out result);
        }

        bool trySetMember(string name, object value)
        {
            if (_proxyType.GetMember("set_" + name).Count() > 0)
            {
                PropertyInfo prop = _proxyType.GetProperty(name);
                if (!prop.PropertyType.IsAssignableFrom(value.GetType()) && !Utility.TryAdaptLuaValue(value,prop.PropertyType, out value))
                {
                    return false;
                } 
                prop.SetValue(_proxyObj, value, null);
                return true;
            }
            _idx[name] = value;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return trySetMember(binder.Name, value);
        }

        

    }
}
