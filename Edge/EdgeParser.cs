using Edge.SyntaxNodes;
using Edge.Tokens;
using System;
using System.Collections.Generic;

namespace Edge
{

    public class EdgeParser : IParser
    {

        private ILexer lexer;

        private IEnumerable<IToken> tokens;

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
            var namespaces = Namespaces();
            var obj = Object();

            return new RootNode(obj, namespaces);
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

        public RootNode Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException("text");

            tokens = lexer.Tokenize(text);

            throw new NotImplementedException();
        }

    }
}
