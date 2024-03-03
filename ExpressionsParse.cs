﻿using System;
using System.Collections.Generic;

namespace PycLan
{
    public partial class Parser
    {
        private IExpression FuncParsy()
        {
            Token name = Current;
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

        private IExpression Slicy()
        {
            Token sliced = Current;
            Consume(TokenType.VARIABLE);
            Consume(TokenType.LCUBSCOB);
            IExpression from = Expression();
            if (Match(TokenType.COLON))
            {
                IExpression to = Expression();
                Consume(TokenType.RCUBSCOB);
                return new ListTakeExpression(sliced, from, to);
            }
        /*    if (Match(TokenType.COMMA, TokenType.SEMICOLON))
            {
                List<IExpression> indexes = new List<IExpression>();
                Consume(TokenType.COMMA, TokenType.SEMICOLON);
                while(Current.Type != TokenType.RCUBSCOB)
                {
                    indexes.Add(Expression());
                }

            }*/
            Consume(TokenType.RCUBSCOB);
            return new ListTakeExpression(sliced, from, null);
        }

        private IExpression Listy()
        {
            Consume(TokenType.LCUBSCOB);
            List<IExpression> items = new List<IExpression>();
            while (!Match(TokenType.RCUBSCOB))
            {
                items.Add(Expression());
                Match(TokenType.COMMA, TokenType.SEMICOLON);
            }
            return new ListExpression(items);
        }

        private IExpression Primary()
        {
            Token current = Current;
            if (current.Type == TokenType.STRING && Get(1).Type == TokenType.LCUBSCOB)
               return Slicy();

            if (current.Type == TokenType.VARIABLE && Get(1).Type == TokenType.LCUBSCOB)
                return Slicy();

            if (current.Type == TokenType.VARIABLE && Get(1).Type == TokenType.LEFTSCOB || current.Type == TokenType.FUNCTION)
                return FuncParsy();

            if (current.Type == TokenType.LCUBSCOB)
                return Listy();

            if (Match(TokenType.NOW))
                return new NowExpression();

            if (Match(TokenType.INPUT))
            {
                if (Match(TokenType.LEFTSCOB))
                {
                    IExpression message = Expression();
                    Consume(TokenType.RIGHTSCOB);
                    return new InputExpression(message);
                }
                return new InputExpression();
            }

            if (Match(TokenType.STRING))
                return new NumExpression(current);

            if (Match(TokenType.WORD_TRUE, TokenType.WORD_FALSE))
                return new NumExpression(current);

            if (Match(TokenType.INTEGER, TokenType.DOUBLE))
                return new NumExpression(current);

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
                Token name = Current;
                IExpression result = new IncDecBeforeExpression(current, name);
                Consume(TokenType.VARIABLE);
                return result;
            }

            throw new Exception($"НЕВОЗМОЖНОЕ МАТЕМАТИЧЕСКОЕ ВЫРАЖЕНИЕ: <{current}>\nПОЗИЦИЯ: ЛИНИЯ<{line}> СИМВОЛ<{position}>");
        }

        private IExpression Unary()
        {
            Token current = Current;
            int sign = -1;
            if (Match(TokenType.NOT))
            {
                while (true)
                {
                    current = Current;
                    if (Match(TokenType.NOT))
                    {
                        sign *= -1;
                        continue;
                    }
                    break;
                }
                return sign < 0 ? new UnaryExpression(current, Primary()) : Primary();
            }
            if (Match(TokenType.MINUS, TokenType.PLUS))
                return new UnaryExpression(current, Primary());
            return Primary();
        }

        private IExpression Powy()
        {
            IExpression result = Unary();
            while (true)
            {
                Token current = Current;
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
            while (true)
            {
                Token current = Current;
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
                if (current.Type == TokenType.VARIABLE && Get(1).Type == TokenType.LEFTSCOB || current.Type == TokenType.FUNCTION)
                {
                    result = new BinExpression(result, Mul, FuncParsy());
                    continue;
                }
                break;
            }
            return result;
        }

        private IExpression Addity()
        {
            IExpression result = Muly();
            while (true)
            {
                Token current = Current;
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
            while (true)
            {
                Token current = Current;
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
            while (true)
            {
                Token current = Current;
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
            while (true)
            {
                Token current = Current;
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
