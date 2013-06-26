using System;

namespace Edge.Tokens
{
   
    public class SymbolToken : IToken
    {

        private char symbol;

        public SymbolToken(char symbol)
        {
            this.symbol = symbol;
        }

        public override bool Equals(object obj)
        {
            var token = obj as SymbolToken;
            if (token != null && token.symbol == this.symbol)
                return true;

            return false;
        }

        public override string ToString()
        {
            return "Symbol: " + symbol;
        }

        public char Symbol
        {
            get
            {
                return symbol;
            }
        }

    }

}
