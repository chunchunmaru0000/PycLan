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
}
