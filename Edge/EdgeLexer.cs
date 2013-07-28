using Edge.Tokens;
using System;
using System.Collections.Generic;

namespace Edge
{

    public class EdgeLexer : ILexer
    {

        public IEnumerable<IToken> Tokenize(string text)
        {
            var tokens = new List<IToken>();

            for (int i = 0; i < text.Length; )
            {
                i++;
            }

            return tokens;
        }

    }

}
