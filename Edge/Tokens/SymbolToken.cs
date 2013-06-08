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

        public char Symbol
        {
            get
            {
                return symbol;
            }
        }

    }

}
