using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace PycLan
{
    class Parser
    {
        public Token[] tokens;
        public int lenght;
        public int position;

        public Parser(Token[] tokens) 
        {
            this.tokens = tokens;
            lenght = tokens.Length;
            position = 0;
        }

        private Token Get(int offset)
        {
            if (position + offset < lenght)
                return tokens[position + offset];
            return tokens.Last();
        }

        private Token Current => Get(0); 

        private bool Match(TokenType type)
        {
            if (Current.Type != type)
                return false;
            position++;
            return true;
        }

        private NumExpression Primary()
        {
            if (Match(TokenType.NUMBER))
                return new NumExpression(Current.Value);
            throw new Exception("НЕВОЗМОЖНОЕ ВЫРАЖЕНИЕ");
        }

        private IExpression Unary()
        {
            NumExpression num = Primary();
            if (Match(TokenType.MINUS))
                return new UnaryExpression(Current, num);
            return num;
        }

        private NumExpression Muly()
        {
            IExpression result = Unary();
            while (true)
            {
                if (Match(TokenType.MULTIPLICATION))
                {
                    result = new BinExpression((NumExpression)result, new Token() { Type = TokenType.MULTIPLICATION }, (NumExpression)Unary());
                    continue;
                }
                if (Match(TokenType.DIVISION))
                {
                    result = new BinExpression((NumExpression)result, new Token() { Type = TokenType.DIVISION }, (NumExpression)Unary());
                    continue;
                }
                if (Match(TokenType.POWER))
                {
                    result = new PowerExpression((NumExpression)result, (NumExpression)Unary());
                    continue;
                }
                if (Match(TokenType.MOD))
                {
                    result = new ModExpression((NumExpression)result, (NumExpression)Unary());
                    continue;
                }
                if (Match(TokenType.DIV))
                {
                    result = new DivExpression((NumExpression)result, (NumExpression)Unary());
                    continue;
                }
                break;
            }
            return (NumExpression)result;
        }



        private List<IExpression> Parse()
        {

            return null;
        }
    }
}
