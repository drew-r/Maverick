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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Maverick;

namespace Maverick
{
    
    //slight variation to singleton 
    public class Log : IMaverickLog, IDisposable
    {

        static StreamWriter _sw;
        static object _swLock = new object();
        static void write(string message)
        {
            if (Configuration.Verbose)
            {
                Console.WriteLine(message);
            }
            lock (_swLock)
            {
                _sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "maverick.log",true);
                _sw.WriteLine("[" + DateTime.Now.ToString() + "] " + message);
                _sw.Flush();
                _sw.Close();
                _sw.Dispose();
            }
        }
        static bool _disposed = false;
        static void dispose()
        {
            if (_disposed) return; _disposed = true;
            if (_sw != null) { _sw.Dispose(); }
        }

        public void Write(string message)
        {
            write(message);
        }
        public void Dispose()
        {
            dispose();
        }


    }
}
