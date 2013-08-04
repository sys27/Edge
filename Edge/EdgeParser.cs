using Edge.SyntaxNodes;
using Edge.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Edge
{

    public class EdgeParser : IParser
    {

        private ILexer lexer;

        private List<IToken> tokens;
        private int position;

        // todo: load from config
        private HashSet<string> defaultAssemblies;

        public EdgeParser()
            : this(new EdgeLexer())
        {

        }

        public EdgeParser(ILexer lexer)
        {
            this.lexer = lexer;

            defaultAssemblies = new HashSet<string>()
            {
                "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                "System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                "WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                "PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                "PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                "System.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
            };
        }

        private IToken GetToken()
        {
            var token = tokens[position];
            position++;

            return token;
        }

        private IToken PeekToken()
        {
            return tokens[position];
        }

        private Type SearchType(string type)
        {
            var types = from lib in defaultAssemblies
                        let assembly = Assembly.Load(lib)
                        from t in assembly.GetTypes()
                        where t.Name == type
                        select t;

            // todo: error message
            if (types.Count() == 0)
                throw new EdgeParserException();
            if (types.Count() > 1)
                throw new EdgeParserException();

            return types.First();
        }

        private RootNode Root()
        {
            var namespaces = Namespaces();
            var obj = Object();

            return new RootNode(obj, namespaces);
        }

        private IEnumerable<NamespaceNode> Namespaces()
        {
            if (PeekToken() is UsingToken)
            {
                var namespaces = new List<NamespaceNode>();

                while (PeekToken() is UsingToken)
                {
                    namespaces.Add(GetNamespace());
                }

                return namespaces;
            }

            return null;
        }

        private NamespaceNode GetNamespace()
        {
            var token = GetToken();
            if (!(token is UsingToken))
                // todo: error message
                throw new EdgeParserException();

            StringBuilder sb = new StringBuilder();

            while (true)
            {
                token = GetToken();
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
            var properties = Properties(type);

            return new ObjectNode(type, properties);
        }

        private IEnumerable<PropertyNode> Properties(Type type)
        {
            var token = PeekToken();
            if (token is SymbolToken && ((SymbolToken)token).Symbol == '{')
            {
                position++;
                var properties = new List<PropertyNode>();

                while (true)
                {
                    token = PeekToken();
                    if (token is SymbolToken && ((SymbolToken)token).Symbol == '}')
                    {
                        position++;
                        break;
                    }

                    if (token is PropertyToken)
                        properties.Add(GetProperty(type));
                        // todo: comma
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

        private PropertyNode GetProperty(Type type)
        {
            var token = GetToken();
            if (!(token is PropertyToken))
                // todo: error message
                throw new EdgeParserException();

            var propertyInfo = type.GetProperty(((PropertyToken)token).Property);
            if (propertyInfo == null)
                // todo: error message
                throw new EdgeParserException();
            var propertyValue = PropertyValue(propertyInfo);

            return new PropertyNode(propertyInfo, propertyValue);
        }

        private object PropertyValue(PropertyInfo propertyInfo)
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
