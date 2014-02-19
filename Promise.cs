using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maverick
{
    public class Promise
    {
        public Promise(Func<object> asyncOp)
        {
            Scheduler.Request();
            task = new Task(() =>
            {
                result = asyncOp();
            });
            try
            {
                task.Start();
            }
            catch (Exception e)
            {
                raiseException(_taskException);
            }
        }
        dynamic result = null;
        readonly Task task;

        public Promise success(Action<object> cb)
        {
            task.ContinueWith((t) => Scheduler.Enqueue((i) => cb(result)));
            return this;
        }

        void raiseException(Exception e)
        {
            _taskException = e;
            if (_exceptionCallback != null) 
            { 
                Scheduler.Enqueue(() => _exceptionCallback(_taskException));
            }
        }

        Exception _taskException;
        Action<object> _exceptionCallback;
        public Promise error(Action<object> cb)
        {
            _exceptionCallback = cb; 
            if (_taskException != null) { raiseException(_taskException); }            
            return this;
        }
    }
}
