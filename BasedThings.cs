﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        NOT,

        //other
        SEMICOLON,
        PLUSPLUS,
        MINUSMINUS,

        RIGHTSCOB,
        LEFTSCOB,
        RCUBSCOB,
        LCUBSCOB,
        RTRISCOB,
        LTRISCOB,

        WHITESPACE,
        EXCLAMATION,
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
}
