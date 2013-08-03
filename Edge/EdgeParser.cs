using Edge.SyntaxNodes;
using Edge.Tokens;
using System;
using System.Collections.Generic;

namespace Edge
{

    public class EdgeParser : IParser
    {

        private ILexer lexer;

        public EdgeParser()
            : this(new EdgeLexer())
        {

        }

        public EdgeParser(ILexer lexer)
        {
            this.lexer = lexer;
        }

        private RootNode Root()
        {
            throw new NotImplementedException();
        }

        private IEnumerable<NamespaceNode> Namespaces()
        {
            throw new NotImplementedException();
        }

        private ObjectNode Object()
        {
            throw new NotImplementedException();
        }

        private IEnumerable<PropertyNode> Properties()
        {
            throw new NotImplementedException();
        }

        public RootNode Parse(IEnumerable<IToken> tokens)
        {
            throw new NotImplementedException();
        }

    }
}
