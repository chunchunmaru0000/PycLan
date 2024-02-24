using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace PycLan
{
    public class Parser
    {
        public Token[] tokens;
        public int lenght;
        public int position;
        public static Token Mul = new Token() { View = "*", Value = null, Type = TokenType.MULTIPLICATION };

        public Parser(Token[] tokens) 
        {
            this.tokens = tokens;
            lenght = tokens.Length;
            position = 0;
        }

        private Token Get(int offset)
        {
            if (position + offset < lenght)
                return tokens[position + offset];
            return tokens.Last();
        }

        private Token Current => Get(0);

        private Token Consume(TokenType type)
        {
            Token current = Current;
            if (Current.Type != type)
                throw new Exception($"ТОКЕН НЕ СОВПАДАЕТ С ОЖИДАЕМЫМ\nОЖИДАЛСЯ: {type}\nТЕКУЩИЙ: {Current.Type}");
            position++;
            return current;
        }

        private bool Match(TokenType type)
        {
            if (Current.Type != type)
                return false;
            position++;
            return true;
        }

        private bool Match(TokenType type0, TokenType type1)
        {
            if (Current.Type != type0 && Current.Type != type1)
                return false;
            position++;
            return true;
        }

        private IExpression ProceedMul(ref IExpression expression)
        {
            while (true)
            {
                Token now = Current;
                if (Match(TokenType.INTEGER, TokenType.DOUBLE))
                {
                    expression = new BinExpression(expression, Mul, new NumExpression(now.Value));
                    continue;
                }
                if (Match(TokenType.VARIABLE))
                {
                    expression = new BinExpression(expression, Mul, new VariableExpression(now));
                    continue;
                }
                if (Match(TokenType.LEFTSCOB))
                {
                    IExpression result = Expression();
                    Match(TokenType.RIGHTSCOB);
                    expression = new BinExpression(expression, Mul, result);
                    continue;
                }
                break;
            }
            return expression;
        }

        private IExpression Primary()
        {
            Token current = Current;
            if (Match(TokenType.INTEGER, TokenType.DOUBLE))
            {
                IExpression number = new NumExpression(current.Value);
                return ProceedMul(ref number);
            }
            if (Match(TokenType.LEFTSCOB))
            {
                IExpression result = Expression();
                Match(TokenType.RIGHTSCOB);
                return ProceedMul(ref result);
            }
            if (Match(TokenType.VARIABLE))
            {
                IExpression variable = new VariableExpression(current);
                return ProceedMul(ref variable);
            }
            throw new Exception($"НЕВОЗМОЖНОЕ МАТЕМАТИЧЕСКОЕ ВЫРАЖЕНИЕ: {current.Value}/{current.View}/{current.Type}");
        }

        private IExpression Unary()
        {
            Token current = Current;
            if (Match(TokenType.PLUS))
                return new NumExpression(current.Value);
            if (Match(TokenType.MINUS))
                return new UnaryExpression(current, Primary());
            return Primary();
        }

        private IExpression Powy()
        {
            IExpression result = Unary();
            Token current = Current;
            while (true)
            {
                if (Match(TokenType.POWER))
                {
                    result = new BinExpression(result, current, Unary());
                    continue;
                }
                break;
            }
            return result;
        }

        private IExpression Mody()
        {
            IExpression result = Powy();
            Token current = Current;
            while (true)
            {
                if (Match(TokenType.MOD) || Match(TokenType.DIV))
                {
                    result = new BinExpression(result, current, Powy());
                    continue;
                }
                break;
            }
            return result;
        }

        private IExpression Muly()
        {
            IExpression result = Mody();
            Token current = Current;
            while (true)
            {
                if (Match(TokenType.MULTIPLICATION) || Match(TokenType.DIVISION))
                {
                    result = new BinExpression(result, current, Mody());
                    continue;
                }
                break;
            }
            return result;
        }

        private IExpression Addity()
        {
            IExpression result = Muly();
            Token current = Current;
            while (true)
            {
                if (Match(TokenType.PLUS) || Match(TokenType.MINUS))
                {
                    result = new BinExpression(result, current, Muly());
                    continue;
                }
                break;
            }
            return result;
        }

        private IExpression Expression()
        {
            return Addity();
        }

        private IStatement Statement()
        {
            Token current = Current;
            if (current.Type == TokenType.VARIABLE && Get(1).Type == TokenType.DO_EQUAL)
            {
                Consume(TokenType.VARIABLE);
                Consume(TokenType.DO_EQUAL);
                return new AssignStatement(current.View, Expression());
            }
            if (current.Type == TokenType.INTEGER || current.Type == TokenType.DOUBLE || current.Type == TokenType.VARIABLE)
                return new PrintNumberStatement(Expression());

            throw new Exception($"НЕИЗВЕСТНОЕ ДЕЙСТВИЕ: {current.View}/{current.Value}/{current.Type}");
        }

        public IStatement[] Parse()
        {
            List<IStatement> parsed = new List<IStatement>();
            while (!Match(TokenType.EOF))
            {
                parsed.Add(Statement());
                parsed.Last().Execute();
                Consume(TokenType.SEMICOLON);
            }
            return parsed.ToArray();
        }
    }
}
