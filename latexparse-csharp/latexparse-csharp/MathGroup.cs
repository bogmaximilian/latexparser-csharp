using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public class MathGroup : CommandBase, IMathGroup
    {
        public List<CommandBase> RelativeCommands { get; set; } = new List<CommandBase>();

        public GParameter Contentparameter { get; set; }

        public MathGroup()
        {
            Contentparameter = new GParameter("Group", Parametertypes.Required);
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
                cmdstr += cmd.ToString(depth + 2);
            }

            return $"{indent}Mathgroup: " +
                   $"\n{Contentparameter.ToString(depth + 1)}" +
                   $"{indent + "-"}RelCmds: {cmdstr}";
        }
    }
}