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
