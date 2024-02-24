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
            object value = Value.Evaluated();
            switch (Operation.Type) 
            {
                case TokenType.PLUS:
                    return value;
                case TokenType.MINUS:
                    if (value is int)
                        return -(int)value;
                    else 
                        return -(double)value;
                case TokenType.NOT:
                    return !Convert.ToBoolean(value);
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
                        if (Convert.ToDouble(lft) < 0 && rght is double)
                            throw new Exception($"НЕЛЬЗЯ ПРИ ВОЗВЕДЕНИИ В СТЕПЕНЬ ОРИЦАТЕЛЬНОГО ЧИСЛА ИСПОЛЬЗОВАТЬ В СТЕПЕНИ НЕ ЦЕЛОЕ ЧИСЛО:\n{lft}/{operation.Type}/{rght}/{left}/{operation}/{right}");
                        else
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
                    throw new Exception($"НЕПОДДЕРЖИВАЕМАЯ БИНАРНАЯ ОПЕРАЦИЯ: {lft} {operation.Type} {rght} | {left} {operation} {right}");
            }
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
            if (!(lft is bool))
                lft = Convert.ToDouble(lft);
            object rght = right.Evaluated();
            if (!(rght is bool))
                rght = Convert.ToDouble(rght);
            switch (comparation.Type)
            {
                case TokenType.EQUALITY:
                    return (bool)lft == (bool)rght;
                case TokenType.NOTEQUALITY:
                    return (bool)lft != (bool)rght;
                case TokenType.LESS:
                    return (double)lft < (double)rght;
                case TokenType.LESSEQ:
                    return (double)lft <= (double)rght;
                case TokenType.MORE:
                    return (double)lft > (double)rght;
                case TokenType.MOREEQ:
                    return (double)lft >= (double)rght;
                default:
                    throw new Exception($"НЕСРАВНЕННО: {lft} {comparation.Type} {rght} | {left}{comparation}{right}");
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
