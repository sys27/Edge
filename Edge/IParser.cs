using Edge.SyntaxNodes;
using Edge.Tokens;
using System;
using System.Collections.Generic;

namespace Edge
{
    
    public interface IParser
    {

        RootNode Parse(IEnumerable<IToken> tokens);

    }

}
