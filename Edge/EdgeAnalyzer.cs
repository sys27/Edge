using Edge.SyntaxNodes;
using System;
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
            return from lib in assemblies
                   let assembly = Assembly.Load(lib)
                   from t in assembly.GetTypes()
                   from ns in namespaces
                   where t.Namespace == ns && t.Name == type
                   select t;
        }

        private Type CheckType(string type)
        {
            var types = GetAppropriateTypes(type);

            // todo: error message
            if (types.Count() == 0)
                throw new EdgeAnalyzerException();
            if (types.Count() > 1)
                throw new EdgeAnalyzerException();

            return types.First();
        }

        public void Analyze(SyntaxTree tree)
        {
            CheckNamespaces(tree.Namespaces.Concat(namespaces).Distinct());
            CheckObjects(tree.Objects);
        }

        private void CheckNamespaces(IEnumerable<string> namespaces)
        {
            foreach (var ns in namespaces)
                CheckNamespace(ns);
        }

        private void CheckNamespace(string ns)
        {
            foreach (var lib in assemblies)
            {
                var assembly = Assembly.Load(lib);

                foreach (var type in assembly.GetTypes())
                {
                    if (type.Namespace == ns)
                        return;
                }
            }

            // todo: error message
            throw new EdgeAnalyzerException();
        }

        private void CheckObjects(IEnumerable<ObjectNode> objects)
        {
            ChechAllIDs(objects);

            foreach (var obj in objects)
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
            ConstructorInfo ctor = null;
            var args = ctorArgs.ToList();

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

            foreach (var property in properties)
                CheckProperty(objType, property);
        }

        private void CheckProperty(Type objType, PropertyNode property)
        {
            var prop = objType.GetProperty(property.Property);

            if (prop == null)
                // todo: error message
                throw new EdgeAnalyzerException();

            CheckValue(prop.PropertyType, property.Value);
        }

        private void CheckValue(Type expected, IValueNode value)
        {
            if (value is NumberNode)
            {
                if (!expected.IsAssignableFrom(typeof(double)))
                    // todo: error message
                    throw new EdgeAnalyzerException();
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
