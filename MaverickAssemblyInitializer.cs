using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maverick
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MaverickAssemblyInitializer : Attribute
    {
    }
}
