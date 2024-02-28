using System.Collections.Generic;

namespace PycLan
{
    public partial class Parser
    {
        private IStatement OneOrBlock()
        {
            if (Match(TokenType.LTRISCOB))
                return Block();
            else
            {
                Consume(TokenType.COLON);
                return Statement();
            }
        }

        private IStatement Assigny()
        {
            Token current = Current;
            Consume(TokenType.VARIABLE);
            Consume(TokenType.DO_EQUAL);
            IStatement result = new AssignStatement(current.View, Expression());
            Consume(TokenType.SEMICOLON);
            return result;
        }

        private IStatement Printy()
        {
            IStatement print = new PrintStatement(Expression());
            Consume(TokenType.SEMICOLON);
            return print;
        }

        private IStatement IfElsy()
        {
            IExpression condition = Expression();
            IStatement ifStatements = OneOrBlock();
            IStatement elseStatements;
            if (Match(TokenType.WORD_ELSE))
                elseStatements = OneOrBlock();
            else
                elseStatements = null;
            return new IfStatement(condition, ifStatements, elseStatements);
        }

        private IStatement Whily()
        {
            IExpression condition = Expression();
            IStatement statement = OneOrBlock();
            return new WhileStatement(condition, statement);
        }

        private IStatement Fory()
        {
            Token current = Current;
            Consume(TokenType.VARIABLE);
            Consume(TokenType.DO_EQUAL);
            IStatement definition = new AssignStatement(current.View, Expression());
            Consume(TokenType.SEMICOLON, TokenType.COMMA);

            IExpression condition = Expression();
            Consume(TokenType.SEMICOLON, TokenType.COMMA);

            current = Current;

            IStatement alter = null;
            if (Match(TokenType.VARIABLE))
            {
                Consume(TokenType.DO_EQUAL);
                alter = new AssignStatement(current.View, Expression());
            }
            else if (Current.Type == TokenType.PLUSPLUS || Current.Type == TokenType.MINUSMINUS)
            {
                Token operation = Current;
                Consume(TokenType.PLUSPLUS, TokenType.MINUSMINUS);
                string name = Current.View;
                Consume(TokenType.VARIABLE);
                alter = new IncDecBefore(operation, name);
            }

            IStatement statement = OneOrBlock();
            return new ForStatement(definition, condition, alter, statement);
        }

        public IStatement BeforeIncDecy()
        {
            Token current = Current;
            Consume(TokenType.PLUSPLUS, TokenType.MINUSMINUS);
            string name = Current.View;
            Consume(TokenType.VARIABLE);
            IStatement statement = new IncDecBefore(current, name);
            Consume(TokenType.SEMICOLON);
            return statement;
        }

        public IStatement Breaky()
        {
            IStatement statement = new BreakStatement();
            Consume(TokenType.SEMICOLON);
            return statement;
        }

        public IStatement Continuy()
        {
            IStatement statement = new ContinueStatement();
            Consume(TokenType.SEMICOLON);
            return statement;
        }

        public IStatement Returny() 
        {
            IExpression expression = Expression();
            Consume(TokenType.SEMICOLON);
            return new ReturnStatement(expression);
        }

        public IStatement Functiony()
        {
            string name = Current.View;
            Consume(TokenType.VARIABLE);
            List<string> args = new List<string>();
            Consume(TokenType.ARROW);
            Consume(TokenType.LEFTSCOB);
            while (!Match(TokenType.RIGHTSCOB))
            {
                args.Add(Consume(TokenType.VARIABLE).View);
                Match(TokenType.COMMA, TokenType.SEMICOLON);
            }
            IStatement body = OneOrBlock();
            return new DeclareFunctionStatement(name, args.ToArray(), body);
        }
    }
}
