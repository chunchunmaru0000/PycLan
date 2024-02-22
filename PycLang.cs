using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PycLan.PycLang;

namespace PycLan
{
    public enum TokenType
    {
        //base types
        EOF,
        WORD,
        INTEGER,
            FLOAT,
            BOOLEAN,
        PLUS,
        MINUS,
        MULTIPLICATION,
        DIVISION,
        SEMICOLON,
        RIGHTSCOB,
        LEFTSCOB,
        WHITESPACE,
        EXCLAMATION,
        DOG,
        QUOTE,
        EQUALITY,

        //words types
        WORD_IF,
        WORD_ELSE,
        WORD_WHILE,
        WORD_PRINT,
        WORD_FOR,
        WORD_TRUE,
        WORD_FALSE
    }

    public enum ExpressionType
    {
        CALCULATABLE
    }

    public class Token
    {
        public string View { get; set; }
        public object Value { get; set; }
        public TokenType Type { get; set; }
    }

    public class ExpressionToken : Token
    {
        public Token[] Expression { get; set; }
        public ExpressionType ExpType { get; set; }
    }

    public class PycLang
    {
        public string code;
        public int position = 0;

        public PycLang(string code, int position=0)
        {
            this.code = code;
            this.position = position;
        }

        public void Restart()
        {
            position = 0;
        }

        private char Current
        {
            get
            { 
                if (position < code.Length)
                    return code[position];
                return '\0';
            }
        }

        private void Next() { position++; }

        private Token NextToken()
        {
            if (Current == '\0')
                return new Token() { View = null, Value = null, Type = TokenType.EOF };
            if (char.IsWhiteSpace(Current))
            {
                int start = position;
                while (char.IsWhiteSpace(Current))
                    Next();
                string word = code.Substring(start, position - start);
                return new Token() { View = word, Value = null, Type = TokenType.WHITESPACE };
            }
            if (char.IsDigit(Current))
            {
                int start = position;
                while (char.IsDigit(Current))
                    Next();
                string word = code.Substring(start, position - start);
                return new Token() { View = word, Value = Convert.ToInt32(word), Type = TokenType.INTEGER };
            }
            if (PycTools.Usable(Current))
            {
                int start = position;
                while (PycTools.Usable(Current))
                    Next();
                string word = code.Substring(start, position - start);
                return Worder.Wordizator(new Token() { View = word, Value = null, Type = TokenType.WORD });
            }
            switch (Current)
            {
                case '=':
                    Next();
                    return new Token() { View = "=", Value = null, Type = TokenType.EQUALITY };
                case '+':
                    Next();
                    return new Token() { View = "+", Value = null, Type = TokenType.PLUS };
                case '-':
                    Next();
                    return new Token() { View = "-", Value = null, Type = TokenType.MINUS };
                case '*':
                    Next();
                    return new Token() { View = "*", Value = null, Type = TokenType.MULTIPLICATION };
                case '/':
                    Next();
                    return new Token() { View = "/", Value = null, Type = TokenType.DIVISION };
                case '@':
                    Next();
                    return new Token() { View = "@", Value = null, Type = TokenType.DOG };
                case '!':
                    Next();
                    return new Token() { View = "!", Value = null, Type = TokenType.EXCLAMATION };
                case ';':
                    Next();
                    return new Token() { View = ";", Value = null, Type = TokenType.SEMICOLON };
                case '(':
                    Next();
                    return new Token() { View = "(", Value = null, Type = TokenType.RIGHTSCOB };
                case ')':
                    Next();
                    return new Token() { View = ")", Value = null, Type = TokenType.LEFTSCOB };
                case '"':
                    Next();
                    return new Token() { View = '"' + "", Value = null, Type = TokenType.QUOTE };
                default:
                    throw new Exception("НЕ СУЩЕСТВУЮЩИЙ СИМВОЛ В ДАННОМ ЯЗЫКЕ");
            }
        }

        public Token[] Tokenizator()
        {
            List<Token> tokens = new List<Token>();
            while(true)
            {
                Token token = NextToken();
                tokens.Add(token);
                if (token.Type == TokenType.EOF)
                    break;
            }
            return tokens.ToArray();
        }
        
        public ExpressionToken Expressionizator(Token[] tokens)
        {
            foreach(Token token in tokens)
            {

            }
            throw new NotImplementedException();
        }
    }
}
