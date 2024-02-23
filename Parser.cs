using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace PycLan
{
    public class Parser
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

        private bool Match(TokenType type0, TokenType type1)
        {
            if (Current.Type != type0 && Current.Type != type1)
                return false;
            position++;
            return true;
        }

        private IExpression Primary()
        {
            if (Match(TokenType.INTEGER, TokenType.FLOAT))
                return new NumExpression(Current.Value);
            Console.WriteLine($"### {Current.Value} {Current.View} {Current.Type}");
            throw new Exception("НЕВОЗМОЖНОЕ ВЫРАЖЕНИЕ");
        }

        private IExpression Unary()
        {
            IExpression result = Primary();
            if (Match(TokenType.MINUS))
                result = new UnaryExpression(Current, result);
            return result;
        }

        private IExpression Muly()
        {
            IExpression result = Unary();
            while (true)
            {
                if (Match(TokenType.MULTIPLICATION))
                {
                    result = new BinExpression(result, new Token() { Type = TokenType.MULTIPLICATION }, Unary());
                    continue;
                }
                if (Match(TokenType.DIVISION))
                {
                    result = new BinExpression(result, new Token() { Type = TokenType.DIVISION }, Unary());
                    continue;
                }
                if (Match(TokenType.POWER))
                {
                    result = new PowerExpression(result, Unary());
                    continue;
                }
                if (Match(TokenType.MOD))
                {
                    result = new ModExpression(result, Unary());
                    continue;
                }
                if (Match(TokenType.DIV))
                {
                    result = new DivExpression(result, Unary());
                    continue;
                }
                break;
            }
            return result;
        }

        private IExpression Addity()
        {
            IExpression result = Unary();
            while (true)
            {
                if (Match(TokenType.PLUS))
                {
                    result = new BinExpression(result, new Token() { Type = TokenType.PLUS }, Muly());
                    continue;
                }
                if (Match(TokenType.MINUS))
                {
                    result = new BinExpression(result, new Token() { Type = TokenType.MINUS }, Muly());
                    continue;
                }
                break;
            }
            return result;
        }

        private IExpression Expression()
        {
            return Addity();
        }

        public IExpression[] Parse()
        {
            List<IExpression> parsed = new List<IExpression>();
            while (!Match(TokenType.EOF))
                parsed.Add(Expression());
            return parsed.ToArray();
        }
    }
}
