using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PycLan
{
    public sealed class AssignStatement : IStatement
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

    public sealed class PrintStatement : IStatement
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

    public sealed class IfStatement : IStatement
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

    public sealed class BlockStatement : IStatement
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

    public sealed class WhileStatement : IStatement
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
            {
                try
                {
                    Statement.Execute();
                }
                catch (BreakStatementException)
                {
                    break;
                }
                catch (ContinueStatementException)
                {
                    // contonue by itself
                }
            }
        }

        public override string ToString()
        {
            return $"{Expression}: {{{Statement}}}";
        }
    }

    public sealed class ForStatement : IStatement
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
            {
                try
                {
                    Statement.Execute();
                }
                catch (BreakStatementException)
                {
                    break;
                }
                catch (ContinueStatementException)
                {
                    // contonue by itself
                }
            }
        }

        public override string ToString()
        {
            return $"ДЛЯ {Definition} {Condition} {Alter}: {Statement}";
        }
    }

    public sealed class BreakStatementException : Exception { }

    public sealed class BreakStatement : Exception, IStatement
    {
        public void Execute()
        {
            throw new BreakStatementException();
        }

        public override string ToString()
        {
            return "ВЫЙТИ;";
        }
    }

    public sealed class ContinueStatementException : Exception { }

    class ContinueStatement : Exception, IStatement
    {
        public void Execute()
        {
            throw new ContinueStatementException();
        }

        public override string ToString()
        {
            return "ПРОДОЛЖИТЬ;";
        }
    }

    class DeclareFunctionStatement : IStatement
    {
        public string Name;
        public Dictionary<string, IExpression> Args;
        public IFunction Body;

        public DeclareFunctionStatement(string name, Dictionary<string, IExpression> args, IFunction body)
        {
            Name = name;
            Args = args;
            Body = body;
        }

        public void Execute()
        {
            Objects.AddFunction(Name, Body);
        }

        public override string ToString()
        {
            return $"{Name} => ({string.Join("|", Args.Keys.Select(a => a).ToArray())}) {Body};";
        }
    }
}
