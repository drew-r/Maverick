
using System.Collections.Generic;
using System.Reflection;
namespace Maverick
{
    class AppManifest
    {
        public AppManifest(string body, string[] includes, string[] compiles, string[] references)
        {
            Body = body;
            Includes = includes;
            Compiles = compiles;
            _references.AddRange(references);
        }
        public string Body { get; private set; }
        public string[] Includes { get; private set; }
        public string[] Compiles { get; private set; }
        List<string> _references = new List<string>();
        public string[] References { get { return _references.ToArray(); } }
        public void AddReference(string reference) { _references.Add(reference); }
        List<Assembly> _assemblies = new List<Assembly>();
        public IEnumerable<Assembly> Assemblies {   get { return _assemblies; } }
        public void AddAssembly(Assembly assembly) { _assemblies.Add(assembly); }
    }
}
