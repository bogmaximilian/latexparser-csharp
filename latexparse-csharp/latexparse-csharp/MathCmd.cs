using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public class MathCmd : Command, IMathGroup
    {
        public List<IMathGroup> RelativeCommands { get; set; }

        public MathCmd(string name) : base(name)
        {
            
        }

        public override string ToString(int depth)
        {
            string cmdstr = "";
            foreach (CommandBase cmd in RelativeCommands)
            {
                cmdstr += cmd.ToString(depth + 1);
            }
            return $"{base.ToString(depth)}" +
                   $"\n{cmdstr}";
        }
    }
}
