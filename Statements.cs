using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Console.WriteLine(Expression.Evaluated());
        }
    }

    class IfStatement : IStatement
    {
        public IExpression Expression;
        public IStatement ifStatement;
        public IStatement elseStatement;
        public IfStatement(IExpression expression, IStatement ifStatement, IStatement elseStatement)
        {
            Expression = expression;
            this.ifStatement = ifStatement;
            this.elseStatement = elseStatement;
        }

        public void Execute()
        {
            if (Convert.ToBoolean(Expression.Evaluated()))
                ifStatement.Execute();
            else if (!(elseStatement == null))
            {
                elseStatement.Execute();
            }
        }
    }

    class BlockStatement : IStatement
    {
        public List<IStatement> Statements;

        public BlockStatement(List<IStatement> statements)
        {
            Statements = statements;
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
    }
}
