using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;


namespace latexparse_csharp
{
    internal enum SearchMode
    {
        BeginCommand,
        CommandSequence,
        Parameters,
        Text
    }

    /// <summary>
    /// Internal ExtensionsMethods to handle DeepCloning, etc...
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Perform a Deep Clone of an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }

    public class LatexParser
    {
        private static string FileData { get; set; }

        private static List<Command> Commands { get; set; }

        /// <summary>
        /// Parse Latex File to C# Class Structure. For further Information see the Docs.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static List<CommandBase> ParseFile(string filepath)
        {
            //Get File Data
            FileData = File.ReadAllText(filepath, Encoding.UTF8).Replace("\r", "").Replace("\n", "");

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

            //Setup normal parameters to get the Subcommands
            GParameter param = new GParameter("test", Parametertypes.Required);
            int counter = 0;
            GetSubCommands(ref param, ref counter);
            return param.SubCommands;
        }

        private static void GetSubCommands(ref GParameter parentparam, ref int counter)
        {
            char endingchar;
            //Get Character that can end this Parameter
            switch (parentparam.Parametertype)
            {
                case Parametertypes.Required:
                    endingchar = (parentparam.CanHaveBody) ? '\\' : '}';
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

                    if (!parentparam.CanHaveBody)
                    {
                        counter = i;
                        return;
                    }
                    else
                    {
                        startindex = i;
                        mode = SearchMode.CommandSequence;
                    }
                }
                else if (mode == SearchMode.BeginCommand || FileData[i] == '\\')
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
                            startindex + 1, i - (startindex + 1)).ToArray());

                        //Check if Command is accepted as an end to the active body parameter + if the parentparam is a body parameter
                        if (parentparam.CanHaveBody && parentparam.EndBodyList.Contains(cmdname))
                        {
                            counter = startindex;
                            return;
                        }

                        Command res = Commands.First(x => x.Name == cmdname);
                        //Verify if Command is loaded
                        if (res != null)
                        {
                            //Update Mode
                            mode = SearchMode.Parameters;

                            //Set Current Command as Copy of recognized Command
                            currcmd = res.DeepClone();

                            //Add Command to Parentparam (can either be acutal parameter or constructed in ParseFile)
                            parentparam.SubCommands.Add(currcmd);

                            //Tune back counter to enable parameter detection
                            i--;
                        }
                    }
                }
                else if (mode == SearchMode.Parameters)
                {
                    //Switch current Character to get SubCommands of Current Parameter
                    switch (FileData[i])
                    {
                        case '{':
                            //Get the according Parameter 
                            GParameter currparam = currcmd.Parameters.OfType<GParameter>()
                                .First(x => x.Parametertype == Parametertypes.Required && !x.ValueRecorded);
                            i++;
                            GetSubCommands(ref currparam, ref i);
                            currparam.ValueRecorded = true;
                            break;
                        case '[':
                            GParameter curroparam = currcmd.Parameters.OfType<GParameter>()
                                .FirstOrDefault(x => x.Parametertype == Parametertypes.Optional && !x.ValueRecorded);
                            i++;
                            GetSubCommands(ref curroparam, ref i);
                            curroparam.ValueRecorded = true;
                            break;
                        default:
                            if (currcmd.Parameters.OfType<SCParameter>().Any(x => !x.ValueRecorded && x.Key == FileData[i]))
                            {
                                SCParameter param = currcmd.Parameters.OfType<SCParameter>().First(x => !x.ValueRecorded);
                                param.ValueRecorded = true;
                                param.Enabled = true;
                            }
                            else if (currcmd.Parameters.OfType<GParameter>().Any(x => !x.ValueRecorded && ((GParameter)x).Parametertype == Parametertypes.Required))
                            {
                                GParameter param = (GParameter)currcmd.Parameters.First(x =>
                                    !x.ValueRecorded &&
                                    ((GParameter)x).Parametertype == Parametertypes.Required);
                                if (FileData[i] != ' ')
                                {
                                    if (param.CanHaveBody)
                                    {
                                        GetSubCommands(ref param, ref i);
                                        param.ValueRecorded = true;
                                        i--;
                                    }
                                    else
                                        param.SubCommands.Add(new TextCommand(FileData[i].ToString()));
                                }
                            }
                            break;
                    }

                    if (currcmd.Parameters.All(x => x.ValueRecorded))
                    {
                        mode = SearchMode.BeginCommand;
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

            if (mode == SearchMode.Text)
            {
                //Get string content and load it into a textcomment
                string txtcontent = new string(new ArraySegment<char>(FileData.ToCharArray(),
                    startindex, FileData.Length - startindex).ToArray());

                //Add TextCommand to parentparam
                ((GParameter)parentparam).SubCommands.Add(new TextCommand(txtcontent));
            }

            counter = FileData.Length - 1;
        }
    }
}
