using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public class SCParameter : Parameter
    {
        public char Key { get; set; }

        public bool Enabled { get; set; } = false;

        public SCParameter(string name, char key) : base(name)
        {
            this.Key = key;
        }

        public override Parameter Clone()
        {
            return new SCParameter(this.Name, this.Key)
            {
                ValueRecorded = this.ValueRecorded
            };
        }

        public override string ToString()
        {
            return this.ToString(0);
        }

        public override string ToString(int depth)
        {
            string indent = string.Empty;
            for (int i = 0; i < depth; i++)
            {
                indent += "\t";
            }

            return $"{indent}SCP: {this.Name}\t{Enabled}";
        }
    }
}
