using System;
namespace Maverick
{
	public class FileManifest
	{
        public FileManifest(string body, string[] includes, string[] compiles, string[] references)
        {
            Body = body;
            Includes = includes;
            Compiles = compiles;
            References = references;
        }
        public string Body { get; private set; }
        public string[] Includes { get; private set; }
        public string[] Compiles { get; private set; }
        public string[] References { get; private set; }
	}
}