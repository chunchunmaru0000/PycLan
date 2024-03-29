﻿using System.Net;

namespace PycLan
{
    static class Worder
    {
        public static Token Wordizator(Token word)
        {
            if (Objects.ContainsFunction(word.View))
            {
                word.Type = TokenType.FUNCTION;
                return word;
            }
            switch (word.View.ToLower())
            {
                case "егда":
                    word.Type = TokenType.WORD_IF;
                    return word;
                case "еже":
                    word.Type = TokenType.WORD_IF;
                    return word;
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
                case "не":
                    word.Type = TokenType.NOT;
                    return word;

                case "класс":
                    word.Type = TokenType.CLASS;
                    return word;
                case "новый":
                    word.Type = TokenType.NEW;
                    return word;
                case "здесь":
                    word.Type = TokenType.THIS;
                    return word;
                case "этот":
                    word.Type = TokenType.THIS;
                    return word;
                case "овый":
                    word.Type = TokenType.THIS;
                    return word;
                case "свой":
                    word.Type = TokenType.THIS;
                    return word;

                case "пока":
                    word.Type = TokenType.WORD_WHILE;
                    return word;
                case "докамест":
                    word.Type = TokenType.WORD_WHILE;
                    return word;
                case "дондеже":
                    word.Type = TokenType.WORD_WHILE;
                    return word;

                case "для":
                    word.Type = TokenType.WORD_FOR;
                    return word;

                case "начертать":
                    word.Type = TokenType.WORD_PRINT;
                    return word;
                case "епистолия":
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
                case "реснота":
                    word.Value = true;
                    word.Type = TokenType.WORD_TRUE;
                    return word;
                case "аминь":
                    word.Value = true;
                    word.Type = TokenType.WORD_TRUE;
                    return word;

                case "ложь":
                    word.Value = false;
                    word.Type = TokenType.WORD_FALSE;
                    return word;

                case "и":
                    word.Type = TokenType.AND;
                    return word;
                case "&&":
                    word.Type = TokenType.AND;
                    return word;

                case "или":
                    word.Type = TokenType.OR;
                    return word;
                case "||":
                    word.Type = TokenType.OR;
                    return word;

                case "больше":
                    word.Type = TokenType.MORE;
                    return word;
                case "паче":
                    word.Type = TokenType.MORE;
                    return word;
                case "вяще":
                    word.Type = TokenType.MORE;
                    return word;

                case "меньше":
                    word.Type = TokenType.LESS;
                    return word;

                case "продолжить":
                    word.Type = TokenType.CONTINUE;
                    return word;
                case "выйти":
                    word.Type = TokenType.BREAK;
                    return word;

                case "вернуть":
                    word.Type = TokenType.RETURN;
                    return word;
                case "воздать":
                    word.Type = TokenType.RETURN;
                    return word;
                case "пояти":
                    word.Type = TokenType.RETURN;
                    return word;
                case "яти":
                    word.Type = TokenType.RETURN;
                    return word;
                case "чтить":
                    word.Type = TokenType.RETURN;
                    return word;

                case "деяти":
                    word.Type = TokenType.PROCEDURE;
                    return word;
                case "выполнить":
                    word.Type = TokenType.PROCEDURE;
                    return word;
                case "процедура":
                    word.Type = TokenType.PROCEDURE;
                    return word;
                case "процедуру":
                    word.Type = TokenType.PROCEDURE;
                    return word;

                case "сейчас":
                    word.Type = TokenType.NOW;
                    return word;
                case "чистка":
                    word.Type = TokenType.CLEAR;
                    return word;
                case "сон":
                    word.Type = TokenType.SLEEP;
                    return word;
                case "русить":
                    word.Type = TokenType.VOVASCRIPT;
                    return word;

                /*           SQL           */

                case "создать":
                    word.Type = TokenType.CREATE;
                    return word;
                case "бд":
                    word.Type = TokenType.DATABASE;
                    return word;

                case "таблица":
                    word.Type = TokenType.TABLE;
                    return word;
                case "таблицу":
                    word.Type = TokenType.TABLE;
                    return word;
                case "тб":
                    word.Type = TokenType.TABLE;
                    return word;

                case "строчка":
                    word.Type = TokenType.STROKE;
                    return word;
                case "число":
                    word.Type = TokenType.NUMBER;
                    return word;
                case "точка":
                    word.Type = TokenType.FNUMBER;
                    return word;
                case "обманчивость":
                    word.Type = TokenType.BUL;
                    return word;
                case "все":
                    word.Type = TokenType.ALL;
                    return word;
                case "всё":
                    word.Type = TokenType.ALL;
                    return word;

                case "добавить":
                    word.Type = TokenType.INSERT;
                    return word;
                case "в":
                    word.Type = TokenType.IN;
                    return word;
                case "значения":
                    word.Type = TokenType.VALUES;
                    return word;
                case "колонки":
                    word.Type = TokenType.COLONS;
                    return word;
                case "выбрать":
                    word.Type = TokenType.SELECT;
                    return word;
                case "из":
                    word.Type = TokenType.FROM;
                    return word;
                case "от":
                    word.Type = TokenType.AT;
                    return word;
                case "как":
                    word.Type = TokenType.AS;
                    return word;
                case "где":
                    word.Type = TokenType.WHERE;
                    return word;
                default:
                    word.Type = TokenType.VARIABLE;
                    return word;
            }
        }
    }
}
