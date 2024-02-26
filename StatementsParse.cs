using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            result.Execute();  //
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
    }
}
