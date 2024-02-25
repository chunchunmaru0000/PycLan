using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PycLan
{
    public enum TokenType
    {
        //base types
        EOF,
        WORD,
        STRING,
        BOOLEAN,

        NUMBER,
        INTEGER,
        DOUBLE,

        //operators
        PLUS,
        MINUS,
        MULTIPLICATION,
        DIVISION,
        DO_EQUAL,
        DIV,
        MOD,
        POWER,

        //cmp
        EQUALITY,
        NOTEQUALITY,
        MORE,
        MOREEQ,
        LESS,
        LESSEQ,
        NOT,

        //other
        VARIABLE, // not now
        SEMICOLON,
        PLUSPLUS,
        MINUSMINUS,
        COMMA,

        RIGHTSCOB,
        LEFTSCOB,
        RCUBSCOB,
        LCUBSCOB,
        RTRISCOB,
        LTRISCOB,

        SLASH_N,
        COMMENTO,
        WHITESPACE,
        DOG,
        QUOTE,
        DOT,

        //words types
        WORD_IF,
        WORD_ELSE,
        WORD_WHILE,
        WORD_PRINT,
        WORD_FOR,
        WORD_TRUE,
        WORD_FALSE
    }

    public class Token
    {
        public string View { get; set; }
        public object Value { get; set; }
        public TokenType Type { get; set; }
    }

    public interface IExpression
    {
        object Evaluated();
    }

    public interface IStatement
    {
        void Execute();
    }

    public static class Objects
    {
        public static Dictionary<string, object> Variables = new Dictionary<string, object>()
        {
            { "пи", Math.PI }
        };

        public static bool ContainsVariable(string key)
        {
            return Variables.ContainsKey(key);
        }

        public static object GetVariable(string key)
        {
            if (ContainsVariable(key))
                return Variables[key];
            throw new Exception($"НЕТ ТАКОЙ ПЕРЕМЕННОЙ В ДАННЫЙ МОМЕНТ ХОТЯ БЫ: {key}");
        }

        public static void AddVariable(string key, object value)
        {
            if (Variables.ContainsKey(key))
                Variables[key] = value;
            else
                Variables.Add(key, value);
        } 
    }
}
