//Copyright © 2013 Drew Rathbone.
//drewrathbone@gmail.com 
//
//This file is part of Maverick.
//
//Maverick is free software, you can redistribute it and/or modify it under the terms of GNU Affero General Public License 
//as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. 
//You should have received a copy of the the GNU Affero General Public License, along with Maverick. 
//
//Maverick is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty 
//of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
//Additional permission under the GNU Affero GPL version 3 section 7: 
//If you modify this Program, or any covered work, by linking or combining it with other code, such other code is not for 
//that reason alone subject to any of the requirements of the GNU Affero GPL version 3.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            List<string> includes = new List<string>();
            List<string> compiles = new List<string>();
            List<string> references = new List<string>();
            _log.Write("Preprocessing " + srcPath);
            foreach (string line in appScript.Where(item => item.StartsWith("#")))
            {
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
                    string filename = preprocessorInstruction.Split(' ')[1].Trim('\'', '"').Replace('/', '\\');
                    compiles.Add(Path.GetDirectoryName(srcPath) + "\\" + filename);
                    _log.Write("\tCompiles " + filename);
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

            return new FileManifest(String.Join("\r\n", appScript.Where(item => !item.StartsWith("#"))), includes.ToArray(), compiles.ToArray(), references.ToArray());

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
                CSharpCompiler.Compile("MaverickDynamic", ppr.Compiles, ppr.References);
                appMan.AddReference("MaverickDynamic");
                _log.Write("Adding reference to MaverickDynamic...");
            }

            return appMan;
        }
    }

}
