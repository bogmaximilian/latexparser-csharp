using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    [Serializable]
    public class TextCommand : CommandBase
    {
        public string Content { get; set; }

        public TextCommand(string content)
        {
            this.Content = content;
        }

        public override string ToString()
        {
            return $"TxtCmd: {this.Content}";
        }

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
