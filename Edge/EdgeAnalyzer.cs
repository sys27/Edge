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
            foreach (var obj in objects)
                CheckObject(obj);
        }

        private void CheckObject(ObjectNode obj)
        {

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
