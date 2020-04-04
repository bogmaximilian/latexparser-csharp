using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    /// <summary>
    /// CommandBase provides a BaseClass for Command and TextCommand so both can be in the Same list
    /// </summary>
    [Serializable]
    public class CommandBase
    {
        /// <summary>
        /// Convert the String into a tree-like structured string
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public virtual string ToString(int depth)
        {
            return this.ToString();
        }
    }
}
