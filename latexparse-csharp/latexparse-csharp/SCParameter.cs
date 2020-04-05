using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    /// <summary>
    /// Parameter for single-characters
    /// </summary>
    [Serializable]
    public class SCParameter : Parameter
    {
        /// <summary>
        /// What needs to be specified in order for the Parser to let the SCP take Action. Normally *
        /// </summary>
        public char Key { get; set; }

        /// <summary>
        /// Is the Parameter Enabled
        /// </summary>
        public bool Enabled { get; set; } = false;

        public SCParameter(string name, char key) : base(name)
        {
            this.Key = key;
        }

        public override string ToString()
        {
            return this.ToString(0);
        }

        /// <summary>
        /// Convert the SCParameter into a tree-like structured string
        /// </summary>
        /// <param name="depth">Depth specifies which level in the Class Structure the Parameter is</param>
        /// <returns></returns>
        public override string ToString(int depth)
        {
            string indent = string.Empty;
            for (int i = 0; i < depth; i++)
            {
                indent += "-";
            }

            return $"{indent}SCP: {this.Name} - {Enabled}";
        }
    }
}
