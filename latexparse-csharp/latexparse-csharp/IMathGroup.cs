using System.Collections.Generic;

namespace latexparse_csharp
{
    public interface IMathGroup
    {
        List<CommandBase> RelativeCommands { get; set; }
    }
}