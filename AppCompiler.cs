using Goose;

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
            string srcPathDir = Path.GetDirectoryName(srcPath) + Path.DirectorySeparatorChar;
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
                    FileManifest includeAcr = preprocess(Path.GetDirectoryName(srcPath) + Path.DirectorySeparatorChar + includeFile);
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
                        string filename = arg.Trim('\'', '"');
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

        //todo refactor
        public static AppManifest Make(VM vm, string appPath)
        {
            FileManifest ppr = preprocess(appPath);
            AppManifest appMan = new AppManifest(ppr.Body, ppr.Includes, ppr.Compiles, ppr.References);
            
            
            if (ppr.Compiles.Count() > 0)
            {
                _log.Write("Compiling C#...");
                appMan.AddAssembly(vm.Compile(Path.Combine(Path.GetTempPath(), "MaverickDynamic"), ppr.Compiles, ppr.References));
            }

            foreach (string reference in ppr.References)
            {
                appMan.AddAssembly(Goose.AssemblyLocator.ResolveAssembly(reference));
            }

            return appMan;
        }
    }

}
