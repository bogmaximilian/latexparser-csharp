using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace latexparse_csharp
{
    public class LatexParser
    {
        public static LatexTree ParseFile(string filepath)
        {
            string filedata = File.ReadAllText(filepath, Encoding.UTF8);
            for (int i = 0; i < filedata.Length; i++)
            {
                
            }
        }
    }
}
