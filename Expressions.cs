﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PycLan
{
    public sealed class NumExpression : IExpression
    {
        public Token Value { get; set; }

        public NumExpression(Token value)
        {
            Value = value;
        }

        public object Evaluated()
        {
            return Value.Value;
        } 

        public override string ToString()
        {
            object value = Value.Value;
            if (value is List<object>)
                return $"[{string.Join(", ", value)}]";
            if (value is bool)
                return (bool)value ? "Истина" : "Ложь";
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
                    if (lft is List<object>)
                        if (rght is List<object>)
                        {
                            ((List<object>)lft).AddRange((List<object>)rght);
                            return lft;
                        }
                        else
                        {
                            ((List<object>)lft).Add(rght);
                            return lft;
                        }
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
                    throw new Exception($"НЕПОДДЕРЖИВАЕМАЯ БИНАРНАЯ ОПЕРАЦИЯ: <{lft}> <{operation.Type.GetStringValue()}> <{rght}> | <{left}> <{operation}> <{right}>");
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
                        throw new Exception($"ТАК НЕЛЬЗЯ СРАВНИВАТЬ СТРОКИ: <{left}> <{comparation}> <{right}>");
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
                        throw new Exception($"НЕСРАВНЕННЫЕ ЧИСЛА: <{lft}> <{comparation.Type.GetStringValue()}> <{rght}> | <{left}> <{comparation}> <{right}>");
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
                        throw new Exception("НЕСРАВНЕННЫЕ УСЛОВИЯ: <" + (lft ? "Истина" : "Ложь") + $"> <{comparation.Type.GetStringValue()}> <" + (rght ? "Истина" : "Ложь") + $"> | <{left}> <{comparation}> <{right}>");
                }
            }
            throw new Exception($"НЕЛЬЗЯ СРАВНИВАТЬ РАЗНЫЕ ТИПЫ: <{left}> <{comparation}> <{right}>");
        }

        public override string ToString()
        {
            return $"{left} {comparation.View} {right}";
        }
    }

    public sealed class ShortIfExpression : IExpression
    {
        IExpression Condition;
        IExpression Pravda;
        IExpression Nepravda;

        public ShortIfExpression(IExpression condition, IExpression pravda, IExpression nepravda)
        {
            Condition = condition;
            Pravda = pravda;
            Nepravda = nepravda;
        }

        public object Evaluated()
        {
            bool condition = Convert.ToBoolean(Condition.Evaluated());
            if (condition)
                return Pravda.Evaluated();
            else
                return Nepravda.Evaluated();
        }

        public override string ToString()
        {
            return $"({Condition} ? {Pravda} : {Nepravda})";
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
            if (Value is List<object>)
                return $"[{string.Join(", ", Value)}]";
            return $"{Name} ИМЕЕТ ЗНАЧЕНИЕ {Value}";
        }
    }

    public sealed class IncDecBeforeExpression : IExpression, IStatement
    {
        public Token Operation;
        public Token Name;

        public IncDecBeforeExpression(Token operation, Token name)
        {
            Operation = operation;
            Name = name;
        }

        public object Evaluated()
        {
            string name = Name.View;
            object value = Objects.GetVariable(name);
            if (value is long || value is bool)
            {
                long temp = value is bool ? Convert.ToBoolean(value) ? 1 : 0 : Convert.ToInt64(value);
                switch (Operation.Type)
                {
                    case TokenType.PLUSPLUS:
                        Objects.AddVariable(name, ++temp);
                        return temp;
                    case TokenType.MINUSMINUS:
                        Objects.AddVariable(name, --temp);
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
                        Objects.AddVariable(name, ++temp);
                        return temp;
                    case TokenType.MINUSMINUS:
                        Objects.AddVariable(name, --temp);
                        return temp;
                    default:
                        throw new Exception("НЕВОЗМОЖНО");
                }
            }
            throw new Exception($"С ДАННЫМ ЗНАЧЕНИЕМ {value} ДАННОЕ ДЕЙСТВИЕ ({Operation.View}) НЕВОЗМОЖНО");
        }

        public void Execute()
        {
            string name = Name.View;
            object value = Objects.GetVariable(name);
            if (value is long || value is bool)
            {
                long temp = value is bool ? Convert.ToBoolean(value) ? 1 : 0 : Convert.ToInt64(value);
                switch (Operation.Type)
                {
                    case TokenType.PLUSPLUS:
                        Objects.AddVariable(name, ++temp);
                        return;
                    case TokenType.MINUSMINUS:
                        Objects.AddVariable(name, --temp);
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
                        Objects.AddVariable(name, ++temp);
                        return;
                    case TokenType.MINUSMINUS:
                        Objects.AddVariable(name, --temp);
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
        public IExpression Message;
        public InputExpression() { }

        public InputExpression(IExpression message)
        {
            Message = message;
        }

        public object Evaluated()
        {
            string message;
            if (Message != null)
                message = Message.Evaluated().ToString();
            else
                message = "";
            Console.Write(message);
            return Console.ReadLine();
        }

        public override string ToString()
        {
            return $"<{Message.Evaluated().ToString()??""}>";
        }
    }

    public sealed class FunctionExpression : IExpression 
    {
        public Token Name;
        public List<IExpression> Args;

        public FunctionExpression(Token name)
        {
            Name = name;
            Args = new List<IExpression>();
        }

        public FunctionExpression(Token name, List<IExpression> args) 
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
            IFunction function = Objects.GetFunction(Name.View);
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
            if (!(function == null))
                return function.Execute(args);
            else
                throw new Exception($"НЕСУЩЕСТВУЮЩАЯ ФУНКЦИЯ ХОТЯ БЫ СЕЙЧАС: <{Name.View}>");
        }

        public override string ToString()
        {
            return $"ФУНКЦИЯ {Name.View}({string.Join(", ", Args.Select(a => a.ToString()))})";
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

    public sealed class ListTakeExpression : IExpression
    {
        public Token Arr;
        public IExpression From;
        public IExpression To = null;

        public ListTakeExpression(Token arr, IExpression from, IExpression to)
        {
            Arr = arr;
            From = from;
            To = to;
        }

        public string SliceString(string Slice)
        {
            try
            {
                int from = Convert.ToInt32(From.Evaluated());
                int to = 0;
                if (To != null)
                    to = Convert.ToInt32(To.Evaluated());
                int length = Slice.Length;
                if (from < 0)
                    from = length + from + 1;
                if (To != null)
                {
                    if (to < 0)
                        to = length + to + 1;
                    return Slice.Substring(from, to - from);
                }
                return Slice[from] + "";
            }
            catch (Exception)
            {
                int from = Convert.ToInt32(From.Evaluated());
                int to = Convert.ToInt32(To.Evaluated());
                throw new Exception($"НЕКОРРЕКТНЫЕ ИНДЕКСЫ: ОТ <{from}> ДО <{to}> С ДЛИНОЙ <{to - from}>");
            }
        }

        public object Evaluated()
        {
            if (Arr.Type == TokenType.STRING)
                return SliceString(Arr.View);

            object sliced = Objects.GetVariable(Arr.View);
            if (sliced is string)
                return SliceString(Convert.ToString(sliced));

            int from = Convert.ToInt32(From.Evaluated());
            int to = 0;
            if (To != null)
                to = Convert.ToInt32(To.Evaluated());
            List<object> arr = (List<object>)sliced;

            int length = arr.Count;
            if (from < 0)
                from = length + from + 1;
            if (To != null)
            {
                if (to < 0)
                    to = length + to + 1;
                return arr.Skip(from).Take(to - from).ToList();
            }
            return arr[from];
        }

        public override string ToString()
        {
            int from = Convert.ToInt32(From.Evaluated());
            int to = Convert.ToInt32(To.Evaluated());
            return $"{Arr.View}[{from}" + to??"" + "]";
        }
    }

    public sealed class ListExpression : IExpression
    {
        public List<IExpression> Items;

        public ListExpression(List<IExpression> items)
        {
            Items = items;
        }

        public object Evaluated()
        {
            List<object> items = new List<object>(Items.Count);
            foreach (IExpression expression in Items)
                items.Add(expression.Evaluated());
            return items;
        }

        public override string ToString()
        {
            return "СПИСОК";
        }
    }

    public sealed class SplitExpression : IExpression
    {
        Token Variable;
        IExpression Separator;

        public SplitExpression(Token variable, IExpression separator)
        {
            Variable = variable;
            Separator = separator;
        }

        public object Evaluated()
        {
            string stroka = Variable.View;
            char separator = Convert.ToString(Separator.Evaluated())[0];
            return stroka.Split(separator).Select(s => (object)s).ToList();
        }

        public override string ToString()
        {
            return $"{Variable.View}.РАЗДЕЛ({Separator})";
        }
    }
}
