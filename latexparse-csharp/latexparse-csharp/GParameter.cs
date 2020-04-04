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

        public override Parameter Clone()
        {
            List<CommandBase> cmds = new List<CommandBase>();
            foreach (CommandBase cmd in SubCommands)
            {
                cmds.Add(cmd.Clone());
            }

            return new GParameter(this.Name, this.Parametertype, this.CanHaveBody)
            {
                SubCommands = cmds,
                ValueRecorded = this.ValueRecorded
            };
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
                cmdstr += cmd.ToString();
            }

            string indent = string.Empty;
            for (int i = 0; i < depth; i++)
            {
                indent += "\t";
            }

            string paramtype = (Parametertype == Parametertypes.Required) ? "GP" : "OP";
            return $"{indent}{paramtype}: {this.Name} \n" +
                   $"{cmdstr}\n";
            
        }
    }
}
