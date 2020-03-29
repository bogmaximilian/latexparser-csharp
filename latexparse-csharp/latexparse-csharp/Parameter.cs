using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public class Parameter
    {
        public string Name { get; set; }

        public Parameter(string name)
        {
            this.Name = name;
        }
    }
}
