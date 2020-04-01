using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public class CommandBase
    {
        public virtual CommandBase Clone()
        {
            return new CommandBase();
        }
    }
}
