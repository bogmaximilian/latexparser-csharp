using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using latexparse_csharp;
using System.Diagnostics;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<CommandBase> cmds = LatexParser.ParseFile(@"D:\temp\test\test.tex");
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
            foreach (CommandBase cmd in cmds)
            {
                Console.Write(cmd.ToString(0));
            }
        }
    }
}
