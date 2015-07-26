// Copyright 2013 - 2015 Dmitry Kischenko
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Edge
{

    public class EdgeAnalyzer : IAnalyzer
    {

        private HashSet<string> assemblies;
        private HashSet<string> namespaces;

        public EdgeAnalyzer()
        {

        }

        private IEnumerable<Type> GetAppropriateTypes(string type)
        {
            var appDomain = AppDomain.CreateDomain("For Libs");
            try
            {
                var allTypes = new List<Type>();

                foreach (var lib in assemblies)
                {
                    var types = appDomain.Load(lib).GetTypes();

                    foreach (var ns in namespaces)
                        allTypes.AddRange(types.Where(t => t.Namespace == ns && t.Name == type));
                }

                return allTypes;
            }
            finally
            {
                AppDomain.Unload(appDomain);
            }
        }

        private Type CheckType(string type)
        {
            var types = GetAppropriateTypes(type).ToList();

            // todo: error message
            if (types.Count == 0)
                throw new EdgeAnalyzerException();
            if (types.Count > 1)
                throw new EdgeAnalyzerException();

            return types[0];
        }

        public void Analyze(SyntaxTree tree)
        {
            if (tree.Namespaces != null)
            {
                CheckNamespaces(tree.Namespaces.Concat(namespaces).Distinct());
            }
            else
            {
                tree.Namespaces = namespaces;
                CheckNamespaces(tree.Namespaces);
            }
            CheckObjects(tree.Objects);
        }

        private void CheckNamespaces(IEnumerable<string> namespaces)
        {
            if (namespaces != null)
            {
                var appDomain = AppDomain.CreateDomain("For Libs");

                try
                {
                    // todo: inline?
                    foreach (var ns in namespaces)
                        CheckNamespace(appDomain, ns);
                }
                finally
                {
                    AppDomain.Unload(appDomain);
                }
            }
        }

        private void CheckNamespace(AppDomain appDomain, string ns)
        {
            foreach (var assembly in assemblies)
                foreach (var type in appDomain.Load(assembly).GetTypes())
                    if (type.Namespace == ns)
                        return;

            // todo: error message
            throw new EdgeAnalyzerException();
        }

        private void CheckObjects(IEnumerable<ObjectNode> objects)
        {
            var objs = objects.ToList();

            if (objs.Count == 0)
                // todo: error message
                throw new ArgumentException();

            if (objs.Count(obj => obj is RootObjectNode) != 1)
                // todo: error message
                throw new EdgeAnalyzerException();

            ChechAllIDs(objs);

            foreach (var obj in objs)
                CheckObject(obj);
        }

        private void ChechAllIDs(IEnumerable<ObjectNode> objects)
        {
            var list = objects.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i].Id == list[j].Id)
                        // todo: error message
                        throw new EdgeAnalyzerException();
                }
            }
        }

        private void CheckObject(ObjectNode obj)
        {
            var type = CheckType(obj.Type);
            CheckCtor(type, obj.ConstructorArguments);
            CheckProperties(type, obj.Properties);
        }

        private void CheckCtor(Type objType, IEnumerable<IValueNode> ctorArgs)
        {
            ConstructorInfo ctor;
            List<IValueNode> args = null;
            if (ctorArgs != null)
                args = ctorArgs.ToList();

            if (ctorArgs == null || args.Count == 0)
            {
                ctor = objType.GetConstructor(Type.EmptyTypes);
            }
            else
            {
                Type[] types = new Type[args.Count];
                var strType = typeof(string);
                var doubleType = typeof(double);

                for (int i = 0; i < types.Length; i++)
                {
                    var t = args[0];
                    if (t is ReferenceNode)
                        types[i] = CheckType(((ReferenceNode)t).Type);
                    else if (t is StringNode)
                        types[i] = strType;
                    else if (t is NumberNode)
                        types[i] = doubleType;
                    else if (t is EnumNode)
                        types[i] = CheckType(((EnumNode)t).Type);
                    else
                        // todo: error message
                        throw new EdgeParserException();
                }

                ctor = objType.GetConstructor(types);
            }

            if (ctor == null)
                // todo: error message
                throw new EdgeAnalyzerException();
        }

        private void CheckProperties(Type objType, IEnumerable<PropertyNode> properties)
        {
            if (properties != null)
            {
                var list = properties.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = i + 1; j < list.Count; j++)
                    {
                        if (list[i].Property == list[j].Property)
                            // todo: error message
                            throw new EdgeAnalyzerException();
                    }
                }

                foreach (var property in list)
                    CheckProperty(objType, property);
            }
        }

        private void CheckProperty(Type objType, PropertyNode property)
        {
            var prop = objType.GetProperty(property.Property);

            if (prop == null)
                // todo: error message
                throw new EdgeAnalyzerException();

            CheckValue(prop.PropertyType, property);
        }

        private void CheckValue(Type expected, PropertyNode property)
        {
            if (property.Value is NumberNode)
            {
                if (!expected.IsAssignableFrom(typeof(double)))
                    // todo: error message
                    throw new EdgeAnalyzerException();
            }
            else if (property.Value is StringNode)
            {
                if (!expected.IsAssignableFrom(typeof(string)))
                    // todo: error message
                    throw new EdgeAnalyzerException();
            }
            else if (property.Value is ReferenceNode)
            {
                if (!expected.IsAssignableFrom(CheckType(((ReferenceNode)property.Value).Type)))
                    // todo: error message
                    throw new EdgeAnalyzerException();
            }
            else if (property.Value is EnumNode)
            {
                var e = (EnumNode)property.Value;
                Type eType;

                if (e.Type != null)
                {
                    eType = CheckType(e.Type);

                    if (expected != eType)
                        // todo: error message
                        throw new EdgeAnalyzerException();
                }
                else
                {
                    eType = expected;
                }

                if (!Enum.IsDefined(eType, e.Value))
                    // todo: error message
                    throw new EdgeAnalyzerException();

                e.Type = eType.Name;
            }
            else if (property.Value is ArrayNode)
            {
                var arr = (ArrayNode)property.Value;

                var interfaceType = expected.GetInterface("IDictionary`2");
                if (interfaceType != null)
                {
                    property.Value = new CollectionNode(expected.Name, interfaceType.GetGenericArguments()[1].Name, arr.Array);
                }
                else if ((interfaceType = expected.GetInterface("ICollection`1")) != null)
                {
                    property.Value = new CollectionNode(expected.Name, interfaceType.GetGenericArguments()[0].Name, arr.Array);
                }
                else if (typeof(ICollection).IsAssignableFrom(expected))
                {
                    property.Value = new CollectionNode(expected.Name, "Object", arr.Array);
                }
                else if (!expected.IsAssignableFrom(CheckType(arr.ElementType)))
                {
                    // todo: error message
                    throw new EdgeAnalyzerException();
                }
            }
            else if (property.Value is BindingNode)
            {
            }
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
