using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public class GParameter : Parameter
    {
        public List<Command> SubCommands { get; set; }

        public GParameter(string name) : base(name)
        {
            this.SubCommands = new List<Command>();
        }
    }
}
