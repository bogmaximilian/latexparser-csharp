using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace latexparse_csharp
{
    /// <summary>
    /// Enumerator for the Parametertypes
    /// </summary>
    public enum Parametertypes
    {
        Required,
        Optional
    }

    /// <summary>
    /// GroupParameter recognized as either {} or []
    /// </summary>
    [Serializable]
    public class GParameter : Parameter
    {
        /// <summary>
        /// SubCommands that are in the Parameter
        /// </summary>
        public List<CommandBase> SubCommands { get; set; } = new List<CommandBase>();

        public Parametertypes Parametertype { get; set; }

        /// <summary>
        /// Specifies if the Command is a ParentCommand like section, begin, etc...
        /// </summary>
        public bool CanHaveBody { get; set; }

        /// <summary>
        /// Sets the List of Command that can close the body
        /// </summary>
        public List<string> EndBodyList { get; set; }

        public GParameter(string name, Parametertypes paramtype, bool body, List<string> bodyend) : base(name)
        {
            this.Parametertype = paramtype;
            this.CanHaveBody = body;
            this.EndBodyList = bodyend;
        }

        public GParameter(string name, Parametertypes paramtype) : this(name, paramtype, false, new List<string>())
        {

        }


        public override string ToString()
        {
            return this.ToString(0);
        }
        /// <summary>
        /// Converts the Parameter into a treestructured string 
        /// </summary>
        /// <param name="depth">Specifies at which level in the Class Structure the Parameter is (Normally 0)</param>
        /// <returns></returns>
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
