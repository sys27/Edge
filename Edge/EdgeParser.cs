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
using System.Collections;
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

        // todo: load from config
        private HashSet<string> assemblies;
        private HashSet<string> namespaces;

        private Dictionary<string, ObjectNode> objects;

        public EdgeParser()
            : this(new EdgeLexer())
        {

        }

        public EdgeParser(ILexer lexer)
        {
            this.lexer = lexer;

            assemblies = new HashSet<string>();
            namespaces = new HashSet<string>();
            objects = new Dictionary<string, ObjectNode>();
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

        private bool CheckSymbol(IToken token, char symbol)
        {
            return token is SymbolToken && ((SymbolToken)token).Symbol == symbol;
        }

        private void ReadIds()
        {
            foreach (var token in tokens)
            {
                if (token is IdToken)
                {
                    var id = token as IdToken;
                    objects[id.Id] = null;
                }
            }
        }

        private void CheckIds()
        {
            foreach (var id in objects)
            {
                if (id.Value == null)
                    // todo: error message
                    throw new EdgeParserException();
            }
        }

        private string GenereteId(string type)
        {
            type = char.ToLowerInvariant(type[0]) + type.Substring(1);

            for (int i = 1; i < int.MaxValue; i++)
            {
                var id = type + i;
                if (!objects.ContainsKey(id))
                    return id;
            }

            // todo: error message
            throw new EdgeParserException();
        }

        private SyntaxTree Tree()
        {
            GetNamespaces();
            Root();

            return new SyntaxTree(namespaces, objects["this"], objects.Values);
        }

        private void Root()
        {
            var token = GetToken();
            if (!(token is TypeToken))
                // todo: error message
                throw new EdgeParserException();

            var type = ((TypeToken)token).Type;

            objects["this"] = null;

            if (CheckSymbol(PeekToken(), '#'))
                // todo: error message
                throw new EdgeParserException();

            var ctor = CtorArgs();
            var properties = Properties();

            var obj = new RootObjectNode(type, ctor, properties);
            objects["this"] = obj;
        }

        private void GetNamespaces()
        {
            if (PeekToken() is UsingToken)
                while (PeekToken() is UsingToken)
                    namespaces.Add(GetNamespace());
        }

        private string GetNamespace()
        {
            var token = GetToken();
            if (!(token is UsingToken))
                // todo: error message
                throw new EdgeParserException();

            StringBuilder sb = new StringBuilder();

            while (true)
            {
                token = GetToken();
                if (CheckSymbol(token, ';'))
                    break;

                if (token is WordToken)
                    sb.Append(((WordToken)token).Word);
                else if (CheckSymbol(token, '.'))
                    sb.Append('.');
                else
                    // todo: error message 
                    throw new EdgeParserException();
            }

            var ns = sb.ToString();
            if (string.IsNullOrWhiteSpace(ns))
                // todo: error message
                throw new EdgeParserException();

            return ns;
        }

        private ReferenceNode Object()
        {
            var token = GetToken();
            if (!(token is TypeToken))
                // todo: error message
                throw new EdgeParserException();

            var type = ((TypeToken)token).Type;
            var id = ObjectId(type);
            objects[id] = null;
            var ctor = CtorArgs();
            var properties = Properties();

            var obj = new ObjectNode(type, id, ctor, properties);
            objects[id] = obj;

            return new ReferenceNode(id, type);
        }

        private string ObjectId(string type)
        {
            var token = PeekToken();
            if (CheckSymbol(token, '#'))
            {
                position++;

                token = GetToken();
                if (!(token is IdToken))
                    // todo: error message
                    throw new EdgeParserException();

                return ((IdToken)token).Id;
            }

            return GenereteId(type);
        }

        private IEnumerable<IValueNode> CtorArgs()
        {
            var token = PeekToken();
            if (CheckSymbol(token, '('))
            {
                position++;
                var args = new List<IValueNode>();

                while (true)
                {
                    token = PeekToken();
                    if (CheckSymbol(token, ')'))
                    {
                        position++;
                        break;
                    }

                    args.Add(GetValue());

                    token = PeekToken();
                    if (CheckSymbol(token, ','))
                    {
                        position++;
                        token = GetToken();

                        if (!(token is NumberToken || token is StringToken || token is TypeToken || token is WordToken))
                            // todo: error message
                            throw new EdgeParserException();
                    }
                }

                if (args.Count > 0)
                    return args;
            }

            return null;
        }

        private IEnumerable<PropertyNode> Properties()
        {
            var token = PeekToken();
            if (CheckSymbol(token, '{'))
            {
                position++;
                var properties = new List<PropertyNode>();

                while (true)
                {
                    token = PeekToken();
                    if (CheckSymbol(token, '}'))
                    {
                        position++;
                        break;
                    }

                    if (token is PropertyToken)
                    {
                        var prop = GetProperty();
                        if ((from property in properties
                             where property.Property.Equals(prop.Property)
                             select property).Count() > 0)
                            // todo: error message ...
                            throw new EdgeParserException();

                        properties.Add(prop);

                        token = PeekToken();
                        if (CheckSymbol(token, ','))
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

                if (properties.Count > 0)
                    return properties;
            }

            return null;
        }

        private PropertyNode GetProperty()
        {
            var token = GetToken();
            if (!(token is PropertyToken))
                // todo: error message
                throw new EdgeParserException();

            var propertyInfo = ((PropertyToken)token).Property;
            if (propertyInfo == null)
                // todo: error message
                throw new EdgeParserException();
            var propertyValue = PropertyValue();

            return new PropertyNode(propertyInfo, propertyValue);
        }

        private IValueNode PropertyValue()
        {
            var token = GetToken();
            if (!(token is SymbolToken) || ((SymbolToken)token).Symbol != ':')
                // todo: error message
                throw new EdgeParserException();

            return GetValue();
        }

        private ArrayNode Array()
        {
            var token = GetToken();
            string elementType = null;

            if (token is TypeToken)
            {
                elementType = ((TypeToken)token).Type;

                token = GetToken();
            }

            if (CheckSymbol(token, '['))
            {
                var arr = new List<IValueNode>();

                while (true)
                {
                    token = PeekToken();
                    if (CheckSymbol(token, ']'))
                    {
                        position++;
                        break;
                    }

                    IValueNode obj = GetValue();
                    arr.Add(obj);

                    token = PeekToken();
                    if (CheckSymbol(token, ','))
                    {
                        position++;
                        token = PeekToken();
                        if (!(token is NumberToken || token is StringToken || token is TypeToken || token is WordToken))
                            // todo: error message
                            throw new EdgeParserException();
                    }
                }

                if (arr.Count > 0)
                    return new ArrayNode(elementType, arr.ToArray());
            }

            return null;
        }

        private IValueNode GetValue()
        {
            try
            {
                var token = GetToken();

                if (token is NumberToken)
                {
                    var numberToken = token as NumberToken;
                    var number = numberToken.Number;

                    return new NumberNode(number);
                }
                if (token is StringToken)
                {
                    var stringToken = token as StringToken;
                    var str = stringToken.Str;

                    return new StringNode(str);
                }
                if (token is TypeToken)
                {
                    token = PeekToken();
                    if (CheckSymbol(token, '['))
                    {
                        position--;
                        return Array();
                    }

                    position--;
                    return Object();
                }
                if (token is WordToken)
                {
                    var word = token as WordToken;

                    token = PeekToken();
                    if (CheckSymbol(token, '.'))
                    {
                        position++;
                        token = GetToken();

                        if (!(token is WordToken))
                            // todo: error message
                            throw new EdgeParserException();

                        var word2 = token as WordToken;
                        return new EnumNode(word.Word, word2.Word);
                    }

                    return new EnumNode(word.Word);
                }
                if (token is SymbolToken)
                {
                    var symbol = token as SymbolToken;
                    if (symbol.Symbol == '#')
                    {
                        token = GetToken();

                        if (!(token is IdToken))
                            // todo: error message
                            throw new EdgeParserException();

                        var id = token as IdToken;

                        return new ReferenceNode(id.Id, objects[id.Id].Type);
                    }
                    if (symbol.Symbol == '@')
                    {
                        return GetBinding();
                    }
                    if (symbol.Symbol == '[')
                    {
                        position--;

                        return Array();
                    }
                }

                // todo: error message
                throw new EdgeParserException();
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
        }

        private BindingNode GetBinding()
        {
            var token = GetToken();

            if (CheckSymbol(token, '('))
            {
                var values = GetBindingValues();

                string elementName;
                string path;
                string strMode;
                BindingMode mode = BindingMode.Default;

                values.TryGetValue("ElementName", out elementName);
                values.TryGetValue("Path", out path);
                if (values.TryGetValue("Mode", out strMode))
                    mode = (BindingMode)Enum.Parse(typeof(BindingMode), strMode);

                return new BindingNode(elementName, path, mode);
            }
            else if (token is WordToken)
            {
                var word = token as WordToken;

                if (word.Word.Contains('.'))
                {
                    var dot = word.Word.IndexOf('.');

                    return new BindingNode(word.Word.Substring(0, dot), word.Word.Substring(dot + 1));
                }

                return new BindingNode(word.Word);
            }
            else
            {
                // todo: error message
                throw new EdgeParserException();
            }
        }

        private IDictionary<string, string> GetBindingValues()
        {
            IToken token;
            var result = new Dictionary<string, string>();

            while (true)
            {
                token = GetToken();

                if (CheckSymbol(token, ')'))
                    break;

                if (token is WordToken)
                {
                    var word = token as WordToken;

                    var key = word.Word;

                    token = GetToken();
                    if (!CheckSymbol(token, '='))
                        // todo: error message
                        throw new EdgeParserException();

                    token = GetToken();
                    if (token is WordToken)
                    {
                        word = token as WordToken;

                        result[key] = word.Word;
                    }
                    else
                    {
                        // todo: error message
                        throw new EdgeParserException();
                    }
                }
                else
                {
                    // todo: error message
                    throw new EdgeParserException();
                }

                token = PeekToken();
                if (CheckSymbol(token, ','))
                {
                    position++;
                    token = PeekToken();

                    if (!(token is WordToken))
                        // todo: error message
                        throw new EdgeParserException();
                }
            }

            if (result.Count == 0)
                // todo: error message
                throw new EdgeParserException();
            else
                return result;
        }

        public SyntaxTree Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException("text");

            tokens = lexer.Tokenize(text).ToList();
            ReadIds();

            var root = Tree();

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
                assemblies = new HashSet<string>(value);
            }
        }

        public IEnumerable<string> Namespaces
        {
            get
            {
                return namespaces;
            }
            set
            {
                namespaces = new HashSet<string>(value);
            }
        }

    }

}
