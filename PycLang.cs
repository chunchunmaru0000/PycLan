using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace PycLan
{
    public static class PycLang
    {
        public static bool Tokens = false;
        public static bool PrintVariablesInDebug = false;
        public static bool PrintFunctionsInDebug = false;
        public static bool PrintVariablesAfterDebug = true;
        public static bool PrintFunctionsAfterDebug = false;
        public static bool Debug = true;
        public static bool TimePrint = true;
        public static void LogTokens(ref Token[] tokens)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(string.Join("|", tokens.Select(t => t.View).ToArray()));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.Join("|", tokens.Select(t => Convert.ToString(t.Value)).ToArray()));
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(string.Join("|", tokens.Select(t => Convert.ToString(t.Type.GetStringValue())).ToArray()));
            Console.ForegroundColor = ConsoleColor.Yellow;
        }

        public static void PrintVariables(bool printVariablesInDebug = false, bool printFunctionsInDebug = false)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            if (printVariablesInDebug)
                foreach (var variable in Objects.Variables)
                {
                    if (variable.Value.GetType().ToString() == "System.Collections.Generic.List`1[System.Object]")
                        Console.WriteLine($"{variable.Key} = [{string.Join(", ", (List<object>)variable.Value)}]; тип <<{TypePrint.Pyc(variable.Value)}>>; ");
                    else
                        Console.WriteLine($"{variable.Key} = {variable.Value}; тип <<{TypePrint.Pyc(variable.Value)}>>; ");
                }

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            if (printFunctionsInDebug)
                foreach (var function in Objects.Functions)
                    Console.WriteLine($"{function.Key} = {function.Value}; тип <<{TypePrint.Pyc(function.Value)}>>; ");
            Console.ResetColor();
        }

        public static void PycOnceLoad(string code)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                var tokens = new Tokenizator(code).Tokenize();
                if (Tokens)
                    LogTokens(ref tokens);

                stopwatch.Start();

                new Parser(tokens).Run(Debug, PrintVariablesInDebug, PrintFunctionsInDebug);
                //IStatement program = new Parser(tokens).Parse();
                //program.Execute();

                stopwatch.Stop();
                if (TimePrint)
                    Console.WriteLine(stopwatch.Elapsed);
            }
            catch (Exception error) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(error.Message); Console.ResetColor(); }

            PrintVariables(PrintVariablesAfterDebug, PrintFunctionsAfterDebug);
        }

        public static void Pyc()
        {
            while (true)
            {
                Console.ResetColor();
                Console.Write("> ");
                string code = Console.ReadLine() ?? "";
                switch (code.Trim())
                {
                    case "Ё ДЕБАГ Ё":
                        Debug = !Debug;
                        Console.WriteLine(Debug ? "Истина" : "Ложь");
                        break;
                    case "Ё ПЕРЕМЕННЫЕ ПОСЛЕ Ё":
                        PrintVariablesAfterDebug = !PrintVariablesAfterDebug;
                        Console.WriteLine(PrintVariablesAfterDebug ? "Истина" : "Ложь");
                        break;
                    case "Ё ПЕРЕМЕННЫЕ Ё":
                        PrintVariablesInDebug = !PrintVariablesInDebug;
                        Console.WriteLine(PrintVariablesInDebug ? "Истина" : "Ложь");
                        break;
                    case "Ё ФУНКЦИИ ПОСЛЕ Ё":
                        PrintFunctionsAfterDebug = !PrintFunctionsAfterDebug;
                        Console.WriteLine(PrintFunctionsAfterDebug ? "Истина" : "Ложь");
                        break;
                    case "Ё ФУНКЦИИ Ё":
                        PrintFunctionsInDebug = !PrintFunctionsInDebug;
                        Console.WriteLine(PrintFunctionsInDebug ? "Истина" : "Ложь");
                        break;
                    case "Ё ТОКЕНЫ Ё":
                        Tokens = !Tokens;
                        Console.WriteLine(Tokens ? "Истина" : "Ложь");
                        break;
                    default:
                        PycOnceLoad(code);
                        break;
                }
                
            }
        }
    }
}
