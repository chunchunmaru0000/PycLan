using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace PycLan
{
    sealed class NumExpression : IExpression
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

    sealed class UnaryExpression : IExpression
    {
        public Token Operation;
        public NumExpression Value;
        
        public UnaryExpression(Token operation, NumExpression value)
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
                        return -(float)value;
                default:
                    throw new Exception("ДА КАК ТАК ВООБЩЕ ВОЗМОЖНО ЧТО ЛИБО ПОСТАВИТЬ КРОМЕ + ИЛИ - ПЕРЕД ЧИСЛОМ");
            }
        }
    }

    sealed class PowerExpression : IExpression
    {
        public NumExpression Value;
        public NumExpression Power;

        public PowerExpression(NumExpression value, NumExpression power)
        {
            Value = value;
            Power = power;
        }

        public object Evaluated()
        {
            if (Value.Evaluated() is int)
                return (int)Math.Pow((double)Value.Evaluated(), (double)Power.Evaluated());
            return (float)Math.Pow((double)Value.Evaluated(), (double)Power.Evaluated());
        }
    }

    sealed class ModExpression : IExpression
    {
        public NumExpression left;
        public NumExpression right;

        public ModExpression(NumExpression left, NumExpression right)
        {
            this.left = left;
            this.right = right;
        }

        public object Evaluated()
        {
            if (left.Evaluated() is float || right.Evaluated() is float)
                return (float)left.Evaluated() % (float)right.Evaluated();
            return (int)left.Evaluated() % (int)right.Evaluated();
        }
    }

    sealed class DivExpression : IExpression
    {
        public NumExpression left;
        public NumExpression right;

        public DivExpression(NumExpression left, NumExpression right)
        {
            this.left = left;
            this.right = right;
        }

        public object Evaluated()
        {
            if (left.Evaluated() is float || right.Evaluated() is float)
                return (float)left.Evaluated() / (float)right.Evaluated();
            return (int)left.Evaluated() / (int)right.Evaluated();
        }
    }

    sealed class BinExpression : IExpression 
    {
        public NumExpression left;
        public Token operation;
        public NumExpression right;

        public BinExpression(NumExpression left, Token operation, NumExpression right)
        {
            this.left = left;
            this.operation = operation;
            this.right = right;
        }

        public object Evaluated()
        {
            switch (operation.Type)
            {
                case TokenType.PLUS:
                    if (left.Value is float || right.Value is float) 
                        return (float)left.Value + (float)right.Value;
                    return (int)left.Value + (int)right.Value;
                case TokenType.MINUS:
                    if (left.Value is float || right.Value is float)
                        return (float)left.Value - (float)right.Value;
                    return (int)left.Value - (int)right.Value;
                case TokenType.MULTIPLICATION:
                    if (left.Value is float || right.Value is float)
                        return (float)left.Value * (float)right.Value;
                    return (int)left.Value * (int)right.Value;
                case TokenType.DIVISION:
                    if ((float)right.Value != 0)
                    {
                        if (left.Value is float || right.Value is float)
                            return (float)left.Value + (float)right.Value;
                        return (int)left.Value + (int)right.Value;
                    }
                    throw new Exception("ЧЕРТИЛА НА 0 ДЕЛИШЬ");
                default:
                    throw new Exception("НЕПОДДЕРЖИВАЕМАЯ ОПЕРАЦИЯ");
            }
        }
    }

    sealed class BoolExpression : IExpression
    {
        public bool Value { get; set; }

        public BoolExpression(object value)
        {
            if (!(value is bool))
            {
                if (value is float || value is int)
                    Value = (float)value == 0;
                else throw new Exception("ЭТО КАК");
            }
            Value = (bool)value;
        }

        public object Evaluated()
        {
            return Value;
        }
    }

    sealed class CmpExpression : IExpression
    {
        public BoolExpression left;
        public Token comparation;
        public BoolExpression right;

        public CmpExpression(BoolExpression left, Token comparation, BoolExpression right)
        {
            this.left = left;
            this.comparation = comparation;
            this.right = right;
        }

        public object Evaluated()
        {
            switch (comparation.Type)
            {
                case TokenType.EQUALITY:
                    return left.Value == right.Value;
                case TokenType.NOTEQUALITY:
                    return left.Value != right.Value;
                default:
                    throw new Exception("НЕСРАВНЕННО");
            }
        }
    }
}
