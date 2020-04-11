using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public class PreMathCmd : CommandBase
    {
        public string MathContent { get; set; }

        public PreMathCmd(string mcontent)
        {
            this.MathContent = mcontent;
        }

        public PreMathCmd() : this(String.Empty)
        {

        }

        public override string ToString(int depth)
        {
            string indent = string.Empty;
            for (int i = 0; i < depth; i++)
            {
                indent += "-";
            }

            return $"{indent}MathCmd: {this.MathContent}";
        }
    }
}
