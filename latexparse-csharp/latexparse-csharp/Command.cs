using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace latexparse_csharp
{
    public class Command : CommandBase
    {
        public string Name { get; set; }

        public List<Parameter> Parameters { get; set; } = new List<Parameter>();

        public Command(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Parse Xml node to Command it is specifying.
        /// Required XML Strcuture see CommandDictionary.xml
        /// </summary>
        public static Command Parse(XmlNode node)
        {

            Command cmd = new Command(node.Attributes["name"].Value);

            foreach (XmlNode subnode in node.ChildNodes)
            {

                switch (subnode.Name)
                {
                    case "GP":
                        if (subnode.Attributes["body"] != null)
                        {
                            cmd.Parameters.Add(new GParameter(
                                subnode.Attributes["name"].Value,
                                Parametertypes.Required,
                                bool.Parse(subnode.Attributes["body"].Value)));
                        }
                        break;

                    case "OP":
                        cmd.Parameters.Add(new GParameter(
                            subnode.Attributes["name"].Value,
                            Parametertypes.Optional,
                            false));
                        break;

                    case "SCP":
                        cmd.Parameters.Add(new SCParameter(subnode.Attributes["name"].Value,
                            subnode.Attributes["key"].Value[0]));
                        break;
                }

            }

            return cmd;
        }


        /// <summary>
        /// Create a deep copy of the Class
        /// </summary>
        /// <returns></returns>
        public override CommandBase Clone()
        {
            List<Parameter> clonedparams = new List<Parameter>();

            foreach (Parameter param in this.Parameters)
            {
                clonedparams.Add(param.Clone());
            }
            return new Command(this.Name)
            {
                Parameters = this.Parameters
            };
        }
    }
}
