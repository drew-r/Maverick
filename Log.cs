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
        static void write(string message,bool verboseOnly = false)
        {
            if (verboseOnly && !Configuration.Verbose) return;
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
        public void WriteIfVerbose(string message)
        {
            write(message, true);
        }
        public void Dispose()
        {
            dispose();
        }


    }
}
