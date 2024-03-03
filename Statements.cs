using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PycLan
{
    public sealed class AssignStatement : IStatement
    {
        public Token Variable;
        public IExpression Expression;

        public AssignStatement(Token variable, IExpression expression) 
        { 
            Variable = variable;
            Expression = expression;
        }

        public void Execute()
        {
            object result = Expression.Evaluated();
            string name = Variable.View;
            Objects.AddVariable(name, result);
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
                else if (item is string)
                    text += '"' + (string)item + '"';
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
        public Token Name;
        public Token[] Args;
        public IStatement Body;

        public DeclareFunctionStatement(Token name, Token[] args, IStatement body)
        {
            Name = name;
            Args = args;
            Body = body;
        }

        public void Execute()
        {
            Objects.AddFunction(Name.View, new UserFunction(Args, Body));
        }

        public override string ToString()
        {
            return $"{Name} => ({string.Join("|", Args.Select(a => a.View))}) {Body};";
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
        public IExpression Ms;

        public SleepStatement(IExpression ms)
        {
            Ms = ms;
        }

        public void Execute()
        {
            Thread.Sleep(Convert.ToInt32(Ms.Evaluated()));
        }

        public override string ToString()
        {
            return $"СОН({Ms})";
        }
    }

    public sealed class ItemAssignStatement : IStatement
    {
        public Token Variable;
        public IExpression Index;
        public IExpression Expression;

        public ItemAssignStatement(Token variable, IExpression index, IExpression expression)
        {
            Variable = variable;
            Index = index;
            Expression = expression;
        }

        public void Execute()
        {
            string name = Variable.View;
            int index = Convert.ToInt32(Index.Evaluated());
            object value = Expression.Evaluated();
            List<object> list = (List<object>)Objects.GetVariable(name);
            list[index] = value;
            Objects.AddVariable(name, list);
        }

        public override string ToString()
        {
            return $"{Variable.View}[{Index.Evaluated()}] = {Expression.Evaluated()};";
        }
    }

    public sealed class OperationAssignStatement : IStatement
    {
        public Token Variable;
        public Token Operation;
        public IExpression Expression;

        public OperationAssignStatement(Token variable, Token operation, IExpression expression)
        {
            Variable = variable;
            Operation = operation;
            Expression = expression;
        }

        public void Execute()
        {
            string name = Variable.View;
            object value = Expression.Evaluated();
            object variable = Objects.GetVariable(name);
            object result = null;
            switch (Operation.Type)
            {
                case TokenType.PLUSEQ:
                    if (value is bool)
                    {
                        if (variable is long)
                            result = Convert.ToInt64(variable) + ((bool)value ? 1 : 0);
                        else if (variable is double)
                            result = Convert.ToDouble(variable) + ((bool)value ? 1 : 0);
                        else if (variable is string)
                            result = Convert.ToString(variable) + ((bool)value ? 1 : 0);
                    }
                    else if(variable is double || value is double)
                        result = Convert.ToDouble(variable) + Convert.ToDouble(value);
                    else if (variable is string || value is string)
                        result = Convert.ToString(variable) + Convert.ToString(value);
                    else if (variable is long)
                        result = Convert.ToInt64(variable) + Convert.ToInt64(value);
                    else if (variable is List<object>)
                    {
                        if (value is List<object>) 
                            ((List<object>)variable).AddRange((List<object>)value);
                        else
                            ((List<object>)variable).Add(value);
                        result = variable;
                    }
                    else throw new Exception($"НЕДОПУСТИМОЕ ДЕЙСТВИЕ МЕЖДУ: <{variable}>({TypePrint.Pyc(variable)}) И <{value}>({TypePrint.Pyc(value)})");
                    break;
                case TokenType.MINUSEQ:
                    if (variable is double || value is double)
                        result = Convert.ToDouble(variable) - Convert.ToDouble(value);
                    else if (variable is string || value is string)
                        result = Convert.ToString(variable).Replace(Convert.ToString(value), "");
                    else if (variable is long)
                        result = Convert.ToInt64(variable) - Convert.ToInt64(value);
                    else throw new Exception($"НЕДОПУСТИМОЕ ДЕЙСТВИЕ МЕЖДУ: <{variable}>({TypePrint.Pyc(variable)}) И <{value}>({TypePrint.Pyc(value)})");
                    break;
                case TokenType.MULEQ:
                    if (variable is double || value is double)
                        result = Convert.ToDouble(variable) * Convert.ToDouble(value);
                    else if (variable is long)
                        result = Convert.ToInt64(variable) * Convert.ToInt64(value);
                    else throw new Exception($"НЕДОПУСТИМОЕ ДЕЙСТВИЕ МЕЖДУ: <{variable}>({TypePrint.Pyc(variable)}) И <{value}>({TypePrint.Pyc(value)})");
                    break;
                case TokenType.DIVEQ:
                    if (variable is double || value is double || variable is long || value is long)
                        result = Convert.ToDouble(variable) / Convert.ToDouble(value);
                    else throw new Exception($"НЕДОПУСТИМОЕ ДЕЙСТВИЕ МЕЖДУ: <{variable}>({TypePrint.Pyc(variable)}) И <{value}>({TypePrint.Pyc(value)})");
                    break;
                default:
                    throw new Exception($"НЕ МОЖЕТ БЫТЬ: <{name}> <{variable}> <{value}> <{Operation.View}>");
            }
            Objects.AddVariable(name, result);
        }

        public override string ToString()
        {
            return $"{Variable.View} {Operation.View} {Expression}";
        }
    }
}
