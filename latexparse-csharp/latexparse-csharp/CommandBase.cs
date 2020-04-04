using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    [Serializable]
    public class CommandBase
    {
        public virtual CommandBase Clone()
        {
            return new CommandBase();
        }

        public virtual string ToString(int depth)
        {
            return this.ToString();
        }
    }
}
