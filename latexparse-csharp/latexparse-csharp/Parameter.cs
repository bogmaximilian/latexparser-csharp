using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    [Serializable]
    public class Parameter
    {
        public bool ValueRecorded { get; set; } = false;

        public string Name { get; set; }

        public Parameter(string name)
        {
            this.Name = name;
        }


        public virtual string ToString(int depth)
        {
            return this.ToString();
        }
    }
}
