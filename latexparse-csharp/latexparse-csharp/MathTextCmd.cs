using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public class MathTextCmd : TextCommand
    {
        public List<Command> RelativeMathCommands { get; set; } = new List<Command>();
        
        public MathTextCmd(string content) : base(content)
        {

        }


    }
}
