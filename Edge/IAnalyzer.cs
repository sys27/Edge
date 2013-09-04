using Edge.SyntaxNodes;
using System;

namespace Edge
{

    public interface IAnalyzer
    {

        void Analyze(SyntaxTree tree);

    }

}
