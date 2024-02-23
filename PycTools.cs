using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PycLan
{
    class PycTools
    {
        public static bool Usable (char c) { 
            return c != '+' && c != '-' && c != '*' && c != '/' &&
                   c != '(' && c != ')' && c != '"' && c != '!' && 
                   c != ' ' && c != '@' && c != ';' && c != '=' &&
                   c != '%' && c != '\0';
        }
    }
}
