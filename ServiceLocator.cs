using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Maverick
{
    public static class ServiceLocator
    {
        public static void Initialize()
        {            
            try
            {                
                foreach (string serviceMap in Properties.Settings.Default.ServiceMap.Split(',').Where(i => !string.IsNullOrWhiteSpace(i)))
                {
                    string[] serviceMapPair = serviceMap.Split(':');
                    if (serviceMapPair.Length > 2) { throw new InvalidOperationException(String.Format("Service map {0} invalid.",serviceMap)); }
                    Type concreteType;
                    if ((concreteType = GetType(serviceMapPair[1])) != null)
                    {
                        MapType(GetType(serviceMapPair[0]), concreteType);
                    }
                    else
                    {
                        MapConstructor(GetType(serviceMapPair[0]), () => createInstanceFromTypeOrConstructor(GetType(serviceMapPair[1])));
                    }                              
                }
            }
            catch (Exception e)
            {
                throw new Exception("Unable to parse service map: " + e.Message, e);
            }
        }
        public static T Resolve<T>() 
            where T:class
        {
            return (T)Resolve(typeof(T));
        }
       
        public static object Resolve(Type type)
        {
            object concreteTypeOrConstructor;
            try
            {
                concreteTypeOrConstructor = _map[type];
            }
            catch (KeyNotFoundException knfe)
            {
                throw new Exception("Unable to resolve type:\n Not mapped: " + type.ToString());
            }
                                          
            return createInstanceFromTypeOrConstructor(concreteTypeOrConstructor);           
        }

     
        private static object createInstanceFromTypeOrConstructor(object typeOrConstructor)
        {
            try
            {
                if (typeOrConstructor is Type)
                {
                    return Activator.CreateInstance((Type)typeOrConstructor);
                }
                if (typeOrConstructor is Func<object>)
                {
                    return ((Func<object>)typeOrConstructor)();
                }
                throw new Exception("Invalid concrete type or constructor.");
            }
            catch (Exception e)
            {
                throw new Exception("Unable to create instance of " + typeOrConstructor.ToString() + "\nPreserved exception:\n" + Utility.ExceptionToString(e));
            }            
        }
      

        static Dictionary<Type, object> _map = new Dictionary<Type, object>();
        public static void MapType<TAbstract, TConcrete>() 
            where TAbstract:class 
            where TConcrete:new()
        {
            MapType(typeof(TAbstract), typeof(TConcrete));
        }
        public static void MapType(Type abstractType, Type concreteType)
        {
            map(abstractType, concreteType);
        }

        public static void MapConstructor<TAbstract>(Func<object> constructor)
        {
            MapConstructor(typeof(TAbstract),constructor);
        }
        
        public static void MapConstructor(Type abstractType, Func<object> constructor)
        {
            map(abstractType,constructor);             
        }

        static void map(Type abstractType, object concreteTypeOrConstructor)
        {            
            _map[abstractType] = concreteTypeOrConstructor;
        }

        public static Type GetType(string typeIdentifier)
        {
            string typeNamespace = "";
            string type = "";
            if (typeIdentifier.Contains('.'))
            {
                typeNamespace = typeIdentifier.Split('.').First();
                type = typeIdentifier.Split('.').Last();
            }
            else { type = typeIdentifier; }
            return Type.GetType(type, null, new Func<Assembly, string, bool, Type>(
                    (assembly, typename, throwErr) => { 
                        return Type.GetType(type) ?? 
                            Type.GetType(typeIdentifier) ??
                            Type.GetType(typeNamespace + "." + typename + ", " + typeNamespace) ??
                            Type.GetType(typeNamespace + "." + typename + ", " + "MaverickDynamic") ??
                            Type.GetType("MaverickDynamic." + typename + ", MaverickDynamic") ?? 
                            Type.GetType("Maverick." + typename + ", Maverick"); }));
        }

    
    }
}
