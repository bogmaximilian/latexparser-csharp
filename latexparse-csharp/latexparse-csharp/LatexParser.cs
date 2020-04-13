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
        Text,
        CmdBegin,
        Math
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
            GParameter param = new GParameter("test", Parametertypes.Required, true, new List<string>());
            int counter = 0;
            GetSubCommands(ref param, ref counter, false);
            return param.SubCommands;
        }

        private static void GetSubCommands(ref GParameter parentparam, ref int counter, bool mathmode, char endingchar = ' ')
        {

            if (endingchar == ' ')
            {
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
            }

            //Set Search Mode
            SearchMode mode = SearchMode.BeginCommand;

            //Setup Variables for recording commands
            int startindex = counter;
            CommandBase currcmd = null;
            CommandBase relmathcmd = null;

            for (int i = counter; i < FileData.Length; i++)
            {
                #region OldCode
                //if (mode == SearchMode.CmdBegin)
                //{
                //    BeginCmd begincmd = new BeginCmd();
                //    GParameter currparam = begincmd.Parameters.OfType<GParameter>()
                //        .First(x => x.Parametertype == Parametertypes.Required && !x.ValueRecorded);
                //    if (currparam.BeginCmdParam)
                //    {
                //        string cmd = ((TextCommand)currparam.SubCommands[0]).Content;
                //        if (parentparam.CanHaveBody && parentparam.EndBodyList.Contains(cmd))
                //        {
                //            counter = startindex;
                //            return;
                //        }
                //        else if (Commands.Exists(x => x.Name == cmd))
                //        {
                //            Command res = Commands.First(x => x.Name == cmd).DeepClone();
                //            begincmd.LoadParams(res.DeepClone());
                //            parentparam.SubCommands.Add(begincmd);
                //            mode = SearchMode.Parameters;
                //        }
                //    }
                //}
                #endregion

                if (FileData[i] == endingchar && mode != SearchMode.Parameters && mode != SearchMode.CommandSequence)
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
                else if (mode == SearchMode.BeginCommand || (FileData[i] == '\\' && mode != SearchMode.Parameters && mode != SearchMode.CommandSequence))
                {
                    switch (FileData[i])
                    {
                        case '\\':
                            //Start Initiating Command Authorization
                            startindex = i;
                            mode = SearchMode.CommandSequence;
                            break;
                        case '$':
                            #region Old MathCmd Implementation
                            //get the startindex and get the mathcontent it will be handled later
                            //startindex = i;
                            //int endindex = FileData.IndexOf('$', startindex + 1);
                            //string mathcontent = new string(new ArraySegment<char>(FileData.ToCharArray(),
                            //    startindex + 1, endindex - (startindex + 1)).ToArray());

                            ////Add the PreMathCmd to the Parentparameter
                            //parentparam.SubCommands.Add(new PreMathCmd(mathcontent));
                            //i = endindex;
                            #endregion
                            //Enable Math Mode and get Parameterized Content of the Command
                            currcmd = new MathRoot();
                            parentparam.SubCommands.Add(currcmd);
                            GParameter mathparam = ((MathRoot)currcmd).MathParam;
                            i++;
                            GetSubCommands(ref mathparam, ref i, true, '$');
                            break;
                        case '{':
                            relmathcmd = new MathGroup();
                            i++;
                            GParameter contentparam = ((MathGroup) relmathcmd).Contentparameter;
                            GetSubCommands(ref contentparam, ref i, mathmode, '}');
                            contentparam.ValueRecorded = true;
                            parentparam.SubCommands.Add(relmathcmd);
                            break;
                        default:
                            if (mathmode)
                            {
                                switch (FileData[i])
                                {
                                    case '_':
                                        mode = SearchMode.Parameters;
                                        currcmd = new Command("NamingCmd");
                                        ((Command)currcmd).Parameters.Add(new GParameter("Content", Parametertypes.Required));
                                        ((IMathGroup)relmathcmd).RelativeCommands.Add(currcmd);
                                        break;
                                    case '^':
                                        mode = SearchMode.Parameters;
                                        currcmd = new Command("PowerCmd");
                                        ((Command)currcmd).Parameters.Add(new GParameter("Content", Parametertypes.Required));
                                        ((IMathGroup)relmathcmd).RelativeCommands.Add(currcmd);
                                        break;
                                    default:
                                        startindex = i;
                                        mode = SearchMode.Text;
                                        break;
                                }
                            }
                            else
                            {
                                startindex = i;
                                mode = SearchMode.Text;
                            }
                            break;
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
                        if (parentparam.CanHaveBody && parentparam.EndBodyList.Contains(cmdname) && !parentparam.IsBeginCmdBody)
                        {
                            counter = startindex;
                            return;
                        }


                        Command res = null;
                        if (Commands.Exists(x => x.Name == cmdname))
                        {
                            res = Commands.First(x => x.Name == cmdname);
                        }
                        else if (cmdname == "end" && parentparam.IsBeginCmdBody)
                        {
                            //get the normal length end command would have and close the ongoing command if the end command matches
                            int expectedlength = ((TextCommand)((GParameter)parentparam.Parent.Parameters[0]).SubCommands[0]).Content.Length;
                            string checkstring = new string(new ArraySegment<char>(FileData.ToCharArray(),
                                i+1, expectedlength).ToArray());
                            if (checkstring == ((TextCommand)((GParameter)parentparam.Parent.Parameters[0]).SubCommands[0])
                                .Content)
                            {
                                counter = i + expectedlength + 2;
                                return;
                            }
                        }
                        else if (cmdname == "begin")
                        {
                            res = new BeginCmd();
                        }

                        //Verify if Command is loaded
                        if (res != null)
                        {
                            //Update Mode
                            mode = SearchMode.Parameters;

                            //Set Current Command as Copy of recognized Command
                            currcmd = res.DeepClone();

                            //Add Command to Parentparam (can either be actual parameter or constructed in ParseFile)
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
                            GParameter currparam = ((Command)currcmd).Parameters.OfType<GParameter>()
                                .First(x => x.Parametertype == Parametertypes.Required && !x.ValueRecorded);
                            if (!currparam.CanHaveBody)
                            {
                                i++;
                                GetSubCommands(ref currparam, ref i, mathmode);
                                currparam.ValueRecorded = true;
                                if (currparam.BeginCmdParam)
                                {
                                    string cmd = ((TextCommand)currparam.SubCommands[0]).Content;
                                    if (Commands.Exists(x => x.Name == cmd))
                                    {
                                        Command res = Commands.First(x => x.Name == cmd).DeepClone();
                                        ((BeginCmd)currcmd).LoadParams(res.DeepClone());
                                    }
                                }
                                break;
                            }
                            goto default;

                        case '[':
                            GParameter curroparam = ((Command)currcmd).Parameters.OfType<GParameter>()
                                .FirstOrDefault(x => x.Parametertype == Parametertypes.Optional && !x.ValueRecorded);
                            i++;
                            GetSubCommands(ref curroparam, ref i, mathmode);
                            curroparam.ValueRecorded = true;
                            break;
                        
                        default:
                            if (FileData[i] != endingchar)
                            {
                                //Check if the Character is a SCParameter
                                if (((Command)currcmd).Parameters.OfType<SCParameter>().Any(x => !x.ValueRecorded && x.Key == FileData[i]))
                                {
                                    SCParameter param = ((Command)currcmd).Parameters.OfType<SCParameter>().First(x => !x.ValueRecorded);
                                    param.ValueRecorded = true;
                                    param.Enabled = true;
                                }
                                else if (((Command)currcmd).Parameters.OfType<GParameter>().Any(x => !x.ValueRecorded && ((GParameter)x).Parametertype == Parametertypes.Required))
                                {
                                    GParameter param = (GParameter)((Command)currcmd).Parameters.First(x =>
                                        !x.ValueRecorded &&
                                        ((GParameter)x).Parametertype == Parametertypes.Required);
                                    if (FileData[i] != ' ')
                                    {
                                        if (param.CanHaveBody)
                                        {
                                            GetSubCommands(ref param, ref i, mathmode);
                                            param.ValueRecorded = true;
                                            i--;
                                        }
                                        else
                                            param.SubCommands.Add(new TextCommand(FileData[i].ToString()));
                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    GParameter currgparam = ((Command)currcmd).Parameters.OfType<GParameter>()
                                        .First(x => x.Parametertype == Parametertypes.Required &&
                                                    !x.ValueRecorded && x.CanHaveBody);
                                    GetSubCommands(ref currgparam, ref i, mathmode);
                                    mode = SearchMode.BeginCommand;
                                    currgparam.ValueRecorded = true;
                                    i--;
                                }
                                catch (Exception)
                                {
                                    mode = SearchMode.BeginCommand;
                                    i--;
                                }
                            }
                            break;
                        
                    }

                    //if (currcmd.Parameters.All(x => x.ValueRecorded))
                    //{
                    //    mode = SearchMode.BeginCommand;
                    //}

                }
                else if (mode == SearchMode.Text)
                {
                    if (FileData[i] == '\\' || FileData[i] == '{' || FileData[i] == '$' || (mathmode && (FileData[i] == '^' || FileData[i] == '_')))
                    {
                        //Get string content and load it into a textcomment
                        string txtcontent = new string(new ArraySegment<char>(FileData.ToCharArray(),
                            startindex, i - startindex).ToArray());
                        txtcontent = (mathmode) ? txtcontent.Replace(" ", "") : txtcontent;

                        if (mathmode)
                        {
                            relmathcmd = new MathGroup();
                            ((MathGroup)relmathcmd).Contentparameter.SubCommands.Add(new TextCommand(txtcontent));
                            ((GParameter)parentparam).SubCommands.Add(relmathcmd);
                        }
                        else
                        {
                            //Add Command and adjust counter + mode accordingly
                            ((GParameter)parentparam).SubCommands.Add(new TextCommand(txtcontent));
                        }
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

            counter = FileData.Length;
        }

    }
}
