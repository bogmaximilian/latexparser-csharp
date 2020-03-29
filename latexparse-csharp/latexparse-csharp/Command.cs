﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace latexparse_csharp
{
    public class Command
    {
        public string Name { get; set; }

		public List<Parameter> Parameter { get; set; } = new List<Parameter>();

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
            Command cmd = new Command(node.Attributes["Name"].Value);
            foreach (XmlNode subnode in node.ChildNodes)
            {
                switch (subnode.Name)
                {
                    case "GP":
                        cmd.Parameter.Add(new GParameter(subnode.Attributes["Name"].Value));
                        break;

                    case "OP":
                        cmd.Parameter.Add(new OParameter(subnode.Attributes["Name"].Value));
                        break;
                    
                    case "SCP":
                        cmd.Parameter.Add(new SCParameter(subnode.Attributes["Name"].Value, subnode.Attributes["Key"].Value[0]));
                        break;
                }
            }

            return cmd;
        }

	}
}
