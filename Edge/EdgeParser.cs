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

        private Dictionary<string, ObjectNode> ids;

        public EdgeParser()
            : this(new EdgeLexer())
        {

        }

        public EdgeParser(ILexer lexer)
        {
            this.lexer = lexer;
            ids = new Dictionary<string, ObjectNode>();
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

        private void ReadIds()
        {
            foreach (var token in tokens)
            {
                if (token is IdToken)
                {
                    var id = token as IdToken;
                    ids[id.Id] = null;
                }
            }
        }

        private void CheckIds()
        {
            foreach (var id in ids)
            {
                if (id.Value == null)
                    // todo: error message
                    throw new EdgeParserException();
            }
        }

        private string GenereteId(Type type)
        {
            var typeName = type.Name;
            typeName = char.ToLowerInvariant(typeName[0]) + typeName.Substring(1);

            for (int i = 1; i < int.MaxValue; i++)
            {
                var id = typeName + i;
                if (!ids.ContainsKey(id))
                    return id;
            }

            // todo: error message
            throw new EdgeParserException();
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
            var id = ObjectId(type);
            var ctor = CtorArgs(type);
            var properties = Properties(type);

            var obj = new ObjectNode(type, id, ctor, properties);
            ids[id] = obj;

            return obj;
        }

        private string ObjectId(Type objType)
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

            return GenereteId(objType);
        }

        private IEnumerable<object> CtorArgs(Type objType)
        {
            var token = PeekToken();
            if (token is SymbolToken && ((SymbolToken)token).Symbol == '(')
            {
                position++;
                var args = new List<object>();

                while (true)
                {
                    token = PeekToken();
                    if (token is SymbolToken && ((SymbolToken)token).Symbol == ')')
                    {
                        position++;
                        break;
                    }

                    //var obj = GetValue();
                }

                if (args.Count == 0)
                    return null;

                return args;
            }

            return null;
        }

        private IEnumerable<PropertyNode> Properties(Type objType)
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
                        properties.Add(GetProperty(objType));

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

        private PropertyNode GetProperty(Type objType)
        {
            var token = GetToken();
            if (!(token is PropertyToken))
                // todo: error message
                throw new EdgeParserException();

            var propertyInfo = objType.GetProperty(((PropertyToken)token).Property);
            if (propertyInfo == null)
                // todo: error message
                throw new EdgeParserException();
            var propertyValue = PropertyValue(propertyInfo.PropertyType);

            return new PropertyNode(propertyInfo, propertyValue);
        }

        private object PropertyValue(Type propType)
        {
            var token = GetToken();
            if (!(token is SymbolToken) || ((SymbolToken)token).Symbol != ':')
                // todo: error message
                throw new EdgeParserException();

            return GetValue(propType);
        }

        private object GetValue(Type type)
        {
            object value = null;
            try
            {
                var token = GetToken();
                if (token is NumberToken)
                {
                    var numberToken = token as NumberToken;

                    CastHelper.CheckCast(numberToken.Number, type);
                    value = numberToken.Number;
                }
                else if (token is StringToken)
                {
                    var stringToken = token as StringToken;

                    CastHelper.CheckCast(stringToken.Str, type);
                    value = stringToken.Str;
                }
                else if (token is TypeToken)
                {
                    position--;
                    var obj = Object();

                    if (!type.IsAssignableFrom(obj.Info))
                        // todo: error message
                        throw new InvalidCastException();

                    value = obj;
                }
                else if (token is WordToken)
                {
                    // todo: ...
                    var word = token as WordToken;

                    token = PeekToken();
                    if (token is SymbolToken && ((SymbolToken)token).Symbol == '.')
                    {
                        position++;
                        token = GetToken();

                        if (!(token is WordToken))
                            // todo: error message
                            throw new EdgeParserException();

                        word = token as WordToken;
                        value = Enum.Parse(type, word.Word);
                    }
                    else
                    {
                        Type outType;
                        if (TrySearchType(word.Word, out outType))
                        {
                            value = new ObjectNode(outType, GenereteId(outType));
                        }
                        else
                        {
                            value = Enum.Parse(type, word.Word);
                        }
                    }
                }
                else if (token is SymbolToken && ((SymbolToken)token).Symbol == '#')
                {
                    token = GetToken();

                    if (!(token is IdToken))
                        // todo: error message
                        throw new EdgeParserException();

                    var id = token as IdToken;

                    value = new ReferenceNode(id.Id);
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
            ReadIds();

            var root = Root();

            CheckIds();

            return root;
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
