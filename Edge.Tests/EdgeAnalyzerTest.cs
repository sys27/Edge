using Edge;
using Edge.SyntaxNodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Edge.Tests
{

    [TestClass]
    public class EdgeAnalyzerTest
    {

        private EdgeAnalyzer analyzer;

        public EdgeAnalyzerTest()
        {
            analyzer = new EdgeAnalyzer()
            {
                Assemblies = new HashSet<string>()
                {
                    "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                    "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                    "System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                    "WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                    "PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                    "PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                    "System.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
                },
                Namespaces = new HashSet<string>()
                {
                    "System",
                    "System.Windows",
                    "System.Windows.Controls",
                    "System.Windows.Data",
                    "System.Windows.Documents",
                    "System.Windows.Input",
                    "System.Windows.Media",
                    "System.Windows.Media.Imaging",
                    "System.Windows.Navigation",
                    "System.Windows.Shapes"
                }
            };
        }

        [TestMethod]
        public void CheckNamespaceTest()
        {
            var st = new SyntaxTree(
                new List<string>()
                {
                    "System"
                },
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window")
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void CheckWrongNamespaceTest()
        {
            var st = new SyntaxTree(
                new List<string>()
                {
                    "System.Windows.Forms"
                },
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window")
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        public void NullNamespacesTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window")
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ZeroObjectsTest()
        {
            var st = new SyntaxTree(new List<ObjectNode>());

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void ObjectsWithSameID()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new ObjectNode("Window", "w1"),
                    new ObjectNode("Window", "w1")
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void CheckTypeTest()
        {

        }

    }

}
