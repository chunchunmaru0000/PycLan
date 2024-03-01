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

    public sealed class Ceiling : IFunction
    {
        public object Execute(object[] x)
        {
            if (x.Length == 0)
                throw new Exception($"НЕДОСТАТОЧНО АРГУМЕНТОВ, БЫЛО: <{x.Length}>");
            return Math.Ceiling(Convert.ToDouble(x[0]));
        }

        public override string ToString()
        {
            return $"ПОТОЛОК(<>)";
        }
    }

    public sealed class Floor : IFunction
    {
        public object Execute(object[] x)
        {
            if (x.Length == 0)
                throw new Exception($"НЕДОСТАТОЧНО АРГУМЕНТОВ, БЫЛО: <{x.Length}>");
            return Math.Floor(Convert.ToDouble(x[0]));
        }

        public override string ToString()
        {
            return $"ЗАЗЕМЬ(<>)";
        }
    }

    public sealed class Tan : IFunction
    {
        public object Execute(object[] x)
        {
            if (x.Length == 0)
                throw new Exception($"НЕДОСТАТОЧНО АРГУМЕНТОВ, БЫЛО: <{x.Length}>");
            return Math.Tan(Convert.ToDouble(x[0]));
        }

        public override string ToString()
        {
            return $"ТАНГЕНС(<>)";
        }
    }

    public sealed class Max : IFunction
    {
        public object Execute(object[] x)
        {
            if (x.Length == 0)
                throw new Exception($"НЕДОСТАТОЧНО АРГУМЕНТОВ, БЫЛО: <{x.Length}>");
            if (x[0] is long && x[1] is long)
                return Math.Max(Convert.ToInt64(x[0]), Convert.ToInt64(x[1]));
            if (x[0] is long && x[1] is double)
                return Math.Max(Convert.ToInt64(x[0]), Convert.ToDouble(x[1]));
            if (x[0] is double && x[1] is long)
                return Math.Max(Convert.ToDouble(x[0]), Convert.ToInt64(x[1]));
            if (x[0] is double && x[1] is double)
                return Math.Max(Convert.ToDouble(x[0]), Convert.ToDouble(x[1]));
            throw new Exception($"С ДАННЫМИ ТИПАМИ ПЕРЕМЕННЫХ ДАННАЯ ФЕНКЦИЯ НЕВОЗМОЖНА: <{this}> <>");
        }

        public override string ToString()
        {
            return $"НАИБОЛЬШЕЕ(<>)";
        }
    }
}
