using System;
using System.Diagnostics;
using System.Linq;

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
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            foreach (var function in Objects.Functions)
            {
                Console.WriteLine($"{function.Key} = {function.Value}; type {function.Value.GetType()}; ");
            }
            Console.ResetColor();
        }

        public static void PycOnceLoad(string code)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                var tokens = new Tokenizator(code).Tokenize();
                LogTokens(ref tokens);

                stopwatch.Start();

                new Parser(tokens).Run();
                //IStatement program = new Parser(tokens).Parse();
                //program.Execute();

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
