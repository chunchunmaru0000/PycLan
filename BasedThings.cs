using System;
using System.Collections.Generic;

namespace PycLan
{
    public static class TypePrint
    {
        public static string Pyc(object value)
        {
            switch (value.GetType().ToString())
            {
                case "System.String":
                    return "СТРОКА";
                case "System.Int32":
                    return "ЧИСЛО 32 ???";
                case "System.Int64":
                    return "ЧИСЛО 64";
                case "System.Double":
                    return "ЧИСЛО С ТОЧКОЙ 64";
                case "System.Boolean":
                    return "ПРАВДИВОСТЬ";
                case "PycLan.UserFunction":
                    return "ПОЛЬЗОВАТЕЛЬСКАЯ ФУНКЦИЯ";
                case "PycLan.Sinus":
                    return "ФУНКЦИЯ СИНУС";
                case "PycLan.Cosinus":
                    return "ФУНКЦИЯ КОСИНУС";
                case "PycLan.Ceiling":
                    return "ФУНКЦИЯ ПОТОЛОК";
                case "PycLan.Floor":
                    return "ФУНКЦИЯ ЗАЗЕМЬ";
                case "PycLan.Tan":
                    return "ФУНКЦИЯ ТАНГЕНС";
                case "PycLan.FunctionExpression":
                    return "ФУНКЦИЯ";
                case "PycLan.Max":
                    return "ФУНКЦИЯ НАИБОЛЬШЕЕ";
                case "PycLan.Min":
                    return "НАИМЕНЬШЕЕ";
                case "System.Collections.Generic.List`1[System.Object]":
                    return "СПИСОК";
                case "[System.Collections.Generic.List`1[PycLan.IExpression]]":
                    return "СПИСОК";
                case "System.Object[]":
                    return "СПИСОК";
                default:
                    return value.GetType().ToString();
                    //throw new Exception($"НЕ ПОМНЮ ЧТО БЫ ДОБАЛЯЛ ТАКОЙ ТИП: <{value.GetType().Name}> У <{value}>");
            }
        }
    }
    public class StringValueAttribute : Attribute
    {
        public string Value { get; }

        public StringValueAttribute(string value)
        {
            Value = value;
        }
    }
    public static class EnumExtensions
    {
        public static string GetStringValue(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
            return attribs.Length > 0 ? attribs[0].Value : null;
        }
    }

    public enum TokenType
    {
        //base types
        [StringValue("КОНЕЦ ФАЙЛА")]
        EOF,
        [StringValue("СЛОВО")]
        WORD,
        [StringValue("СТРОКА")]
        STRING,
        [StringValue("БУЛ")]
        BOOLEAN,

        [StringValue("ЧИСЛО")]
        NUMBER,
        [StringValue("ЦЕЛОЕ ЧИСЛО64")]
        INTEGER,
        [StringValue("НЕ ЦЕЛОЕ ЧИСЛО64")]
        DOUBLE,

        //operators
        [StringValue("ПЛЮС")]
        PLUS,
        [StringValue("МИНУС")]
        MINUS,
        [StringValue("УМНОЖЕНИЕ")]
        MULTIPLICATION,
        [StringValue("ДЕЛЕНИЕ")]
        DIVISION,
        [StringValue("СДЕЛАТЬ РАВНЫМ")]
        DO_EQUAL,
        [StringValue("СТРЕЛКА")]
        ARROW,
        [StringValue("БЕЗ ОСТАТКА")]
        DIV,
        [StringValue("ОСТАТОК")]
        MOD,
        [StringValue("СТЕПЕНЬ")]
        POWER,
        [StringValue("+=")]
        PLUSEQ,
        [StringValue("-=")]
        MINUSEQ,
        [StringValue("*=")]
        MULEQ,
        [StringValue("/=")]
        DIVEQ,

        //cmp
        [StringValue("РАВЕН")]
        EQUALITY,
        [StringValue("НЕ РАВЕН")]
        NOTEQUALITY,
        [StringValue(">")]
        MORE,
        [StringValue(">=")]
        MOREEQ,
        [StringValue("<")]
        LESS,
        [StringValue("<=")]
        LESSEQ,
        [StringValue("НЕ")]
        NOT,
        [StringValue("И")]
        AND,
        [StringValue("ИЛИ")]
        OR,

        //other
        [StringValue("ПЕРЕМЕННАЯ")]
        VARIABLE,
        [StringValue("ФУНКЦИЯ")]
        FUNCTION,
        [StringValue(";")]
        SEMICOLON,
        [StringValue(":")]
        COLON,
        [StringValue("++")]
        PLUSPLUS,
        [StringValue("--")]
        MINUSMINUS,
        [StringValue(",")]
        COMMA,

        [StringValue(")")]
        RIGHTSCOB,
        [StringValue("(")]
        LEFTSCOB,
        [StringValue("]")]
        RCUBSCOB,
        [StringValue("[")]
        LCUBSCOB,
        [StringValue("}")]
        RTRISCOB,
        [StringValue("{")]
        LTRISCOB,

        [StringValue("ПЕРЕНОС")]
        SLASH_N,
        [StringValue("ЦИТАТА")]
        COMMENTO,
        [StringValue("ПУСТОТА")]
        WHITESPACE,
        [StringValue("СОБАКА")]
        DOG,
        [StringValue("КАВЫЧКА")]
        QUOTE,
        [StringValue("ТОЧКА")]
        DOT,
        [StringValue("ЗНАК ВОПРОСА")]
        QUESTION,

        //words types
        [StringValue("ЕСЛИ")]
        WORD_IF,
        [StringValue("ИНАЧЕ")]
        WORD_ELSE,
        [StringValue("ИНАЧЕЛИ")]
        WORD_ELIF,
        [StringValue("ПОКА")]
        WORD_WHILE,
        [StringValue("НАЧЕРТАТЬ")]
        WORD_PRINT,
        [StringValue("ДЛЯ")]
        WORD_FOR,
        [StringValue("ИСТИНА")]
        WORD_TRUE,
        [StringValue("ЛОЖЬ")]
        WORD_FALSE,
        [StringValue("ПРОДОЛЖИТЬ")]
        CONTINUE,
        [StringValue("ВЫЙТИ")]
        BREAK,
        [StringValue("НАПИСТЬ")]
        INPUT,
        [StringValue("ВЕРНУТЬ")]
        RETURN,
        [StringValue("ВЫПОЛНИТЬ ПРОЦЕДУРУ")]
        PROCEDURE,
        [StringValue("СЕЙЧАС")]
        NOW,
        [StringValue("ЧИСТКА")]
        CLEAR,
        [StringValue("СОН")]
        SLEEP
    }

    public class Token
    {
        public string View { get; set; }
        public object Value { get; set; }
        public TokenType Type { get; set; }

        public override string ToString()
        {
            return $"<{View}> <{Convert.ToString(Value)}> <{Type.GetStringValue()}>"; 
        }
    }

    public interface IExpression
    {
        object Evaluated();

        string ToString();
    }

    public interface IStatement
    {
        void Execute();

        string ToString();
    }

    public interface IFunction
    {
        object Execute(params object[] obj);
    }

    public class Objects
    {
        /*        VARIABLES          */

        public static object NOTHING = (long)0; // need improving i believe
        public static Stack<Dictionary<string, object>> Registers = new Stack<Dictionary<string, object>>();
        public static Dictionary<string, object> Variables = new Dictionary<string, object>()
        {
            { "ПИ", Math.PI },
            { "Е", Math.E }
        };

        public static bool ContainsVariable(string key)
        {
            return Variables.ContainsKey(key);
        }

        public static object GetVariable(string key)
        {
            if (ContainsVariable(key))
                return Variables[key];
            return NOTHING;
        }

        public static void AddVariable(string key, object value)
        {
            if (Variables.ContainsKey(key))
                Variables[key] = value;
            else
                Variables.Add(key, value);
        }

        public static void Push()
        {
            Registers.Push(new Dictionary<string, object>(Variables));
        }

        public static void Pop()
        {
            Variables = Registers.Pop();
        }

        /*        FUNCTIONS          */

        public static IFunction DO_NOTHING;
        public static IFunction Sinus = new Sinus();
        public static IFunction Cosinus = new Cosinus();
        public static IFunction Ceiling = new Ceiling();
        public static IFunction Floor = new Floor();
        public static IFunction Tan = new Tan();
        public static IFunction Max = new Max();
        public static IFunction Min = new Min();

        public static Dictionary<string, IFunction> Functions = new Dictionary<string, IFunction>()
        {
            { "синус", Sinus },
            { "косинус", Cosinus },
            { "потолок", Ceiling },
            { "заземь", Floor },
            { "тангенс", Tan },
            { "макс",  Max },
            { "максимум",  Max },
            { "наибольшее",  Max },
            { "большее",  Max },
            { "меньшее",  Min },
            { "мин",  Min },
            { "наименьшее",  Min },
            { "минимум",  Min },
        };

        public static bool ContainsFunction(string key)
        {
            return Functions.ContainsKey(key);
        }

        public static IFunction GetFunction(string key)
        {
            if (ContainsFunction(key))
                return Functions[key];
            return DO_NOTHING;
        }

        public static void AddFunction(string key, IFunction value)
        {
            if (Functions.ContainsKey(key))
                Functions[key] = value;
            else
                Functions.Add(key, value);
        }
    }
}
