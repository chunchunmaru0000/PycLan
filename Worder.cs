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

                case "харатья":
                    word.Type = TokenType.INPUT;
                    return word;
                case "хартия":
                    word.Type = TokenType.INPUT;
                    return word;
                case "ввод":
                    word.Type = TokenType.INPUT;
                    return word;
                case "ввести":
                    word.Type = TokenType.INPUT;
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
                default:
                    word.Type = TokenType.VARIABLE;
                    return word;
            }
        }
    }
}
