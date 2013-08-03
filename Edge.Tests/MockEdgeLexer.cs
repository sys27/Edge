using Edge.Tokens;
using System;
using System.Collections.Generic;

namespace Edge.Tests
{
   
    public class MockEdgeLexer : ILexer
    {

        private IEnumerable<IToken> tokens;

        public MockEdgeLexer()
        {
            
        }

        public MockEdgeLexer(IEnumerable<IToken> tokens)
        {
            this.tokens = tokens;
        }

        public IEnumerable<IToken> Tokenize(string text)
        {
            return tokens;
        }

        public IEnumerable<IToken> Tokens
        {
            get
            {
                return tokens;
            }
            set
            {
                tokens = value;
            }
        }

    }

}
