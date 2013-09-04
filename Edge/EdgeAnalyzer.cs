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
            ChechNamespaces(tree.Namespaces.Concat(namespaces).Distinct());
            ChechObjects(tree.Objects);
        }

        private void ChechNamespaces(IEnumerable<string> namespaces)
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

        private void ChechObjects(IEnumerable<ObjectNode> objects)
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
