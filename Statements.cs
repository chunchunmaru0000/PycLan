using System;
using System.Collections.Generic;
using System.Linq;

namespace PycLan
{
    class AssignStatement : IStatement
    {
        public string Variable;
        public IExpression Expression;

        public AssignStatement(string variable, IExpression expression) 
        { 
            Variable = variable;
            Expression = expression;
        }

        public void Execute()
        {
            object result = Expression.Evaluated();
            Objects.AddVariable(Variable, result);
        }

        public override string ToString()
        {
            return $"{Variable} = {Expression};";
        }
    }

    class PrintStatement : IStatement
    {
        public IExpression Expression;

        public PrintStatement(IExpression expression)
        {
            Expression = expression;
        }

        public void Execute()
        {
            object value = Expression.Evaluated();
            if (value is bool)
                Console.WriteLine((bool)value ? "Истина" : "Ложь");
            else
                Console.WriteLine(value);
        }

        public override string ToString()
        {
            return $"НАЧРЕТАТЬ {Expression};";
        }
    }

    class IfStatement : IStatement
    {
        public IExpression Expression;
        public IStatement IfPart;
        public IStatement ElsePart;
        public IfStatement(IExpression expression, IStatement ifStatement, IStatement elseStatement)
        {
            Expression = expression;
            IfPart = ifStatement;
            ElsePart = elseStatement;
        }

        public void Execute()
        {
            bool result = Convert.ToBoolean(Expression.Evaluated());
            if (result)
                IfPart.Execute();
            else if (ElsePart != null)
                ElsePart.Execute();
        }

        public override string ToString()
        {
            return $"ЕСЛИ {Expression} ТОГДА {{{IfPart}}} ИНАЧЕ {{{ElsePart}}}";
        }
    }

    class BlockStatement : IStatement
    {
        public List<IStatement> Statements;

        public BlockStatement()
        {
            Statements = new List<IStatement>();
        }

        public void Execute()
        {
            foreach (IStatement statement in Statements)
                statement.Execute();
        }

        public void AddStatement(IStatement statement)
        {
            Statements.Add(statement);
        }

        public override string ToString()
        {
            return string.Join("|", Statements.Select(s =>'<' + s.ToString() + '>').ToArray());
        }
    }

    class WhileStatement : IStatement
    {
        IExpression Expression;
        IStatement Statement;

        public WhileStatement(IExpression expression, IStatement statement)
        {
            Expression = expression;
            Statement = statement;
        }

        public void Execute()
        {
            while(Convert.ToBoolean(Expression.Evaluated()))
                Statement.Execute();
        }

        public override string ToString()
        {
            return $"{Expression}: {{{Statement}}}";
        }
    }

    class ForStatement : IStatement
    {
        IStatement Definition;
        IExpression Condition;
        IStatement Alter;
        IStatement Statement;

        public ForStatement(IStatement definition, IExpression condition, IStatement alter, IStatement statement)
        {
            Definition = definition;
            Condition = condition;
            Alter = alter;
            Statement = statement;
        }

        public void Execute()
        {
            for (Definition.Execute(); Convert.ToBoolean(Condition.Evaluated()); Alter.Execute())
                Statement.Execute();
        }

        public override string ToString()
        {
            return $"ДЛЯ {Definition} {Condition} {Alter}: {Statement}";
        }
    }
}
