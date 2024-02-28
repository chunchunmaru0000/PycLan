using System;
using System.Collections.Generic;

namespace PycLan
{
    public sealed class UserFunction : IFunction
    {
        public string[] Args;
        public IStatement Body;

        public UserFunction(string[] args, IStatement body)
        {
            Args = args;
            Body = body;
        }

        public int ArgsCount()
        {
            return Args.Length;
        }

        public string GetArgName(int i)
        {
            if (i < 0 || i >= ArgsCount())
                return "";
            return Args[i];
        }

        public object Execute(params object[] args)
        {
        //    for(int i = 0; i < ArgsCount(); i++)
          //      Objects.AddVariable(GetArgName(i), args[i]);
            try
            {
                Body.Execute();
            }
            catch (ReturnStatement result)
            {
                return result.GetResult();
            }
            return Objects.NOTHING;
        }

        public override string ToString()
        {
            return $"({string.Join(", ", Args)}){{{Body}}}";
        }
    }

    public sealed class Sinus : IFunction
    {
        public object Execute(object[] x)
        {
            if (x.Length == 0)
                throw new Exception($"НЕДОСТАТОЧНО АРГУМЕНТОВ, БЫЛО: <{x.Length}>");
            return Math.Sin(Convert.ToDouble(x[0]));
        }

        public override string ToString() 
        {
            return $"СИНУС(<>)";
        }
    }

    public sealed class Cosinus : IFunction
    {
        public object Execute(object[] x)
        {
            if (x.Length == 0)
                throw new Exception($"НЕДОСТАТОЧНО АРГУМЕНТОВ, БЫЛО: <{x.Length}>");
            return Math.Cos(Convert.ToDouble(x[0]));
        }

        public override string ToString()
        {
            return $"КОСИНУС(<>)";
        }
    }
}
