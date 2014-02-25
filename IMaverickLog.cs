using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maverick
{
    public interface IMaverickLog
    {
        void Write(string message);
        void WriteIfVerbose(string message);
            
    }
}
