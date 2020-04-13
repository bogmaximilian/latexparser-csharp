using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    [Serializable]
    public class MathCmd : Command, IMathGroup
    {
        public List<CommandBase> RelativeCommands { get; set; } = new List<CommandBase>();

        public MathCmd(string name) : base(name)
        {
            
        }

        public override string ToString(int depth)
        {

            string indent = String.Empty;
            for (int i = 0; i < depth; i++)
            {
                indent += "-";
            }

            string cmdstr = "";
            foreach (CommandBase cmd in RelativeCommands)
            {
                cmdstr += cmd.ToString(depth + 1);
            }
            return $"{base.ToString(depth)}" +
                   $"{indent + "-"}RelCmd: " +
                   $"{cmdstr}";
        }
    }
}
