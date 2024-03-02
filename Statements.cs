using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
            if (value is List<object>)
                Console.WriteLine(ListString((List<object>)value));
            else if (value is bool)
                Console.WriteLine((bool)value ? "Истина" : "Ложь");
            else
                Console.WriteLine(value);
        }

        public static string ListString(List<object> list)
        {
            string text = "[";
            foreach (object item in list)
            {
                if (item is List<object>)
                    text += ListString((List<object>)item);
                else if (item is bool)
                    text += (bool)item ? "Истина" : "Ложь";
                else
                    text += Convert.ToString(item);

                if (item != list.Last())
                    text += ", ";
            }
            return text + ']';
        }

        public override string ToString()
        {
            return $"НАЧЕРТАТЬ {Expression};";
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
                catch (BreakStatement)
                {
                    break;
                }
                catch (ContinueStatement)
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
                catch (BreakStatement)
                {
                    break;
                }
                catch (ContinueStatement)
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

    public sealed class BreakStatement : Exception, IStatement
    {
        public void Execute()
        {
            throw this;
        }

        public override string ToString()
        {
            return "ВЫЙТИ;";
        }
    }

    public sealed class ContinueStatement : Exception, IStatement
    {
        public void Execute()
        {
            throw this;
        }

        public override string ToString()
        {
            return "ПРОДОЛЖИТЬ;";
        }
    }

    public sealed class ReturnStatement : Exception, IStatement
    {
        public IExpression Expression;
        public object Value;

        public ReturnStatement(IExpression expression)
        {
            Expression = expression;
        }

        public void Execute()
        {
            Value = Expression.Evaluated();
            throw this;
        }

        public object GetResult()
        {
            return Value;
        }

        public override string ToString()
        {
            return $"ВЕРНУТЬ {Value};";
        }
    }

    public sealed class DeclareFunctionStatement : IStatement
    {
        public string Name;
        public string[] Args;
        public IStatement Body;

        public DeclareFunctionStatement(string name, string[] args, IStatement body)
        {
            Name = name;
            Args = args;
            Body = body;
        }

        public void Execute()
        {
            Objects.AddFunction(Name, new UserFunction(Args, Body));
        }

        public override string ToString()
        {
            return $"{Name} => ({string.Join("|", Args)}) {Body};";
        }
    }

    public sealed class ProcedureStatement : IStatement
    {
        public IExpression Function;

        public ProcedureStatement(IExpression function)
        {
            Function = function;
        }

        public void Execute()
        {
            Function.Evaluated();
        }

        public override string ToString()
        {
            return $"ВЫПОЛНИТЬ ПРЕЦЕДУРУ {Function}";
        }
    }

    public sealed class ClearStatement : IStatement
    {
        public void Execute()
        {
            Console.Clear();
        }

        public override string ToString()
        {
            return "ЧИСТКА КОНСОЛИ";
        }
    }

    public sealed class SleepStatement : IStatement
    {
        public int Ms;

        public SleepStatement(int ms)
        {
            Ms = ms;
        }

        public void Execute()
        {
            Thread.Sleep(Ms);
        }

        public override string ToString()
        {
            return $"СОН({Ms})";
        }
    }
}
