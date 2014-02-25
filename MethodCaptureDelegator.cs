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
            _targetType = ((_static = (targetGetType == typeof(NLua.ProxyType))) ? ((NLua.ProxyType)_target).UnderlyingSystemType : targetGetType);
        }

        readonly object _target;
        readonly Type _targetType;
        string _methodName = null;
        bool _static;

        public object this[string idx]
        {
            get
            {                   
                _methodName = idx;
                return this;
            }
        }

        public object go(object[] args)
        {
            if (args == null) { args = new object[] { }; } 
            else
            { 
                args = args.Take(args.Length-1).ToArray();
            }

            return new Func<object>(() => {
                               
                if (!_static) { args = args.Skip(1).ToArray(); }
                 MethodBase method = resolveMethod(args);

                return method.Invoke(_target, args); 
            
            });         
        }

  

        MethodBase resolveMethod(object[] args)
        {
               
            MethodInfo method = null;                            
          
            Type[] argTypes = args.Select(a => a.GetType()).ToArray();
            
            IEnumerable<MethodInfo> methodNameMatches = _targetType.GetMethods().Where((mi) => mi.Name == _methodName);
            foreach (MethodInfo methodNameMatch in methodNameMatches)
            {
                ParameterInfo[] pi = methodNameMatch.GetParameters();
                if (pi.Length != argTypes.Length) { continue; }
                bool match = true;
                for (int p_idx = 0; p_idx < pi.Length; p_idx++)
                {
                    Type parameterType = pi[p_idx].ParameterType;
                    Type argType = argTypes[p_idx];
                    object arg = args[p_idx];
                    
                    if (!parameterType.IsAssignableFrom(argType))
                    {
                        if (argType == typeof(double))
                        {
                            if (parameterType == typeof(Int16))
                            {
                                args[p_idx] = Convert.ToInt16(arg);
                                continue;
                            }
                            if (parameterType == typeof(Int32))
                            {
                                args[p_idx] = Convert.ToInt32(arg);
                                continue;
                            }
                        }                       
                        match = false;
                    }
                }

                if (match)
                {
                    method = methodNameMatch;
                }
            }
            if (method == null)
                throw new Exception("Could not resolve method " + _methodName + " from type " + _targetType.ToString() + " target " + _target.ToString() +
                    " with argument types " + String.Join(", ", argTypes.Select(at => at.Name)) + ".");
           

            return method;
        }


        public object go()
        {
            return go(null);
        }


    }
}
