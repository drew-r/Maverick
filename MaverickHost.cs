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
using LuaInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Maverick
{
    class MaverickHost
    {
        AppManifest _appManifest;
        
        Lua _luaVM = new Lua();
        public Lua LuaVM { get { return _luaVM; } }
        IMaverickLog _log;


        //this class (along with alot of other code) needs to be isolated from environment (i.e the console)
        //i think REPL should be implemented by Program - Program uses MaverickHost to either provide REPL, or to run a script or app if given a path
        public MaverickHost(params string[] args)
        {
            //provide a default log
            ServiceLocator.MapType<IMaverickLog, Log>();
            _log = ServiceLocator.Resolve<IMaverickLog>();

            _log.Write("Initializing Configuration...");
            Configuration.Initialize(args);

            _log.Write("Initializing AssemblyLocator...");
            AssemblyLocator.Initialize();

            _log.Write("Initializing ServiceLocator...");
            ServiceLocator.Initialize();

            _log.Write("Preparing Lua virtual machine...");
            _luaVM.DoString("require \"lib/CLRPackage\"");
            _luaVM.DoString("require \"lib/Maverick\"");
            importAssemblyNamespaces(Assembly.GetAssembly(this.GetType()));
            _luaVM["maverick"] = this;
            
            if (Configuration.REPL) { repl(); return; }
                         
            _appManifest = AppCompiler.Make(Configuration.AppPath);
            
            foreach (string reference in _appManifest.References)
            {
                Assembly assembly;
                if ((assembly = AssemblyLocator.ResolveAssembly(reference, true)) == null)
                {
                    throw new Exception("Unable to resolve assembly " + reference);
                }
                ImportAssembly(assembly);
            }                                             
       
            //resolve the log service again in case it has been remapped
            _log = ServiceLocator.Resolve<IMaverickLog>();

          
        }



        private void repl()
        {
            string line;
            while (true)
            {
                try
                {
                    Console.Write(">");
                    line = Console.ReadLine();
                    object[] res = _luaVM.DoString(line);
                    if (res != null) { Console.Write(res); }
                }
                catch (Exception e)
                {
                    Console.WriteLine(Utility.ExceptionToString(e));
                }
            }
        }


        void initializeAssembly(Assembly assembly)  
        {            
                IEnumerable<MethodInfo> initializerMethods = from type in assembly.GetExportedTypes() from method in type.GetMethods() where method.IsDefined(typeof(MaverickAssemblyInitializer), false) select method;
                foreach (MethodInfo initializerMethod in initializerMethods)
                {
                    if (!initializerMethod.IsStatic) throw new Exception("Unable to invoke non-static assembly initializer: " + initializerMethod.Name);
                    if (initializerMethod.GetParameters().Count() > 0) throw new Exception("Unable to invoke assembly initializer with parameters: " + initializerMethod.Name);
                    _log.Write("Initializing assembly " + assembly.FullName + " with method " + initializerMethod.Name);
                    initializerMethod.Invoke(null, null);
                }

        }

        void importAssemblyNamespaces(Assembly assembly)
        {              
            var assemblyNamespaces = (from type in assembly.GetExportedTypes() select new { Namespace = type.Namespace, Assembly = assembly.FullName }).Distinct();
            foreach (var assemblyNamespace in assemblyNamespaces)
            {
                _log.Write("Importing " + assemblyNamespace.Assembly + " :: " + assemblyNamespace.Namespace);
                _luaVM.DoString(String.Format("import('{0}','{1}')", assemblyNamespace.Assembly, assemblyNamespace.Namespace));
            }
                       
        }

        public void ImportAssembly(Assembly assembly)
        {
            initializeAssembly(assembly);
            importAssemblyNamespaces(assembly);            
        }
         
        internal void Run()
        {
            _log.Write("Executing root script body...");
            _luaVM.DoString(_appManifest.Body);
            foreach (string include in _appManifest.Includes)
            {
                _log.Write("Executing included script...");
                _luaVM.DoString(include);
            }

            if (_appContainers.Count > 0)
            {
                _log.Write("Executing " + _appContainers.Count + " app containers...");
                foreach(MaverickApp app in _appContainers.Reverse<MaverickApp>()) { app.Run(); }
            }

            _log.Write("Starting scheduler...");
            Scheduler.Run();
            _log.Write("Scheduler stopped.");
        }


        List<MaverickApp> _appContainers = new List<MaverickApp>();
        public MaverickApp app(Action func)
        {
            _log.Write("Registering app container...");
            MaverickApp app = new MaverickApp(func);
            _appContainers.Add(app);
            return app;            
        }
                
        public void debug()
        {
            System.Diagnostics.Debugger.Break();
        }
        public void debug(object data)
        {
            System.Diagnostics.Debugger.Break();
        }
        public void log(string message)
        {
            log(message, false);            
        }
        public void log(string message, bool verboseOnly)
        {
            if (verboseOnly) { _log.WriteIfVerbose(message); }
            else
            {
                _log.Write(message);
            }
        }
        public void die(string message)
        {
            _log.Write(message);
            Environment.Exit(0);
        }
        public void registerFunction(string path,object target, Delegate func)
        {
            _luaVM.RegisterFunction(path, target, func.Method);
        }

    }



  
}
