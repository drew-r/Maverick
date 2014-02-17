using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Maverick
{
    public class MethodCaptureDelegator
    {
        public MethodCaptureDelegator(object target)
        {
            _target = target;
            Type targetGetType = _target.GetType();
            _targetType = ((_static = (targetGetType == typeof(LuaInterface.ProxyType))) ? ((LuaInterface.ProxyType)_target).UnderlyingSystemType : targetGetType);
        }

        readonly object _target;
        readonly Type _targetType;
        string _methodName = null;
        bool _static;

        public object this[string idx]
        {
            get
            {
                if (_methodName != null) { throw new InvalidOperationException("Method already set."); }
                _methodName = idx;
                return this;
            }
        }       

        public object go(object[] args)
        {
            args = args.Take(args.Length-1).ToArray();
            return new Func<object>(() => {
                MethodBase method = null;
                
                if (!_static) { args = args.Skip(1).ToArray(); }

                int i = 0;
                    do
                    {

                        if (i > args.Length) { throw new Exception("Could not resolve method " + _methodName + " from type " + _targetType.ToString() + " target " + _target.ToString()); }
                        int iters = 0;
                        method = _targetType.GetMethod(_methodName, args.Select((x) =>
                        {
                            Type type = x.GetType();
                            if (type == typeof(double) && iters <= i) { args[iters] = Convert.ToInt32(args[iters]); type = typeof(int); }
                            iters++;
                            return type;
                        }).ToArray());
                        i++;
                    }
                    while (method == null);
               
                return method.Invoke(_target, args); 
            
            });
        
        }
        public object go()
        {
            return go(null);
        }


    }
}
