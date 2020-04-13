using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public class MathGroup : TextCommand, IMathGroup
    {
        public List<IMathGroup> RelativeCommands
        {
            get;
            set;
        }

        public MathGroup(string content) : base(content)
        {

        }

        public override string ToString(int depth)
        {
            return "Mathgroup: " + $"\n{base.ToString(depth + 1)}";
        }
    }
}