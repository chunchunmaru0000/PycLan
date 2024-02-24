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

    class PrintNumberStatement : IStatement
    {
        public IExpression Expression;

        public PrintNumberStatement(IExpression expression)
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
}
