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

        private IEnumerable<Type> GetAppropriateTypes(string type)
        {
            return from lib in assemblies
                   let assembly = Assembly.Load(lib)
                   from t in assembly.GetTypes()
                   from ns in namespaces
                   where t.Namespace == ns && t.Name == type
                   select t;
        }

        private Type SearchType(string type)
        {
            var types = GetAppropriateTypes(type);

            // todo: error message
            if (types.Count() == 0)
                throw new EdgeParserException();
            if (types.Count() > 1)
                throw new EdgeParserException();

            return types.First();
        }

        private bool TrySearchType(string type, out Type outType)
        {
            var types = GetAppropriateTypes(type);

            // todo: error message
            if (types.Count() != 1)
            {
                outType = null;
                return false;
            }

            outType = types.First();
            return true;
        }

        private bool TryCheckGenerics(Type type, Type genericDefinition, out Type genericType)
        {
            genericType = (from interfaceType in type.GetInterfaces()
                           where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == genericDefinition
                           select interfaceType).FirstOrDefault();

            return genericType != null;
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

        private string GenereteId(Type type)
        {
            var typeName = type.Name;
            typeName = char.ToLowerInvariant(typeName[0]) + typeName.Substring(1);

            for (int i = 1; i < int.MaxValue; i++)
            {
                var id = typeName + i;
                if (!objects.ContainsKey(id))
                    return id;
            }

            // todo: error message
            throw new EdgeParserException();
        }

        private SyntaxTree Tree()
        {
            var namespaces = GetNamespaces();
            Root();

            return new SyntaxTree(namespaces, from obj in objects select obj.Value);
        }

        private void Root()
        {
            var token = GetToken();
            if (!(token is TypeToken))
                // todo: error message
                throw new EdgeParserException();

            var type = SearchType(((TypeToken)token).Type);

            objects["this"] = null;

            if (CheckSymbol(PeekToken(), '#'))
                // todo: error message
                throw new EdgeParserException();

            var ctor = CtorArgs(type);
            var properties = Properties(type);

            var obj = new RootObjectNode(type, ctor, properties);
            objects["this"] = obj;
        }

        private IEnumerable<NamespaceNode> GetNamespaces()
        {
            if (PeekToken() is UsingToken)
            {
                var localNamespaces = new List<NamespaceNode>();

                while (PeekToken() is UsingToken)
                {
                    var ns = GetNamespace();

                    localNamespaces.Add(ns);
                    namespaces.Add(ns.Namespace);
                }

                return localNamespaces;
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

            return new NamespaceNode(ns);
        }

        private ReferenceNode Object()
        {
            var token = GetToken();
            if (!(token is TypeToken))
                // todo: error message
                throw new EdgeParserException();

            var type = SearchType(((TypeToken)token).Type);
            var id = ObjectId(type);
            objects[id] = null;
            var ctor = CtorArgs(type);
            var properties = Properties(type);

            var obj = new ObjectNode(type, id, ctor, properties);
            objects[id] = obj;

            return new ReferenceNode(id);
        }

        private string ObjectId(Type objType)
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

            return GenereteId(objType);
        }

        private IEnumerable<IValueNode> CtorArgs(Type objType)
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
                {
                    var types = new Type[args.Count];
                    var strType = typeof(string);
                    var doubleType = typeof(double);

                    for (int i = 0; i < types.Length; i++)
                    {
                        var t = args[0];
                        if (t is ReferenceNode)
                            types[i] = objects[((ReferenceNode)t).Id].Info;
                        else if (t is StringNode)
                            types[i] = strType;
                        else if (t is NumberNode)
                            types[i] = doubleType;
                        else if (t is EnumNode)
                            types[i] = ((EnumNode)t).Info;
                        else
                            // todo: error message
                            throw new EdgeParserException();
                    }

                    var ctor = objType.GetConstructor(types);
                    if (ctor == null)
                        // todo: error message
                        throw new EdgeParserException();

                    return args;
                }

                return null;
            }

            return null;
        }

        private IEnumerable<PropertyNode> Properties(Type objType)
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
                        var prop = GetProperty(objType);
                        if ((from property in properties
                             where property.Info.Equals(prop.Info)
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

        private IValueNode PropertyValue(Type propType)
        {
            var token = GetToken();
            if (!(token is SymbolToken) || ((SymbolToken)token).Symbol != ':')
                // todo: error message
                throw new EdgeParserException();

            return GetValue(propType);
        }

        private ArrayNode Array()
        {
            return Array(null);
        }

        private ArrayNode Array(Type type)
        {
            var token = GetToken();
            Type arrayType = null;
            if (token is TypeToken)
            {
                arrayType = SearchType(((TypeToken)token).Type);

                token = GetToken();
            }

            if (CheckSymbol(token, '['))
            {
                var arr = new List<object>();

                while (true)
                {
                    token = PeekToken();
                    if (CheckSymbol(token, ']'))
                    {
                        position++;
                        break;
                    }

                    object obj = null;
                    if (arrayType != null)
                    {
                        obj = GetValue(arrayType);
                    }
                    else if (type != null)
                    {
                        if (type.IsArray)
                        {
                            arrayType = type.GetElementType();
                        }
                        else
                        {
                            Type genericType;
                            if (TryCheckGenerics(type, typeof(IDictionary<,>), out genericType))
                            {
                                arrayType = genericType.GetGenericArguments()[1];
                                obj = GetValue(arrayType);
                            }
                            else if (TryCheckGenerics(type, typeof(ICollection<>), out genericType))
                            {
                                arrayType = genericType.GetGenericArguments()[0];
                                obj = GetValue(arrayType);
                            }
                            else if (typeof(ICollection).IsAssignableFrom(type))
                            {
                                arrayType = typeof(object);
                                obj = GetValue(arrayType);
                            }
                            else
                            {
                                obj = GetValue();

                                if (obj is ObjectNode)
                                    arrayType = ((ObjectNode)obj).Info;
                                else if (obj is double || obj is string || obj.GetType().IsEnum)
                                    arrayType = obj.GetType();
                                else
                                    // todo: error message
                                    throw new EdgeParserException();
                            }
                        }
                    }
                    else
                    {
                        obj = GetValue();

                        if (obj is ObjectNode)
                            arrayType = ((ObjectNode)obj).Info;
                        else if (obj is double || obj is string || obj.GetType().IsEnum)
                            arrayType = obj.GetType();
                        else
                            // todo: error message
                            throw new EdgeParserException();
                    }

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

                if (arr.Count == 0)
                    return null;

                return new ArrayNode(arrayType, arr.ToArray());
            }

            return null;
        }

        private IValueNode GetValue()
        {
            return GetValue(null);
        }

        private IValueNode GetValue(Type type)
        {
            try
            {
                var token = GetToken();

                if (token is NumberToken)
                {
                    var numberToken = token as NumberToken;
                    var number = numberToken.Number;

                    if (type != null)
                        CastHelper.CheckCast(number, type);

                    return new NumberNode(number);
                }
                if (token is StringToken)
                {
                    var stringToken = token as StringToken;
                    var str = stringToken.Str;

                    if (type != null)
                        CastHelper.CheckCast(str, type);

                    return new StringNode(str);
                }
                if (token is TypeToken)
                {
                    token = PeekToken();
                    if (CheckSymbol(token, '['))
                    {
                        return Array();
                    }

                    position--;
                    var reference = Object();

                    if (type != null && !type.IsAssignableFrom(objects[reference.Id].Info))
                        // todo: error message
                        throw new InvalidCastException();

                    return reference;
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

                        word = token as WordToken;
                        return new EnumNode(type, Enum.Parse(type, word.Word));
                    }

                    Type outType;
                    if (TrySearchType(word.Word, out outType))
                    {
                        var id = GenereteId(outType);
                        var obj = new ObjectNode(outType, GenereteId(outType));

                        objects[id] = obj;

                        return new ReferenceNode(id);
                    }

                    return new EnumNode(type, Enum.Parse(type, word.Word));
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

                        return new ReferenceNode(id.Id);
                    }
                    if (symbol.Symbol == '@')
                    {
                        token = GetToken();

                        if (!(token is WordToken))
                            // todo: error message
                            throw new EdgeParserException();

                        var word = token as WordToken;

                        if (word.Word.Contains('.'))
                        {
                            var dot = word.Word.IndexOf('.');

                            return new BindingNode(word.Word.Substring(0, dot), word.Word.Substring(dot + 1));
                        }

                        return new BindingNode(word.Word);
                    }
                    if (symbol.Symbol == '[')
                    {
                        position--;

                        return Array(type);
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
