using System;
using System.Collections.Generic;
using System.Text;

namespace latexparse_csharp
{
    public enum Documentclasstype
    {
        book,
        article,
        report
    }

    public class Documentclass
    {
        public Documentclasstype Documentclasstype { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Date { get; set; }
    }
}
