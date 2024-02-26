using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PycLan
{
    static class Worder
    {
        public static Token Wordizator(Token word)
        {
            switch (word.View.ToLower())
            {
                case "если":
                    word.Type = TokenType.WORD_IF;
                    return word;
                case "коль":
                    word.Type = TokenType.WORD_IF;
                    return word;
                case "ежели":
                    word.Type = TokenType.WORD_IF;
                    return word;
                case "иначе":
                    word.Type = TokenType.WORD_ELSE;
                    return word;
                case "иначели":
                    word.Type = TokenType.WORD_ELIF;
                    return word;
                case "пока":
                    word.Type = TokenType.WORD_WHILE;
                    return word;
                case "для":
                    word.Type = TokenType.WORD_FOR;
                    return word;
                case "начертать":
                    word.Type = TokenType.WORD_PRINT;
                    return word;
                case "истина":
                    word.Value = true;
                    word.Type = TokenType.WORD_TRUE;
                    return word;
                case "правда":
                    word.Value = true;
                    word.Type = TokenType.WORD_TRUE;
                    return word;
                case "ложь":
                    word.Value = false;
                    word.Type = TokenType.WORD_FALSE;
                    return word;
                default:
                    word.Type = TokenType.VARIABLE;
                    return word;
            }
        }
    }
}
