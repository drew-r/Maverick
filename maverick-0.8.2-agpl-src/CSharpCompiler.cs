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
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;
using System.Reflection;

namespace Maverick
{
    static class CSharpCompiler
    {
     
        public static List<Type> Compile(string assemblyName, string[] fileList, params string[] referencedAssemblies)
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters paras = new CompilerParameters() { OutputAssembly = Path.Combine(Path.GetTempPath(), assemblyName), GenerateInMemory = true };            
            paras.ReferencedAssemblies.Add(AppDomain.CurrentDomain.BaseDirectory + "Maverick.exe");
            if (referencedAssemblies != null) 
            {
                foreach (string referencedAssembly in referencedAssemblies)
                {
                    paras.ReferencedAssemblies.Add(referencedAssembly);                      
                }
            }

            CompilerResults results = codeProvider.CompileAssemblyFromFile(paras,fileList);
            
            if (results.Errors.Count > 0)  
            {
                string msg = "";
                foreach (CompilerError err in results.Errors)
                {
                    msg = String.Concat(msg, err.ErrorNumber + ": " + err.ErrorText + " @ " + err.FileName + " L" + err.Line + " C" + err.Column);
                }
                throw new Exception("Compilation error(s)..." + msg);
            }
            return results.CompiledAssembly.GetTypes().ToList();         
            
        }
        
    }
}
