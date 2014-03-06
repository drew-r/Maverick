using Goose;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace Maverick
{
    class MaverickHost
    {
        AppManifest _appManifest;

        VM _vm;
        
        IMaverickLog _log;


        public MaverickHost(params string[] args)
        {
            //provide a default log
            ServiceLocator.MapType<IMaverickLog, Log>();
            _log = ServiceLocator.Resolve<IMaverickLog>();

            _log.Write("Initializing Configuration...");
            Configuration.Initialize(args);

            _log.Write("Initializing ServiceLocator...");
            ServiceLocator.Initialize();

            _log.Write("Preparing Lua virtual machine...");
            _vm = new VM();
            _vm.Import(Assembly.GetExecutingAssembly());
            _vm.DoString("require \"lib/Maverick\"");

            
            _vm["maverick"] = this;
            
            if (Configuration.REPL) { repl(); return; }
            //if (Configuration.Drone) { drone(); return; }
                         
            _appManifest = AppCompiler.Make(_vm, Configuration.AppPath);

            foreach (Assembly assembly in _appManifest.Assemblies)
            {
                initializeAssembly(assembly);
            }

            //resolve the log service again in case it has been remapped
            _log = ServiceLocator.Resolve<IMaverickLog>();

          
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


        private void repl()
        {
            string line;
            while (true)
            {
                try
                {
                    Console.Write(">");
                    line = Console.ReadLine();
                    object[] res = _vm.DoString(line);
                    if (res != null) { Console.Write(res); }
                }
                catch (Exception e)
                {
                    Console.WriteLine(Utility.ExceptionToString(e));
                }
            }
        }


         
        internal void Run()
        {
            _log.Write("Executing root script body...");
            _vm.DoString(_appManifest.Body);
            foreach (string include in _appManifest.Includes)
            {
                _log.Write("Executing included script...");
                _vm.DoString(include);
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
            _vm.RegisterFunction(path, target, func.Method);
        }

    }



  
}
