using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Maverick
{
    static class AppCompiler
    {

        static IMaverickLog _log = ServiceLocator.Resolve<IMaverickLog>();
        static FileManifest preprocess(string srcPath)
        {
            string[] appScript = File.ReadAllLines(srcPath);
            string srcPathDir = Path.GetDirectoryName(srcPath) + "\\";
            List<string> sanitizedAppScript = new List<string>();
            List<string> includes = new List<string>();
            List<string> compiles = new List<string>();
            List<string> references = new List<string>();
            _log.Write("Preprocessing " + srcPath);
            for (int i = 0; i < appScript.Length; i++)
            {
                string line = appScript[i];
                if (!line.StartsWith("#")) sanitizedAppScript.Add(line);
                int idxOfCmt;
                string preprocessorInstruction = String.Concat(line.Take(((idxOfCmt = line.IndexOf("--")) == -1) ? line.Length : idxOfCmt).Skip(1)).Trim();

                if (preprocessorInstruction.StartsWith("include"))
                {
                    string includeFile = preprocessorInstruction.Split(' ')[1].Trim('\'', '"');
                    FileManifest includeAcr = preprocess(Path.GetDirectoryName(srcPath) + "\\" + includeFile);
                    includes.Add(includeAcr.Body);
                    includes.AddRange(includeAcr.Includes);
                    compiles.AddRange(includeAcr.Compiles);
                    _log.Write("\tIncludes " + includeFile);
                }

                if (preprocessorInstruction.StartsWith("compile"))
                {
                    string arg = preprocessorInstruction.Split(' ')[1];
                    string src = "";
                    if (arg.StartsWith("[["))
                    {
                        string currentLine = preprocessorInstruction;
                        int startIdx = currentLine.IndexOf("[[") + 2;
                        int terminatorIdx;
                        while (true)
                        {
                            if (startIdx < currentLine.Length)
                            {
                                if ((terminatorIdx = currentLine.IndexOf("]]")) > -1)
                                {
                                    src = src + currentLine.Substring(startIdx, terminatorIdx);
                                    break;
                                }
                                else
                                {
                                    src = src + currentLine.Substring(startIdx, currentLine.Length);
                                }
                            }
                            i++;
                            currentLine = appScript[i];
                            startIdx = 0;
                        }
                    }
                    else
                    {
                        string filename = arg.Trim('\'', '"').Replace('/', '\\');
                        src = File.ReadAllText(Path.Combine(Path.GetDirectoryName(srcPath), filename));
                        _log.Write("\tCompiles " + filename);
                    }
                                                        
                    compiles.Add(src);
                    
                }

                if (preprocessorInstruction.StartsWith("reference"))
                {
                    string refName = preprocessorInstruction.Split(' ')[1].Trim('\'', '"');
                    string resolvedPath;

                    resolvedPath = AssemblyLocator.ResolveReference(refName, srcPathDir);

                    references.Add(resolvedPath);
                    _log.Write("\tReferences " + refName + " located at " + resolvedPath);
                }

            }

            
                                                                        
            return new FileManifest(String.Join("\r\n", sanitizedAppScript), includes.ToArray(), compiles.ToArray(), references.ToArray());

        }
        public static AppManifest Make(string appPath)
        {
            FileManifest ppr = preprocess(appPath);
            AppManifest appMan = new AppManifest(ppr.Body, ppr.Includes, ppr.Compiles, ppr.References);
            _log.Write("Adding reference to System...");
            appMan.AddReference(AssemblyLocator.ResolveReference("System"));
            _log.Write("Adding reference to mscorlib...");
            appMan.AddReference(AssemblyLocator.ResolveReference("mscorlib"));
            
            if (ppr.Compiles.Count() > 0)
            {
                _log.Write("Compiling C#...");
                Assembly mavDynamic = CSharpCompiler.Compile(Path.Combine(Path.GetTempPath(), "MaverickDynamic"), ppr.Compiles, ppr.References);                             
               appMan.AddReference(mavDynamic.Location);
                _log.Write("Adding reference to MaverickDynamic...");
            }

            return appMan;
        }
    }

}
