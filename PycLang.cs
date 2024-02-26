using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PycLan.PycLang;

namespace PycLan
{
    public static class PycLang
    {
        public static void LogTokens(ref Token[] tokens)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(string.Join("|", tokens.Select(t => t.View).ToArray()));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.Join("|", tokens.Select(t => Convert.ToString(t.Value)).ToArray()));
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(string.Join("|", tokens.Select(t => Convert.ToString(t.Type)).ToArray()));
            Console.ForegroundColor = ConsoleColor.Yellow;
        }

        public static void PrintVariables()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            foreach (var variable in Objects.Variables)
            {
                Console.WriteLine($"{variable.Key} = {variable.Value}; type {variable.Value.GetType()}; ");
            }
            Console.ResetColor();
        }

        public static void PycOnceLoad(string code)
        {
            try
            {
                var tokens = new Tokenizator(code).Tokenize();
                //LogTokens(ref tokens);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                new Parser(tokens).Run();

                stopwatch.Stop();
                Console.WriteLine(stopwatch.Elapsed);
            }
            catch (Exception error) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(error.Message); Console.ResetColor(); }

            PrintVariables();
        }

        public static void Pyc()
        {
            while (true)
            {
                Console.ResetColor();
                Console.Write("> ");
                PycOnceLoad(Console.ReadLine() ?? "");
            }
        }
    }
}
