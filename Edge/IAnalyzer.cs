using Edge.SyntaxNodes;
using System;
using System.Collections.Generic;
namespace Edge
{
    
    public interface IAnalyzer
    {

        RootNode MatchTypes(RootNode rootNode);

    }

}
