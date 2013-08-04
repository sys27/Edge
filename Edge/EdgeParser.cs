using Edge.SyntaxNodes;
using Edge.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edge
{

    public class EdgeParser : IParser
    {

        private ILexer lexer;

        private List<IToken> tokens;
        private int position;

        public EdgeParser()
            : this(new EdgeLexer())
        {

        }

        public EdgeParser(ILexer lexer)
        {
            this.lexer = lexer;
        }

        private IToken GetToken()
        {
            var token = tokens[position];
            position++;

            return token;
        }

        private IToken Peek()
        {
            return tokens[position];
        }

        private Type SearchType(string type)
        {
            throw new NotImplementedException();
        }

        private RootNode Root()
        {
            var namespaces = Namespaces();
            var obj = Object();

            return new RootNode(obj, namespaces);
        }

        private IEnumerable<NamespaceNode> Namespaces()
        {
            if (Peek() is UsingToken)
            {
                var namespaces = new List<NamespaceNode>();

                while (GetToken() is UsingToken)
                {
                    namespaces.Add(GetNamespace());
                }

                return namespaces;
            }

            return null;
        }

        private NamespaceNode GetNamespace()
        {
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                var token = GetToken();
                if (token is SymbolToken && ((SymbolToken)token).Symbol == ';')
                    break;

                if (token is WordToken)
                    sb.Append(((WordToken)token).Word);
                else if (token is SymbolToken && ((SymbolToken)token).Symbol == '.')
                    sb.Append('.');
                else
                    // todo: error message 
                    throw new EdgeParserException();
            }

            var ns = sb.ToString();
            if (string.IsNullOrWhiteSpace(ns))
                // todo: error message
                throw new EdgeParserException();

            return new NamespaceNode(ns);
        }

        private ObjectNode Object()
        {
            var token = GetToken();
            if (!(token is TypeToken))
                // todo: error message
                throw new EdgeParserException();

            var type = SearchType(((TypeToken)token).Type);
            //var ctor = Constructor();
            var properties = Properties();

            return new ObjectNode(type, properties);
        }

        private IEnumerable<PropertyNode> Properties()
        {
            var token = Peek();
            if (token is SymbolToken && ((SymbolToken)token).Symbol == '{')
            {
                position++;
                var properties = new List<PropertyNode>();

                while (true)
                {
                    token = Peek();
                    if (token is SymbolToken && ((SymbolToken)token).Symbol == '}')
                    {
                        position++;
                        break;
                    }

                    if (token is PropertyToken)
                        properties.Add(GetProperty());
                    else
                        // todo: error message
                        throw new EdgeParserException();
                }

                if (properties.Count == 0)
                    return null;

                return properties;
            }

            return null;
        }

        private PropertyNode GetProperty()
        {
            throw new NotImplementedException();
        }

        public RootNode Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException("text");

            tokens = lexer.Tokenize(text).ToList();

            return Root();
        }

    }
}
