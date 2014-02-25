using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Maverick
{
    static class AssemblyLocator
    {
        static Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();

        public static void Initialize()
        {
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);            
        }

        public static string ResolveReference(string reference, string relativeTo = null)
        {
            string gacPath;
            string resolvedPath = null;

            string[] refExts = { "", ".dll", ".exe" };
            for (int i = 0; resolvedPath == null && i < refExts.Length; i++)
            {
                string refExt = refExts[i];
                resolvedPath =
                    File.Exists(relativeTo + reference + refExt) ? relativeTo + reference + refExt :
                    File.Exists(AppDomain.CurrentDomain.BaseDirectory + reference + refExt) ? AppDomain.CurrentDomain.BaseDirectory + reference + refExt : 
                    File.Exists((gacPath = queryGAC(Path.GetFileNameWithoutExtension(reference + refExt)))) ? gacPath :
                    null;
            }

            if (resolvedPath == null) { throw new FileNotFoundException("Could not resolve reference " + reference + (relativeTo != null ? " relative to " + relativeTo : "") + "."); }
            return resolvedPath;
        }

        static string queryGAC(string reference)
        {
            try
            {
                return System.GACManagedAccess.AssemblyCache.QueryAssemblyInfo(reference);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static Assembly ResolveAssembly(string identifier, bool tryLoad = false)
        {
            Assembly assembly = null;
            if (_assemblies.TryGetValue(identifier, out assembly)) { return assembly; }
            if (tryLoad)
            {
                assembly = Assembly.LoadFrom(identifier);
            }
            return assembly;
        }
        static void cacheAssembly(Assembly assembly)
        {
            _assemblies[assembly.FullName] = assembly;
            _assemblies[assembly.ManifestModule.ToString()] = assembly;
            _assemblies[assembly.Location] = assembly;
        }                                                                                                                      


        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return ResolveAssembly(args.Name);            
        }
        static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            cacheAssembly(args.LoadedAssembly);
        }
        
        

    }

}
