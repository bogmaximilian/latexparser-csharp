using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public enum Parametertypes
    {
        Required,
        Optional
    }

    [Serializable]
    public class GParameter : Parameter
    {
        public List<CommandBase> SubCommands { get; set; } = new List<CommandBase>();

        public Parametertypes Parametertype { get; set; }

        public bool CanHaveBody { get; set; }

        public GParameter(string name, Parametertypes paramtype, bool body) : base(name)
        {
            this.Parametertype = paramtype;
            this.CanHaveBody = body;
        }

        
        public override string ToString()
        {
            return this.ToString(0);
        }

        public override string ToString(int depth)
        {
            string cmdstr = string.Empty;
            foreach (CommandBase cmd in this.SubCommands)
            {
                cmdstr += cmd.ToString(depth + 1);
            }

            string indent = string.Empty;
            for (int i = 0; i < depth; i++)
            {
                indent += "-";
            }

            string paramtype = (Parametertype == Parametertypes.Required) ? "GP" : "OP";
            return $"{indent}{paramtype}: {this.Name} \n" +
                   $"{cmdstr}\n";
            
        }
    }
}
