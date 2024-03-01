using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;

namespace PycLan
{
    public partial class Parser
    {
        private IExpression Primary()
        {
            Token current = Current;
            if (current.Type == TokenType.VARIABLE && Get(1).Type == TokenType.LEFTSCOB || current.Type == TokenType.FUNCTION)
            {
                string name = current.View;
                Consume(TokenType.FUNCTION, TokenType.VARIABLE);
                Consume(TokenType.LEFTSCOB);
                FunctionExpression function = new FunctionExpression(name);
                while (!Match(TokenType.RIGHTSCOB))
                {
                    function.AddArg(Expression());
                    Match(TokenType.COMMA, TokenType.SEMICOLON);
                }
                return function;
            }
            if (Match(TokenType.NOW))
                return new NowExpression();
            if (Match(TokenType.INPUT))
            {
                if (Match(TokenType.LEFTSCOB))
                {
                    string message = Expression().Evaluated().ToString();
                    Consume(TokenType.RIGHTSCOB);
                    return new InputExpression(message);
                }
                return new InputExpression();
            }
            if (Match(TokenType.STRING))
                return new NumExpression(current.Value);
            if (Match(TokenType.WORD_TRUE, TokenType.WORD_FALSE))
                return new NumExpression(current.Value);
            if (Match(TokenType.INTEGER, TokenType.DOUBLE))
                return new NumExpression(current.Value);
            if (Match(TokenType.LEFTSCOB))
            {
                IExpression result = Expression();
                Match(TokenType.RIGHTSCOB);
                return result;
            }
            if (Match(TokenType.VARIABLE))
                return new VariableExpression(current);
            if (Match(TokenType.PLUSPLUS, TokenType.MINUSMINUS))
            {
                string name = Current.View;
                IExpression result = new IncDecBefore(current, name);
                Consume(TokenType.VARIABLE);
                return result;
            }
            throw new Exception($"НЕВОЗМОЖНОЕ МАТЕМАТИЧЕСКОЕ ВЫРАЖЕНИЕ: <{current.ToString()}>\nПОЗИЦИЯ: ЛИНИЯ<{line}> СИМВОЛ<{position}>");
        }

        private IExpression Unary()
        {
            Token current = Current;
            if (Match(TokenType.NOT))
                return new UnaryExpression(current, Primary());
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
            while (true)
            {
                Token current = Current;
                if (Match(TokenType.MULTIPLICATION) || Match(TokenType.DIVISION))
                {
                    result = new BinExpression(result, current, Mody());
                    continue;
                }
                if (current.Type == TokenType.INTEGER || current.Type == TokenType.DOUBLE)
                {
                    result = new BinExpression(result, Mul, Mody());
                    continue;
                }
                if (current.Type == TokenType.VARIABLE)
                {
                    result = new BinExpression(result, Mul, Mody());
                    continue;
                }
                if (Match(TokenType.LEFTSCOB))
                {
                    IExpression expression = Expression();
                    Match(TokenType.RIGHTSCOB);
                    result = new BinExpression(result, Mul, expression);
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

        private IExpression Booly()
        {
            IExpression result = Addity();
            Token current = Current;
            while (true)
            {
                if (Match(TokenType.EQUALITY, TokenType.NOTEQUALITY))
                {
                    result = new CmpExpression(result, current, Addity());
                    continue;
                }
                if (Match(TokenType.MORE, TokenType.MOREEQ))
                {
                    result = new CmpExpression(result, current, Addity());
                    continue;
                }
                if (Match(TokenType.LESS, TokenType.LESSEQ))
                {
                    result = new CmpExpression(result, current, Addity());
                    continue;
                }
                break;
            }
            return result;
        }

        private IExpression Andy()
        {
            IExpression result = Booly();
            Token current = Current;
            while (true)
            {
                if (Match(TokenType.AND))
                {
                    result = new CmpExpression(result, current, Booly());
                }
                break;
            }
            return result;
        }

        private IExpression Ory()
        {
            IExpression result = Andy();
            Token current = Current;
            while (true)
            {
                if (Match(TokenType.OR))
                {
                    result = new CmpExpression(result, current, Andy());
                }
                break;
            }
            return result;
        }
    }
}
