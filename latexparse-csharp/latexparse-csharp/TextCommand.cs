using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public class TextCommand : CommandBase
    {
        public string Content { get; set; }

        public TextCommand(string content)
        {
            this.Content = content;
        }

        public override CommandBase Clone()
        {
            return new TextCommand(this.Content);
        }
    }
}
