﻿using System;
using System.Collections.Generic;
using latexparse_csharp;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Command> cmds = LatexParser.ParseFile(@"D:\temp\test\test.tex");
            Console.ReadKey();
        }
    }
}
