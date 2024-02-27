using System;
using System.Linq;

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
                throw new Exception($"ТОКЕН НЕ СОВПАДАЕТ С ОЖИДАЕМЫМ\nОЖИДАЛСЯ: {type}\nТЕКУЩИЙ: {Current.Type}\nПОЗИЦИЯ: КОМАНДА<{line}> СЛОВО<{position}>");
            position++;
            return current;
        }

        private Token Consume(TokenType type0, TokenType type1)
        {
            Token current = Current;
            if (Current.Type != type0 && Current.Type != type1)
                throw new Exception($"ТОКЕН НЕ СОВПАДАЕТ С ОЖИДАЕМЫМ\nОЖИДАЛСЯ: {type0} ИЛИ {type1}\nТЕКУЩИЙ: {Current.Type}\nПОЗИЦИЯ: КОМАНДА<{line}> СЛОВО<{position}>");
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
                   type == TokenType.STRING     ||
                   type == TokenType.LEFTSCOB   ||
                   type == TokenType.INPUT      ;
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

        private IExpression Expression()
        {
            return Ory();
        }

        private IStatement Statement()
        {
            line++;
            Token current = Current;
            if (current.Type == TokenType.VARIABLE && Get(1).Type == TokenType.DO_EQUAL)
                return Assigny();

            if (current.Type == TokenType.PLUSPLUS || current.Type == TokenType.MINUSMINUS && Get(1).Type == TokenType.VARIABLE)
                return BeforeIncDecy();

            if (Match(TokenType.WORD_IF))
                return IfElsy();

            if (Match(TokenType.WORD_WHILE))
                return Whily();

            if (Match(TokenType.BREAK))
                return Breaky();

            if (Match(TokenType.CONTINUE))
                return Continuy();

            if (Match(TokenType.WORD_FOR))
                return Fory();

            if (Match(TokenType.WORD_PRINT))
                return Printy();

            if (Printble(current.Type))
                return Printy();

            throw new Exception($"НЕИЗВЕСТНОЕ ДЕЙСТВИЕ: {current.toString()}\nПОЗИЦИЯ: ДЕЙСТВИЕ<{line}> СЛОВО<{position}>");
        }

        private IStatement Block()
        {
            BlockStatement block = new BlockStatement();
            while (!Match(TokenType.RTRISCOB))
                block.AddStatement(Statement());
            return block;
        }

        public IStatement Parse()
        {
            BlockStatement parsed = new BlockStatement();
            while(!Match(TokenType.EOF))
                parsed.Statements.Add(Statement());
            return parsed;
        }

        public void Run()
        {
            while (!Match(TokenType.EOF))
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                IStatement statement = Statement();
                Console.WriteLine(statement.ToString());
                Console.ForegroundColor = ConsoleColor.Yellow;
                if (!(statement is BlockStatement))
                    statement.Execute();
            }
        }
    }
}
