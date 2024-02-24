using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

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
            switch (Operation.Type) 
            {
                case TokenType.PLUS:
                    return Value.Evaluated();
                case TokenType.MINUS:
                    object value = Value.Evaluated();
                    if (value is int)
                        return -(int)value;
                    else 
                        return -(double)value;
                default:
                    Console.WriteLine($"value {Value.Evaluated()}|Value {Value}|op {Operation.Type}|Op {Operation.View}");
                    throw new Exception("ДА КАК ТАК ВООБЩЕ ВОЗМОЖНО ЧТО ЛИБО ПОСТАВИТЬ КРОМЕ + ИЛИ - ПЕРЕД ЧИСЛОМ");
            }
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
            switch (operation.Type)
            {
                case TokenType.PLUS:
                    if (lft is double || rght is double) 
                        return Convert.ToDouble(lft) + Convert.ToDouble(rght);
                    return (int)lft + (int)rght;
                case TokenType.MINUS:
                    if (lft is double || rght is double)
                        return Convert.ToDouble(lft) - Convert.ToDouble(rght);
                    return (int)lft - (int)rght;
                case TokenType.MULTIPLICATION:
                    if (lft is double || rght is double)
                        return Convert.ToDouble(lft) * Convert.ToDouble(rght);
                    return (int)lft * (int)rght;
                case TokenType.DIVISION:
                    if (Convert.ToDouble(rght) != 0)
                    {
                        if (lft is double || rght is double)
                            return Convert.ToDouble(lft) / Convert.ToDouble(rght);
                        return (int)lft / (int)rght;
                    }
                    throw new Exception("ЧЕРТИЛА НА 0 ДЕЛИШЬ");
                case TokenType.POWER:
                    if (lft is double || rght is double)
                        return Math.Pow(Convert.ToDouble(lft), Convert.ToDouble(rght));
                    return Convert.ToInt32(Math.Pow(Convert.ToDouble(lft), Convert.ToDouble(rght)));
                case TokenType.MOD:
                    if (lft is double || rght is double)
                        return Convert.ToDouble(lft) % Convert.ToDouble(rght);
                    return (int)lft % (int)rght;
                case TokenType.DIV:
                    if (lft is double || rght is double)
                        return Convert.ToDouble(lft) / Convert.ToDouble(rght);
                    return (int)lft / (int)rght;
                default:
                    throw new Exception($"НЕПОДДЕРЖИВАЕМАЯ БИНАРНАЯ ОПЕРАЦИЯ {lft} {operation.Type} {rght}\n{left}{operation}{right}");
            }
        }
    }

    public sealed class BoolExpression : IExpression
    {
        public bool Value { get; set; }

        public BoolExpression(object value)
        {
            if (!(value is bool))
            {
                if (value is double || value is int)
                    Value = Convert.ToDouble(value) != 0;
                else throw new Exception($"ЭТО КАК {Value} {value}");
            }
            Value = (bool)value;
        }

        public object Evaluated()
        {
            return Value;
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
            object lft = left.Evaluated();
            object rght = right.Evaluated();
            switch (comparation.Type)
            {
                case TokenType.EQUALITY:
                    return lft == rght;
                case TokenType.NOTEQUALITY:
                    return lft != rght;
                default:
                    throw new Exception($"НЕСРАВНЕННО {lft} {comparation.Type} {rght}\n{left}{comparation}{right}");
            }
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
            return Value; 
        }
    }
}
