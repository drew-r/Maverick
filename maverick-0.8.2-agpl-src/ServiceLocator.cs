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

        public static Type GetType(string type)
        {
            string typeAssembly = "";
            if (type.Contains('.'))
            {
                typeAssembly = type.Split('.').First();
                type = type.Split('.').Last();                
            }
            return Type.GetType(type, null, new Func<Assembly, string, bool, Type>(
                    (assembly, typename, throwErr) => { return Type.GetType(typename) ?? Type.GetType(typeAssembly + "." + typename + ", " + typeAssembly) ?? Type.GetType("MaverickDynamic." + typename + ", MaverickDynamic") ?? Type.GetType("Maverick." + typename + ", Maverick"); }
                    ));
        }


    
    }
}
