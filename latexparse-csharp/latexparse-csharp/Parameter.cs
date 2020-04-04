using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    /// <summary>
    /// Provides a base Class for OP, GP and SCP
    /// </summary>
    [Serializable]
    public class Parameter
    {
        /// <summary>
        /// Specifies if the Value has been recorded by the LatexParser
        /// </summary>
        public bool ValueRecorded { get; set; } = false;

        /// <summary>
        /// Name of the Parameter
        /// </summary>
        public string Name { get; set; }

        public Parameter(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Convert the Parameter into a tree-like structured string
        /// </summary>
        /// <param name="depth">Specifies at which level in the Class Structure the Parameter is</param>
        /// <returns></returns>
        public virtual string ToString(int depth)
        {
            return this.ToString();
        }
    }
}
