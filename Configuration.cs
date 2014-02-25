using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Maverick
{
    public static class Configuration
    {
        internal static void Initialize(string[] args)
        {
            try
            {                
                foreach(string c_arg in args)
                {
                    if (AppPath == null)
                    {
                        switch (c_arg)
                        {
                            case "/v":
                                _verbose = true;
                                continue;
                            case "/repl":
                                _repl = true;
                                continue;
                        }
                        
                        AppPath = Path.GetFullPath(c_arg); continue;
                    }
                    _appArgs.Add(c_arg);
                }
                if (AppPath == null && !REPL) { Console.WriteLine("No app path: assuming REPL mode."); _repl = true; }                
            }
            catch (Exception e)
            {
                throw new Exception("Invalid parameters to executable: " + e.Message, e);
            }
        }

        static string _appPath;
        public static string AppPath { 
            get { return _appPath; } 
            private set 
            {
                if (_repl) { throw new InvalidOperationException("Cannot specify app path with REPL flag..."); }
                _appPath = value; 
            } 
        }

        static List<string> _appArgs = new List<string>();
        public static IEnumerable<string> AppArgs { get { return _appArgs; } }
        static bool _verbose = false;
        public static bool Verbose { get { return _verbose; } }
        static bool _repl = false;
        public static bool REPL { get { return _repl; } }


    }
}
