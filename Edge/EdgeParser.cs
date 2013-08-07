// Copyright 2013 Dmitry Kischenko
//
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either 
// express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
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
        private IEnumerable<string> assemblies;

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

        private IToken PeekToken()
        {
            return tokens[position];
        }

        private Type SearchType(string type)
        {
            var types = from lib in assemblies
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

        private bool TrySearchType(string type, out Type outType)
        {
            var types = from lib in assemblies
                        let assembly = Assembly.Load(lib)
                        from t in assembly.GetTypes()
                        where t.Name == type
                        select t;

            // todo: error message
            if (types.Count() != 1)
            {
                outType = null;
                return false;
            }

            outType = types.First();
            return true;
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
            var id = ObjectId();
            //var ctor = Constructor();
            var properties = Properties(type);

            return new ObjectNode(type, id, properties);
        }

        private string ObjectId()
        {
            var token = PeekToken();
            if (token is SymbolToken && ((SymbolToken)token).Symbol == '#')
            {
                position++;

                token = GetToken();
                if (!(token is IdToken))
                    // todo: error message
                    throw new EdgeParserException();

                return ((IdToken)token).Id;
            }

            // todo: create default id
            return null;
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
                    {
                        properties.Add(GetProperty(type));

                        token = PeekToken();
                        if (token is SymbolToken && ((SymbolToken)token).Symbol == ',')
                        {
                            position++;
                            token = PeekToken();
                            if (!(token is PropertyToken))
                                // todo: error message
                                throw new EdgeParserException();
                        }
                    }
                    else
                    {
                        // todo: error message
                        throw new EdgeParserException();
                    }
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
            // todo: test
            var token = GetToken();
            if (!(token is SymbolToken) || ((SymbolToken)token).Symbol != ':')
                // todo: error message
                throw new EdgeParserException();

            object value = null;
            try
            {
                token = GetToken();
                if (token is NumberToken)
                {
                    var numberToken = token as NumberToken;

                    CastHelper.CheckCast(numberToken.Number, propertyInfo.PropertyType);
                    value = numberToken.Number;
                }
                else if (token is StringToken)
                {
                    var stringToken = token as StringToken;

                    CastHelper.CheckCast(stringToken.Str, propertyInfo.PropertyType);
                    value = stringToken.Str;
                }
                else if (token is TypeToken)
                {
                    position--;
                    var obj = Object();

                    if (!propertyInfo.PropertyType.IsAssignableFrom(obj.Info))
                        // todo: error message
                        throw new InvalidCastException();

                    value = obj;
                }
                else if (token is WordToken)
                {
                    // todo: ...
                    var word = token as WordToken;
                    var propertyType = propertyInfo.PropertyType;

                    token = PeekToken();
                    if (token is SymbolToken && ((SymbolToken)token).Symbol == '.')
                    {
                        position++;
                        token = GetToken();

                        if (!(token is WordToken))
                            // todo: error message
                            throw new EdgeParserException();

                        word = token as WordToken;
                        value = Enum.Parse(propertyType, word.Word);
                    }
                    else
                    {
                        Type type;
                        if (TrySearchType(word.Word, out type))
                        {
                            // todo: default id
                            value = new ObjectNode(type, null);
                        }
                        else
                        {
                            value = Enum.Parse(propertyType, word.Word);
                        }
                    }
                }
                else if (token is IdToken)
                {
                    // todo: reference to object
                }
            }
            catch (ArgumentException ae)
            {
                // todo: error message
                throw new EdgeParserException("", ae);
            }
            catch (InvalidCastException ice)
            {
                // todo: error message
                throw new EdgeParserException("", ice);
            }

            return value;
        }

        public RootNode Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException("text");

            tokens = lexer.Tokenize(text).ToList();

            return Root();
        }

        public IEnumerable<string> Assemblies
        {
            get
            {
                return assemblies;
            }
            set
            {
                assemblies = value;
            }
        }

    }
}
