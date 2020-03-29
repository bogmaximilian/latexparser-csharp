using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Linq;
using System.Linq.Expressions;


namespace latexparse_csharp
{
    public class LatexParser
    {
        public static List<Command> ParseFile(string filepath)
        {
            //Get File Data
            string filedata = File.ReadAllText(filepath, Encoding.UTF8);
            //Get all used packages to load Commands into Dictionary
            Regex regex = new Regex(@"\\usepackage(\[(.*?)\])?\{(.*?)\}");

            //Create Command Dictionary
            List<Command> commands = new List<Command>();
            //Iterate through Regex Matches
            foreach (Match match in regex.Matches(filedata))
            {
                //Check if Packages has implementation
                XmlDocument doc = new XmlDocument();
                doc.Load("CommandDictionary.xml");

                for (int i = 0; i < doc.ChildNodes.Count; i++)
                {
                    if (doc.ChildNodes[i].Attributes["Name"].Value == match.Groups[3].Value)
                    {
                        foreach (XmlNode cmdnode in doc.ChildNodes[i])
                        {
                            commands.Add(Command.Parse(cmdnode));
                        }
                    }
                }
            }

            return commands;
        }
    }
}
