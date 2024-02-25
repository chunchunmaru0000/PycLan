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
    public partial class Parser
    {
        public Token[] tokens;
        public int lenght;
        public int position;
        public int line = 0;
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
                throw new Exception($"ТОКЕН НЕ СОВПАДАЕТ С ОЖИДАЕМЫМ\nОЖИДАЛСЯ: {type}\nТЕКУЩИЙ: {Current.Type}\nПОЗИЦИЯ: ЛИНИЯ<{line}> СИМВОЛ<{position}>");
            position++;
            return current;
        }

        private bool Printble(TokenType type)
        {
            return type == TokenType.INTEGER    ||
                   type == TokenType.DOUBLE     ||
                   type == TokenType.VARIABLE   ||
                   type == TokenType.WORD_TRUE  ||
                   type == TokenType.WORD_FALSE ||
                   type == TokenType.PLUS       ||
                   type == TokenType.MINUS      ||
                   type == TokenType.NOT        ||
                   type == TokenType.STRING     ;
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

        private IExpression Primary()
        {
            Token current = Current;
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
            {
                return new VariableExpression(current);
            }
            throw new Exception($"НЕВОЗМОЖНОЕ МАТЕМАТИЧЕСКОЕ ВЫРАЖЕНИЕ: <{current.toString()}>\nПОЗИЦИЯ: ЛИНИЯ<{line}> СИМВОЛ<{position}>");
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

        private IExpression Expression()
        {
            return Booly();
        }

        private IStatement Statement()
        {
            line++;
            Token current = Current;
            if (current.Type == TokenType.VARIABLE && Get(1).Type == TokenType.DO_EQUAL)
            {
                Consume(TokenType.VARIABLE);
                Consume(TokenType.DO_EQUAL);
                IStatement result = new AssignStatement(current.View, Expression());
                Consume(TokenType.SEMICOLON);
                return result;
            }
            
            if (Match(TokenType.WORD_IF))
            {
                IExpression condition = Expression();
                List<IStatement> ifStatements = new List<IStatement>();
                List<IStatement> elseStatements = new List<IStatement>();

                if (Match(TokenType.LTRISCOB))
                    while (!Match(TokenType.RTRISCOB))
                        ifStatements.Add(Statement());
                else
                {
                    Consume(TokenType.COLON);
                    ifStatements.Add(Statement());
                }

                if (Match(TokenType.WORD_ELSE))
                {
                    if (Match(TokenType.LTRISCOB))
                        while (!Match(TokenType.RTRISCOB))
                            elseStatements.Add(Statement());
                    else
                    {
                        Consume(TokenType.COLON);
                        elseStatements.Add(Statement());
                    }
                }
                IStatement ifStatement = new BlockStatement(ifStatements);
                IStatement elseStatement = elseStatements.Count() > 0 ? new BlockStatement(elseStatements) : null;
                IStatement result = new IfStatement(condition, ifStatement, elseStatement);
                return result;
            }

            if (Match(TokenType.WORD_PRINT))
            {
                IStatement statement = new PrintStatement(Expression());
                Consume(TokenType.SEMICOLON);
                return statement;
            }
            if (Printble(current.Type))
            {
                IStatement statement = new PrintStatement(Expression());
                Consume(TokenType.SEMICOLON);
                return statement;
            }

            throw new Exception($"НЕИЗВЕСТНОЕ ДЕЙСТВИЕ: {current.toString()}\nПОЗИЦИЯ: ЛИНИЯ<{line}> СИМВОЛ<{position}>");
        }

        private IStatement Block()
        {
            return null;
        }

        public IStatement[] Parse()
        {
            List<IStatement> parsed = new List<IStatement>();
            while (!Match(TokenType.EOF))
            {
                parsed.Add(Statement());
                parsed.Last().Execute();
            }
            return parsed.ToArray();
        }
    }
}
