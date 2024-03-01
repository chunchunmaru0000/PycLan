using System;
using System.Collections.Generic;
using System.ComponentModel;

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
            return MoreMax(x);
            //throw new Exception($"С ДАННЫМИ ТИПАМИ ПЕРЕМЕННЫХ ДАННАЯ ФЕНКЦИЯ НЕВОЗМОЖНА: <{this}> <>");
        }

        private object MoreMax(object[] x)
        {
            int I = 0;
            double max = Convert.ToDouble(x[I]);
            for (int i = 0; i < x.Length; i++)
            {
                double iterable;
                if (x[i] is string)
                    iterable = ((string)x[i]).Length;
                else if (x[i] is bool)
                    iterable = (bool)x[i] ? 1 : 0;
                else
                    iterable = Convert.ToDouble(x[i]);
                if (max < iterable)
                {
                    max = iterable;
                    I = i;
                }
            }
            if (x[I] is double)
                return max;
            if (x[I] is long)
                return Convert.ToInt64(max);
            if (x[I] is string)
                return Convert.ToInt64(((string)x[I]).Length);
            if (x[I] is bool)
                return (bool)x[I] ? 1 : 0;
            throw new Exception($"ЭТОГО ПРОСТО НЕ МОЖЕТ БЫТЬ: <{x[I]}> <{TypePrint.Pyc(x[I])}>");
        }

        public override string ToString()
        {
            return $"НАИБОЛЬШЕЕ(<>)";
        }
    }

    public sealed class Min : IFunction
    {
        public object Execute(object[] x)
        {
            if (x.Length == 0)
                throw new Exception($"НЕДОСТАТОЧНО АРГУМЕНТОВ, БЫЛО: <{x.Length}>");
            return LessMax(x);
            //throw new Exception($"С ДАННЫМИ ТИПАМИ ПЕРЕМЕННЫХ ДАННАЯ ФЕНКЦИЯ НЕВОЗМОЖНА: <{this}> <>");
        }

        private object LessMax(object[] x)
        {
            int I = 0;
            double min = Convert.ToDouble(x[I]);
            for (int i = 0; i < x.Length; i++)
            {
                double iterable;
                if (x[i] is string)
                    iterable = ((string)x[i]).Length;
                else if (x[i] is bool)
                    iterable = (bool)x[i] ? 1 : 0;
                else
                    iterable = Convert.ToDouble(x[i]);
                if (min > iterable)
                {
                    min = iterable;
                    I = i;
                }
            }
            if (x[I] is double)
                return min;
            if (x[I] is long)
                return Convert.ToInt64(min);
            if (x[I] is string || x[I] is bool)
                return x[I];
            throw new Exception($"ЭТОГО ПРОСТО НЕ МОЖЕТ БЫТЬ: <{x[I]}> <{TypePrint.Pyc(x[I])}>");
        }

        public override string ToString()
        {
            return $"НАИМЕНЬШЕЕ(<>)";
        }
    }
}
