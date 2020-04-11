using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public class MathCmd : CommandBase
    {

        public GParameter MathParam { get; set; }


        public MathCmd()
        {
            MathParam = new GParameter("MathRoot", Parametertypes.Required);
        }

        public override string ToString(int depth)
        {
            string indent = string.Empty;
            for (int i = 0; i < depth; i++)
            {
                indent += "-";
            }

            string cmdstr = String.Empty;
            foreach (CommandBase cmd in MathParam.SubCommands)
            {
                cmdstr += cmd.ToString(depth + 1);
            }
            //UNDONE
            return $"\n{indent}MathCmd: \n" +
                   $"{cmdstr}\n";
        }


    }
}
