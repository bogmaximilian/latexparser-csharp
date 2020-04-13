using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public class MathGroup : CommandBase, IMathGroup
    {
        public List<IMathGroup> RelativeCommands
        {
            get;
            set;
        }

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

            return $"{indent}Mathgroup: " + 
                   $"\n{Contentparameter.ToString(depth + 1)}";
        }
    }
}