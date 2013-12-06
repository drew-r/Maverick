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
