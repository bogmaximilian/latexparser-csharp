using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    [Serializable]
    public class BeginCmd : Command
    {
        public BeginCmd() : base("begin")
        {
            this.Parameters.Add(new GParameter("Command", Parametertypes.Required)
            {
                BeginCmdParam = true
            });
        }

        /// <summary>
        /// Loads the Parameters of a given Command and adds them to the Parameters
        /// Method needs to be supplied with a deepclone of a cmd
        /// </summary>
        /// <param name="cmd">Command to get the paramters from (Must be a deepclone instance)</param>
        public void LoadParams(Command cmd)
        {
            foreach (Parameter param in cmd.Parameters)
            {
                if (param is GParameter && ((GParameter)param).CanHaveBody)
                {
                    ((GParameter) param).Parent = this;
                    ((GParameter)param).IsBeginCmdBody = true;
                }
                this.Parameters.Add(param);
            }

        }
    }
}
