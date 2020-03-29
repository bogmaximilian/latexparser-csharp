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

            //Create Command Dictionary
            List<Command> commands = new List<Command>();

            using (XmlReader reader = XmlReader.Create(new StringReader(Properties.Resources.CommandDictionary),
                new XmlReaderSettings() { IgnoreComments = true}))
            {

                //Read XMl Command Dictionary
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);


                //Get all used packages to load Commands into Dictionary
                Regex regex = new Regex(@"\\usepackage(\[(.*?)\])?\{(.*?)\}");

                //Iterate through Regex Matches
                foreach (Match match in regex.Matches(filedata))
                {
                    for (int i = 0; i < doc.ChildNodes.Count; i++)
                    {
                        if (doc.ChildNodes[i].Name == match.Groups[3].Value || doc.ChildNodes[i].Name == "base")
                        {
                            foreach (XmlNode cmdnode in doc.ChildNodes[i])
                            {
                                commands.Add(Command.Parse(cmdnode));
                            }
                        }

                    }
                }
            }

            return commands;
        }
    }
}
