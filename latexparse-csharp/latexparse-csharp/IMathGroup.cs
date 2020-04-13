using System.Collections.Generic;

namespace latexparse_csharp
{
    public interface IMathGroup
    {
        List<IMathGroup> RelativeCommands { get; set; }
    }
}