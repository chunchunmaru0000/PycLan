using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PycLan.PycLang;

namespace PycLan
{
    public static class PycLang
    {
        public static void Pyc()
        {
            while (true)
            {
                //       try
                //     {
                Console.Write("> ");
                string code = Console.ReadLine() ?? "";

                var tokens = new Tokenizator(code).Tokenize();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(string.Join("|", tokens.Select(t => t.View).ToArray()));
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(string.Join("|", tokens.Select(t => Convert.ToString(t.Value)).ToArray()));
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(string.Join("|", tokens.Select(t => Convert.ToString(t.Type)).ToArray()));

                var exps = new Parser(tokens).Parse();
                foreach (var exp in exps)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(exp.Evaluated());

                }
                Console.ResetColor();
                //   }
                // catch (Exception error) { Console.ResetColor(); Console.WriteLine(error.Message); }
            }
        }
    }
}
