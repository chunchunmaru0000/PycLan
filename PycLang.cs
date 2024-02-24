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
                try
                {
                    Console.Write("> ");
                    string code = Console.ReadLine() ?? "";

                    var tokens = new Tokenizator(code).Tokenize();

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(string.Join("|", tokens.Select(t => t.View).ToArray()));
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(string.Join("|", tokens.Select(t => Convert.ToString(t.Value)).ToArray()));
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(string.Join("|", tokens.Select(t => Convert.ToString(t.Type)).ToArray()));

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    var statements = new Parser(tokens).Parse();

                    foreach (var variable in Objects.Variables)
                    {
                        Console.WriteLine($"{variable.Key} = {variable.Value}; type {variable.Value.GetType()};");
                    }
                    Console.ResetColor();
                }
                catch (Exception error) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(error.Message); Console.ResetColor(); }
            }
        }
    }
}
