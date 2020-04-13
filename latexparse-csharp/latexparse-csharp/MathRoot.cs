using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    [Serializable]
    public class MathRoot : CommandBase
    {

        public GParameter MathParam { get; set; }


        public MathRoot()
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
            return $"\n{indent}MathRoot: \n" +
                   $"{cmdstr}\n";
        }


    }
}
