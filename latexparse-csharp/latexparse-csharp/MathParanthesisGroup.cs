using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public class MathParanthesisGroup : CommandBase, IMathGroup
    {
        public List<CommandBase> RelativeCommands { get; set; } = new List<CommandBase>();

        public char Endingchar { get; set; }

        public GParameter GroupParameter { get; set; }

        public MathParanthesisGroup(char paranthesischar)
        {
            switch (paranthesischar)
            {
                case '[':
                    this.Endingchar = ']';
                    break;
                case '(':
                    this.Endingchar = ')';
                    break;
            }
            GroupParameter = new GParameter("Content", Parametertypes.Required);
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
                   $"\n{GroupParameter.ToString(depth + 1)}" +
                   $"{indent + "-"}RelCmds: {cmdstr}";
        }
    }
}
