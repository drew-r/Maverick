using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maverick
{    
    class MaverickApp 
    {
        public MaverickApp(Action run)
        {
            _run = run;
        }
        
        Action _run;
        internal void Run()
        {
            if (_run == null) return;
            _run();
        }        
        
        Dictionary<string, object> _idx = new Dictionary<string, object>();
        public object this[string idx] { get { return _idx[idx]; } set { _idx[idx] = value; } }
        
    }
}
