﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PycLan
{
    public class Tokenizator
    {
        private string code;
        private int position;
        public Tokenizator(string code) 
        {
            this.code = code;
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
                int dots = 0;
                while (char.IsDigit(Current) || Current == '.')
                {
                    if (Current == '.')
                        dots++;
                    Next();
                }
                string word = code.Substring(start, position - start);
                if (dots == 0)
                    return new Token() { View = word, Value = Convert.ToInt32(word), Type = TokenType.INTEGER };
                if (dots == 1)
                    return new Token() { View = word, Value = Convert.ToDouble(word), Type = TokenType.DOUBLE };
                throw new Exception("МНОГА ТОЧЕК ДЛЯ ЧИСЛА");
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
                    if (Current == '=')
                    {
                        Next();
                        return new Token() { View = "==", Value = null, Type = TokenType.EQUALITY };
                    }
                    return new Token() { View = "=", Value = null, Type = TokenType.DO_EQUAL };
                case '/':
                    Next();
                    if (Current == '/')
                    {
                        Next();
                        return new Token() { View = "//", Value = null, Type = TokenType.DIV };
                    }
                    return new Token() { View = "/", Value = null, Type = TokenType.DIVISION };
                case '!':
                    Next();
                    if (Current == '=')
                    {
                        Next();
                        return new Token() { View = "!=", Value = null, Type = TokenType.NOTEQUALITY };
                    }
                    return new Token() { View = "!", Value = null, Type = TokenType.NOT };
                case '*':
                    Next();
                    if (Current == '*')
                    {
                        Next();
                        return new Token() { View = "**", Value = null, Type = TokenType.POWER };
                    }
                    return new Token() { View = "*", Value = null, Type = TokenType.MULTIPLICATION };
                case '+':
                    Next();
                    return new Token() { View = "+", Value = null, Type = TokenType.PLUS };
                case '-':
                    Next();
                    return new Token() { View = "-", Value = null, Type = TokenType.MINUS };
                case '@':
                    Next();
                    return new Token() { View = "@", Value = null, Type = TokenType.DOG };
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
                case '%':
                    Next();
                    return new Token() { View = "%", Value = null, Type = TokenType.MOD };
                default:
                    throw new Exception("НЕ СУЩЕСТВУЮЩИЙ СИМВОЛ В ДАННОМ ЯЗЫКЕ");
            }
        }

        public Token[] Tokenize()
        {
            List<Token> tokens = new List<Token>();
            while (true)
            {
                Token token = NextToken();
                if (token.Type != TokenType.WHITESPACE)
                    tokens.Add(token);
                if (token.Type == TokenType.EOF)
                    break;
            }
            return tokens.ToArray();
        }
    }
}
