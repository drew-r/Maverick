#compile [[
using System;
namespace InlineCompile {
	public class InlineCompile
	{

	public InlineCompile(){
	Console.WriteLine("InlineCompile");
}

} 

}
]]

InlineCompile()

src = str_arr("using System; namespace NewNamespace { public class NewClass { public NewClass() { Console.WriteLine(\"Hello from NewClass\"); } } }")
assembly = CSharpCompiler.Compile("NewAssembly.dll",src)
maverick:ImportAssembly(assembly)
NewClass()


Console.ReadKey()