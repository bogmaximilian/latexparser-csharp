using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;


namespace latexparse_csharp
{
    internal enum SearchMode
    {
        BeginCommand,
        CommandSequence,
        Parameters,
        Text
    }


    public class LatexParser
    {
        private static string FileData { get; set; }

        private static List<Command> Commands { get; set; }

        public static List<Command> ParseFile(string filepath)
        {
            //Get File Data
            FileData = File.ReadAllText(filepath, Encoding.UTF8);

            //Create Command Dictionary
            Commands = new List<Command>();


            using (XmlReader reader = XmlReader.Create(new StringReader(Properties.Resources.CommandDictionary),
                new XmlReaderSettings() { IgnoreComments = true }))
            {

                //Read XMl Command Dictionary
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);


                //Get all used packages to load Commands into Dictionary
                Regex regex = new Regex(@"\\usepackage(\[(.*?)\])?\{(.*?)\}");

                //Iterate through Regex Matches
                foreach (Match match in regex.Matches(FileData))
                {
                    for (int i = 0; i < doc.ChildNodes.Count; i++)
                    {
                        if (doc.ChildNodes[i].Name == match.Groups[3].Value || doc.ChildNodes[i].Name == "base")
                        {
                            foreach (XmlNode cmdnode in doc.ChildNodes[i])
                            {
                                Commands.Add(Command.Parse(cmdnode));
                            }
                        }

                    }
                }
            }

            return Commands;
        }

        private static void GetSubCommands(ref GParameter parentparam, ref int counter)
        {
            char endingchar;
            //Get Character that can end this Parameter
            switch (parentparam.Parametertype)
            {
                case Parametertypes.Required:
                    endingchar = '}';
                    break;

                default:
                    endingchar = ']';
                    break;
            }
            
            //Set Search Mode
            SearchMode mode = SearchMode.BeginCommand;

            //Setup Variables for recording commands
            int startindex = counter;
            Command currcmd = null;
            
            for (int i = counter; i < FileData.Length; i++)
            {
                if (FileData[i] == endingchar)
                {
                    if (mode == SearchMode.Text)
                    {
                        string txtcontent = new string(new ArraySegment<char>(FileData.ToCharArray(),
                            startindex, i - startindex).ToArray());

                        //Add Command and adjust counter + mode accordingly
                        ((GParameter)parentparam).SubCommands.Add(new TextCommand(txtcontent));
                    }
                    else
                    {
                        if (currcmd != null)
                        {
                            parentparam.SubCommands.Add(currcmd);
                        }
                    }

                    i++;
                    return;
                }
                else if (mode == SearchMode.BeginCommand)
                {
                    if (FileData[i] == '\\')
                    {
                        //Start Initiating Command Authorization
                        startindex = i;
                        mode = SearchMode.CommandSequence;
                    }
                    else
                    {
                        startindex = i;
                        mode = SearchMode.Text;
                    }
                }
                else if (mode == SearchMode.CommandSequence)
                {
                    if (!Char.IsLetter(FileData[i]))
                    {
                        //Get Command Signature
                        string cmdname = new string(new ArraySegment<char>(FileData.ToCharArray(),
                            startindex, i - startindex).ToArray());

                        Command res = Commands.First(x => x.Name == cmdname);
                        //Verify if Command is loaded
                        if (res != null)
                        {
                            //Update Mode
                            mode = SearchMode.Parameters;

                            //Set Current Command as Copy of recognized Command
                            currcmd = (Command)res.Clone();

                            //Tune back counter to enable parameter detection
                            i--;
                        }
                    }
                }
                else if (mode == SearchMode.Parameters)
                {
                    switch (FileData[i])
                    {
                        case '{':
                            GParameter currparam = currcmd.Parameters.OfType<GParameter>()
                                .First(x => x.Parametertype == Parametertypes.Required && !x.ValueRecorded);
                            i++;
                            GetSubCommands(ref currparam, ref i);
                            break;
                        case '[':
                            GParameter curroparam = currcmd.Parameters.OfType<GParameter>()
                                .FirstOrDefault(x => x.Parametertype == Parametertypes.Optional && !x.ValueRecorded);
                            i++;
                            GetSubCommands(ref curroparam, ref i);
                            break;
                        default:
                            SCParameter param = currcmd.Parameters.OfType<SCParameter>().First(x => !x.ValueRecorded);
                            if (param != null && FileData[i] == param.Key)
                            {
                                param.ValueRecorded = true;
                                param.Enabled = true;
                            }
                            break;
                    }
                }
                else if (mode == SearchMode.Text)
                {
                    if (FileData[i] == '\\')
                    {
                        //Get string content and load it into a textcomment
                        string txtcontent = new string(new ArraySegment<char>(FileData.ToCharArray(),
                            startindex, i - startindex).ToArray());

                        //Add Command and adjust counter + mode accordingly
                        ((GParameter)parentparam).SubCommands.Add(new TextCommand(txtcontent));
                        i--;
                        mode = SearchMode.BeginCommand;
                    }
                }

            }
        }
    }
}
