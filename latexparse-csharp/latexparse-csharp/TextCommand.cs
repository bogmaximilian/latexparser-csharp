using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    /// <summary>
    /// Provides lowest layer Command every text is rendered as a textcommand
    /// </summary>
    [Serializable]
    public class TextCommand : CommandBase
    {
        /// <summary>
        /// The String Content of the TextCommand
        /// </summary>
        public string Content { get; set; }

        public TextCommand(string content)
        {
            this.Content = content;
        }

        public override string ToString()
        {
            return $"TxtCmd: {this.Content}";
        }


        /// <summary>
        /// Converts the Command into a treelike structured text
        /// </summary>
        /// <param name="depth">Specifies at which level in the Class Structure the Command is at</param>
        /// <returns></returns>
        public override string ToString(int depth)
        {
            string indent = string.Empty;
            for (int i = 0; i < depth; i++)
            {
                indent += "-";
            }

            return $"{indent}TxtCmd: {this.Content}";
        }
    }
}
