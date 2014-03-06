using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maverick
{
    public static class Scheduler
    {
        static BlockingCollection<Tuple<Func<object>, Action<object>>> _syncQueue = new BlockingCollection<Tuple<Func<object>, Action<object>>>();
        public static void Enqueue(Func<object> func, Action<object> callback)
        {
            _syncQueue.Add(new Tuple<Func<object>, Action<object>>(func, callback));
        }
        
        public static void Enqueue(Action<object> action)
        {
            Enqueue(() => { return null; }, action);
        }

        public static void Enqueue(Action action)
        {
            Enqueue(() => { return null; }, (x) => { action(); });
        }

        public static void Exit()
        {
            Enqueue(() => { _run = false; }); 
        }

        public static void Request()
        {
            _requested = true;
        }
        static bool _requested = false;
        static bool _run = true;
        static bool _invoked = false;
        //blocking method that should be called from the thread that will be scheduled into        
        public static void Run()
        {
            if (!_requested) return;
            if (_invoked) return;
            _invoked = true;
            while (_run)
            {
                Tuple<Func<object>, Action<object>> acbPair = _syncQueue.Take();
                try
                {
                    acbPair.Item2(acbPair.Item1());
                }
                catch (Exception e)
                {
                    ServiceLocator.Resolve<IMaverickLog>().Write("Scheduler caught exception: " + Utility.ExceptionToString(e));
                }
            }            
        }        
        
        //utility method that transforms an Action<object> into an AsyncCallback that will be 
        //scheduled into the main thread (for use in IAsyncResult pattern) 
        public static AsyncCallback QueuedCallback(Action<object> callback)
        {
            return (x) => { Enqueue(() => callback(x)); };
        }

        
    }
}
