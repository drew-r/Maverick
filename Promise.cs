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
                try
                {
                    _result = asyncOp();
	  	            _success = true;		
                }
                catch(Exception e)
                {
                    _taskException = e;
                    if (_exceptionCallback != null) 
                    { 
                        Scheduler.Enqueue(() => _exceptionCallback(_taskException));
                    }                    
                }
                
                if (_success && _successCallback != null)
                {
                    Scheduler.Enqueue(() => _successCallback(_result));
                }
            });
            
            
            
            task.Start();
        }
        
        readonly Task task;
        
	    bool _success = false;
        dynamic _result = null;
        Action<object> _successCallback;
        public Promise success(Action<object> cb)
        {
            _successCallback = cb;
            if (_success) 
            {
                Scheduler.Enqueue(() => _successCallback(_result));
            }
            return this;
        }
       

        Exception _taskException;
        Action<object> _exceptionCallback;
        public Promise error(Action<object> cb)
        {
            _exceptionCallback = cb; 
            if (_taskException != null) 
            {
                Scheduler.Enqueue(() => _exceptionCallback(_taskException));
            }            
            return this;
        }
    }
}
