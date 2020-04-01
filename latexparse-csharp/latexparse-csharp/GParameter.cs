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

        public GParameter(string name, Parametertypes paramtype) : base(name)
        {
            this.Parametertype = paramtype;
        }

        public override Parameter Clone()
        {
            List<CommandBase> cmds = new List<CommandBase>();
            foreach (CommandBase cmd in SubCommands)
            {
                cmds.Add(cmd.Clone());
            }

            return new GParameter(this.Name, this.Parametertype)
            {
                SubCommands = cmds,
                ValueRecorded = this.ValueRecorded
            };
        }
    }
}
