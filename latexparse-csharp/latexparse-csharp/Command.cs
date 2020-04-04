﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace latexparse_csharp
{
    [Serializable]
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
            //Get cmd name from xml Node
            Command cmd = new Command(node.Attributes["name"].Value);
            

            //Get subnode Name and depending on it setup Command Parameters 
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
                        else
                        {
                            cmd.Parameters.Add(new GParameter(
                                subnode.Attributes["name"].Value, 
                                Parametertypes.Required, false));
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
        /// Converts the Command into a string with values of submembers
        /// This Function is ugly for cleaner output use ToString(int depth)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string paramstr = "";
            foreach (Parameter param in Parameters)
            {
                paramstr += param.ToString();
            }

            return $"Cmd: {this.Name} \n" +
                   $"{paramstr}\n";
        }

        /// <summary>
        /// Convert the Command into a tree-like structured string
        /// </summary>
        /// <param name="depth">Specifies at which level of depth in the Class Structure the element is</param>
        /// <returns></returns>
        public override string ToString(int depth)
        {
            string indent = "";
            for (int i = 0; i  < depth; i++)
            {
                indent += "-";
            }
            
            string paramstr = "";
            foreach (Parameter param in Parameters)
            {
                paramstr += param.ToString(depth + 1);
            }

            return $"{indent}Cmd: {this.Name} \n" +
                   $"{paramstr}\n";
        }
    }
}
