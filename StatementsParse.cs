namespace PycLan
{
    public partial class Parser
    {
        private IStatement Assigny(Token current)
        {
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
            Consume(TokenType.VARIABLE);
            Consume(TokenType.DO_EQUAL);
            IStatement alter = new AssignStatement(current.View, Expression());

            IStatement statement = OneOrBlock();
            return new ForStatement(definition, condition, alter, statement);
        }
    }
}
