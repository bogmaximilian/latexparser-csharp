using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public class OParameter : Parameter
    {
        public string Content { get; set; }

        public OParameter(string name) : base(name)
        {

        }

        public OParameter(string name, string content) : this(name)
        {
            this.Content = content;
        }
    }
}
