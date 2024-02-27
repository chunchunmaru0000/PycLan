using System;

namespace PycLan
{
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
