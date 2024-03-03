﻿using System;
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
                Match(TokenType.COLON);
                return Statement();
            }
        }

        private IStatement Assigny()
        {
            Token current = Current;
            Consume(TokenType.VARIABLE);
            Consume(TokenType.DO_EQUAL);
            IExpression expression = Expression();
            IStatement result = new AssignStatement(current, expression);
            Match(TokenType.SEMICOLON, TokenType.COMMA);
            return result;
        }

        private IStatement ItemAssigny()
        {
            Token variable = Current;
            Consume(TokenType.VARIABLE);

            Consume(TokenType.LCUBSCOB);
            IExpression index = Expression();
            Consume(TokenType.RCUBSCOB);

            Consume(TokenType.DO_EQUAL);
            IExpression value = Expression();
            Match(TokenType.SEMICOLON, TokenType.COMMA);
            return new ItemAssignStatement(variable, index, value);
        }

        private IStatement Printy()
        {
            IStatement print = new PrintStatement(Expression());
            Match(TokenType.SEMICOLON);
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
            IStatement definition = Assigny();

            IExpression condition = Expression();
            Match(TokenType.SEMICOLON, TokenType.COMMA);

            Token current = Current;
            IStatement alter = null;
            if (Match(TokenType.VARIABLE))
            {
                Consume(TokenType.DO_EQUAL);
                alter = new AssignStatement(current, Expression());
            }
            else if (Current.Type == TokenType.PLUSPLUS || Current.Type == TokenType.MINUSMINUS)
            {
                Token operation = Current;
                Consume(TokenType.PLUSPLUS, TokenType.MINUSMINUS);
                Token name = Current;
                Consume(TokenType.VARIABLE);
                alter = new IncDecBeforeExpression(operation, name);
            }

            IStatement body = OneOrBlock();
            return new ForStatement(definition, condition, alter, body);
        }

        public IStatement BeforeIncDecy()
        {
            Token current = Current;
            Consume(TokenType.PLUSPLUS, TokenType.MINUSMINUS);
            Token name = Current;
            Consume(TokenType.VARIABLE);
            IStatement statement = new IncDecBeforeExpression(current, name);
            Match(TokenType.SEMICOLON);
            return statement;
        }

        public IStatement Breaky()
        {
            IStatement statement = new BreakStatement();
            Match(TokenType.SEMICOLON);
            return statement;
        }

        public IStatement Continuy()
        {
            IStatement statement = new ContinueStatement();
            Match(TokenType.SEMICOLON, TokenType.COMMA);
            return statement;
        }

        public IStatement Returny() 
        {
            IExpression expression = Expression();
            Match(TokenType.SEMICOLON, TokenType.COMMA);
            return new ReturnStatement(expression);
        }

        public IStatement Functiony()
        {
            Token name = Current;
            Consume(TokenType.VARIABLE);
            List<Token> args = new List<Token>();
            Consume(TokenType.ARROW);
            Consume(TokenType.LEFTSCOB);
            while (!Match(TokenType.RIGHTSCOB))
            {
                args.Add(Consume(TokenType.VARIABLE));
                Match(TokenType.COMMA, TokenType.SEMICOLON);
            }
            IStatement body = OneOrBlock();
            return new DeclareFunctionStatement(name, args.ToArray(), body);
        }

        public IStatement Procedury()
        {
            IExpression expression = Expression();
            Match(TokenType.SEMICOLON, TokenType.COMMA);
            return new ProcedureStatement(expression);
        }

        public IStatement Cleary()
        {
            Match(TokenType.SEMICOLON, TokenType.COMMA);
            return new ClearStatement();
        }

        public IStatement Sleepy()
        {
            Consume(TokenType.LEFTSCOB);
            IExpression ms = Expression();
            Consume(TokenType.RIGHTSCOB);
            Match(TokenType.SEMICOLON, TokenType.COMMA);
            return new SleepStatement(ms);
        }
    }
}
