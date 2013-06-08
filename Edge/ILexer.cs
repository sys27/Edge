using Edge.Tokens;
using System;
using System.Collections.Generic;

namespace Edge
{

    public interface ILexer
    {

        IEnumerable<IToken> Tokenize(string text);

    }

}
