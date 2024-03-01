using System;
using System.Collections.Generic;
using System.Linq;

namespace PycLan
{
    public sealed class NumExpression : IExpression
    {
        public object Value { get; set; }

        public NumExpression(object value)
        {
            Value = value;
        }

        public object Evaluated()
        {
            return Value;
        } 

        public override string ToString()
        {
            if (Value is bool)
                return (bool)Value ? "Истина" : "Ложь";
            return Convert.ToString(Value);
        }
    }

    public sealed class UnaryExpression : IExpression
    {
        public Token Operation;
        public IExpression Value;
        
        public UnaryExpression(Token operation, IExpression value)
        {
            Operation = operation;
            Value = value;
        }

        public object Evaluated()
        {
            object value = Value.Evaluated();
            switch (Operation.Type) 
            {
                case TokenType.PLUS:
                    return value;
                case TokenType.MINUS:
                    if (value is long)
                        return -Convert.ToInt64(value);
                    else 
                        return -(double)value;
                case TokenType.NOT:
                    return !Convert.ToBoolean(value);
                default:
                    Console.WriteLine($"value {Value.Evaluated()}|Value {Value}|op {Operation.Type}|Op {Operation.View}");
                    throw new Exception("ДА КАК ТАК ВООБЩЕ ВОЗМОЖНО ЧТО ЛИБО ПОСТАВИТЬ КРОМЕ + ИЛИ - ПЕРЕД ЧИСЛОМ");
            }
        }

        public override string ToString()
        {
            return Operation.View + ' ' + Value.ToString();
        }
    }

    public sealed class BinExpression : IExpression 
    {
        public IExpression left;
        public Token operation;
        public IExpression right;

        public BinExpression(IExpression left, Token operation, IExpression right)
        {
            this.left = left;
            this.operation = operation;
            this.right = right;
        }

        public object Evaluated()
        {
            object lft = left.Evaluated();
            object rght = right.Evaluated();
            if (lft is string || rght is string)
            {
                string slft = lft.ToString();
                string srght = rght.ToString();
                switch (operation.Type)
                {
                    case TokenType.PLUS:
                        return slft + srght;
                    case TokenType.MINUS:
                        string str = slft;
                        return str.Replace(srght, "");
                    case TokenType.MULTIPLICATION:
                        string result = "";
                        for (int i = 0; i < srght.Length; i++)
                            result += slft + srght;
                        return result;
                    default:
                        throw new Exception($"НЕПОДДЕРЖИВАЕМАЯ БИНАРНАЯ ОПЕРАЦИЯ ДЛЯ СТРОКИ: {lft} {operation.Type} {rght} | {left} {operation} {right}");
                }
            }
            else switch (operation.Type)
            {
                case TokenType.PLUS:
                    if (lft is double || rght is double) 
                        return Convert.ToDouble(lft) + Convert.ToDouble(rght);
                    return Convert.ToInt64(lft) + Convert.ToInt64(rght);
                case TokenType.MINUS:
                    if (lft is double || rght is double)
                        return Convert.ToDouble(lft) - Convert.ToDouble(rght);
                    return Convert.ToInt64(lft) - Convert.ToInt64(rght);
                case TokenType.MULTIPLICATION:
                    if (lft is double || rght is double)
                        return Convert.ToDouble(lft) * Convert.ToDouble(rght);
                    return Convert.ToInt64(lft) * Convert.ToInt64(rght);
                case TokenType.DIVISION:
                    if (Convert.ToDouble(rght) != 0)
                    {
                        if (lft is double || rght is double)
                            return Convert.ToDouble(lft) / Convert.ToDouble(rght);
                        return Convert.ToInt64(lft) / Convert.ToInt64(rght);
                    }
                    throw new Exception("ЧЕРТИЛА НА 0 ДЕЛИШЬ");
                case TokenType.POWER:
                    if (lft is double || rght is double)
                        if (Convert.ToDouble(lft) < 0 && rght is double)
                            throw new Exception($"НЕЛЬЗЯ ПРИ ВОЗВЕДЕНИИ В СТЕПЕНЬ ОРИЦАТЕЛЬНОГО ЧИСЛА ИСПОЛЬЗОВАТЬ В СТЕПЕНИ НЕ ЦЕЛОЕ ЧИСЛО:\n{lft}/{operation.Type}/{rght}/{left}/{operation}/{right}");
                        else
                            return Math.Pow(Convert.ToDouble(lft), Convert.ToDouble(rght));
                    return Convert.ToInt64(Math.Pow(Convert.ToDouble(lft), Convert.ToDouble(rght)));
                case TokenType.MOD:
                    if (lft is double || rght is double)
                        return Convert.ToDouble(lft) % Convert.ToDouble(rght);
                    return Convert.ToInt64(lft) % Convert.ToInt64(rght);
                case TokenType.DIV:
                    if (lft is double || rght is double)
                        return Convert.ToDouble(lft) / Convert.ToDouble(rght);
                    return Convert.ToInt64(lft) / Convert.ToInt64(rght);
                default:
                    throw new Exception($"НЕПОДДЕРЖИВАЕМАЯ БИНАРНАЯ ОПЕРАЦИЯ: {lft} {operation.Type} {rght} | {left} {operation} {right}");
            }
        }

        public override string ToString()
        {
            return $"{left} {operation.View} {right};";
        }
    }

    public sealed class CmpExpression : IExpression
    {
        public IExpression left;
        public Token comparation;
        public IExpression right;

        public CmpExpression(IExpression left, Token comparation, IExpression right)
        {
            this.left = left;
            this.comparation = comparation;
            this.right = right;
        }

        public object Evaluated()
        {
            object olft = left.Evaluated();
            object orght = right.Evaluated();
            if (olft is string || orght is string) 
            {
                string slft = left.ToString();
                string srght = right.ToString();
                int slftl = slft.Length;
                int srghtl = srght.Length;
                switch (comparation.Type)
                {
                    case TokenType.EQUALITY:
                        return slft == srght;
                    case TokenType.NOTEQUALITY:
                        return slft != srght;
                    case TokenType.LESS:
                        return slftl < srghtl;
                    case TokenType.LESSEQ:
                        return slftl <= srghtl;
                    case TokenType.MORE:
                        return slftl > srghtl;
                    case TokenType.MOREEQ:
                        return slftl >= srghtl;
                    case TokenType.AND:
                        return slftl > 0 && srghtl > 0;
                    case TokenType.OR:
                        return slftl > 0 || srghtl > 0;
                    default:
                        throw new Exception($"ТАК НЕЛЬЗЯ СРАВНИВАТЬ СТРОКИ: {left}{comparation.ToString()}{right}");
                }
            }
            if (!(olft is bool) && !(orght is bool))
            {
                double lft = Convert.ToDouble(olft);
                double rght = Convert.ToDouble(orght);
                switch (comparation.Type)
                {
                    case TokenType.EQUALITY:
                        return lft == rght;
                    case TokenType.NOTEQUALITY:
                        return lft != rght;
                    case TokenType.LESS:
                        return lft < rght;
                    case TokenType.LESSEQ:
                        return lft <= rght;
                    case TokenType.MORE:
                        return lft > rght;
                    case TokenType.MOREEQ:
                        return lft >= rght;
                    case TokenType.AND:
                        return lft != 0 && rght != 0;
                    case TokenType.OR:
                        return lft != 0 || rght != 0;
                    default:
                        throw new Exception($"НЕСРАВНЕННЫЕ ЧИСЛА: {lft} {comparation.Type} {rght} | {left}{comparation}{right}");
                }
            }
            else if (olft is bool && orght is bool)
            {
                bool lft = Convert.ToBoolean(olft);
                bool rght = Convert.ToBoolean(orght);
                switch (comparation.Type)
                {
                    case TokenType.EQUALITY:
                        return lft == rght;
                    case TokenType.NOTEQUALITY:
                        return lft != rght;
                    case TokenType.AND:
                        return lft && rght;
                    case TokenType.OR:
                        return lft || rght;
                    default:
                        throw new Exception($"НЕСРАВНЕННЫЕ УСЛОВИЯ: {lft} {comparation.Type} {rght} | {left}{comparation}{right}");
                }
            }
            throw new Exception($"НЕЛЬЗЯ СРАВНИВАТЬ РАЗНЫЕ ТИПЫ: {olft} {comparation.Type} {orght} | {olft.GetType()}{comparation.View}{orght.GetType()}");
        }

        public override string ToString()
        {
            return $"{left} {comparation.View} {right}";
        }
    }

    public sealed class VariableExpression : IExpression
    {
        public string Name;
        public object Value;

        public VariableExpression(Token varivable)
        {
            Name = varivable.View;
            Value = Objects.GetVariable(Name);
        }

        public object Evaluated() 
        { 
            return Objects.GetVariable(Name); 
        }

        public override string ToString()
        {
            return $"{Name} ИМЕЕТ ЗНАЧЕНИЕ {Value}";
        }
    }

    public sealed class IncDecBefore : IExpression, IStatement
    {
        public Token Operation;
        public string Name;

        public IncDecBefore(Token operation, string name)
        {
            Operation = operation;
            Name = name;
        }

        public object Evaluated()
        {
            object value = Objects.GetVariable(Name);
            if (value is long || value is bool)
            {
                long temp = value is bool ? Convert.ToBoolean(value) ? 1 : 0 : Convert.ToInt64(value);
                switch (Operation.Type)
                {
                    case TokenType.PLUSPLUS:
                        Objects.AddVariable(Name, ++temp);
                        return temp;
                    case TokenType.MINUSMINUS:
                        Objects.AddVariable(Name, --temp);
                        return temp;
                    default:
                        throw new Exception("НЕВОЗМОЖНО");
                }
            }
            if (value is double)
            {
                double temp = Convert.ToDouble(value);
                switch (Operation.Type)
                {
                    case TokenType.PLUSPLUS:
                        Objects.AddVariable(Name, ++temp);
                        return temp;
                    case TokenType.MINUSMINUS:
                        Objects.AddVariable(Name, --temp);
                        return temp;
                    default:
                        throw new Exception("НЕВОЗМОЖНО");
                }
            }
            throw new Exception($"С ДАННЫМ ЗНАЧЕНИЕМ {value} ДАННОЕ ДЕЙСТВИЕ ({Operation.View}) НЕВОЗМОЖНО");
        }

        public void Execute()
        {
            object value = Objects.GetVariable(Name);
            if (value is long || value is bool)
            {
                long temp = value is bool ? Convert.ToBoolean(value) ? 1 : 0 : Convert.ToInt64(value);
                switch (Operation.Type)
                {
                    case TokenType.PLUSPLUS:
                        Objects.AddVariable(Name, ++temp);
                        return;
                    case TokenType.MINUSMINUS:
                        Objects.AddVariable(Name, --temp);
                        return;
                    default:
                        throw new Exception("НЕВОЗМОЖНО");
                }
            }
            if (value is double)
            {
                double temp = Convert.ToDouble(value);
                switch (Operation.Type)
                {
                    case TokenType.PLUSPLUS:
                        Objects.AddVariable(Name, ++temp);
                        return;
                    case TokenType.MINUSMINUS:
                        Objects.AddVariable(Name, --temp);
                        return;
                    default:
                        throw new Exception("НЕВОЗМОЖНО");
                }
            }
            throw new Exception($"С ДАННЫМ ЗНАЧЕНИЕМ {value} ДАННОЕ ДЕЙСТВИЕ ({Operation.View}) НЕВОЗМОЖНО");
        }

        public override string ToString()
        {
            return '<' + Operation.ToString() + Name + '>';
        }
    }

    public sealed class InputExpression : IExpression
    {
        public string Message;
        public InputExpression() { }

        public InputExpression(string message)
        {
            Message = message;
        }

        public object Evaluated()
        {
            Console.Write(Message??"");
            return Console.ReadLine();
        }

        public override string ToString()
        {
            return $"<{Message??""}>";
        }
    }

    public sealed class FunctionExpression : IExpression 
    {
        public string Name;
        public List<IExpression> Args;

        public FunctionExpression(string name)
        {
            Name = name;
            Args = new List<IExpression>();
        }

        public FunctionExpression(string name, List<IExpression> args) 
        {
            Name = name;
            Args = args;
        }

        public void AddArg(IExpression arg)
        {
            Args.Add(arg);
        }

        public object Evaluated()
        {
            int argov = Args.Count;
            object[] args = new object[argov];
            for (int i = 0; i < argov; i++)
                args[i] = Args[i].Evaluated();
            IFunction function = Objects.GetFunction(Name);
            if (function is UserFunction)
            {
                UserFunction userFunction = (UserFunction)function;
                if (argov != userFunction.ArgsCount())
                    throw new Exception($"НЕВЕРНОЕ КОЛИЧЕСТВО АРГУМЕНТОВ: БЫЛО<{argov}> ОЖИДАЛОСЬ<{userFunction.ArgsCount()}>");
                Objects.Push();
                for (int i = 0; i < argov; i++)
                    Objects.AddVariable(userFunction.GetArgName(i), args[i]);
                object result = userFunction.Execute();
                Objects.Pop();
                return result;
            }
            return function.Execute(args);
        }

        public override string ToString()
        {
            return $"ФУНКЦИЯ {Name}({string.Join(", ", Args.Select(a => a.ToString()))})";
        }
    }

    public sealed class NowExpression : IExpression
    {
        public double Time;
        public object Evaluated()
        {
            Time = (double)DateTime.Now.Ticks / 10000;
            return Time;
        }

        public override string ToString()
        {
            return $"СЕЙЧАС<{Time}>";
        }
    }
}
