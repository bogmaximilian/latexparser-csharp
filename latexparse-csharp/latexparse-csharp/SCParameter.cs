using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public class SCParameter : Parameter
    {
        public char Key { get; set; }

        public SCParameter(string name, char key) : base(name)
        {
            this.Key = key;
        }
    }
}
