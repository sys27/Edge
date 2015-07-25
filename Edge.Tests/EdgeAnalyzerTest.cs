using Edge.SyntaxNodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Edge.Tests
{

    [TestClass]
    public class EdgeAnalyzerTest
    {

        private readonly EdgeAnalyzer analyzer;

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
            var root = new RootObjectNode("Window");
            var st = new SyntaxTree(new List<string> { "System" }, root, new List<ObjectNode> { root });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void CheckWrongNamespaceTest()
        {
            var root = new RootObjectNode("Window");
            var st = new SyntaxTree(new List<string> { "System.Windows.Forms" }, root, new List<ObjectNode> { root });

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
            var st = new SyntaxTree(null, new List<ObjectNode>());

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void ObjectsWithSameId()
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
        public void ObjectWithoutRoot()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>
                {
                    new ObjectNode("Window", "hello")
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void ObjectWithTwoRoot()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window"),
                    new RootObjectNode("Window")
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void CheckTypeTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>
                {
                    new RootObjectNode("ArrayList")
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        public void CtorTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>
                {
                    new RootObjectNode("Uri", new List<IValueNode>() { new StringNode("icon.ico") })
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void CtorWrongCountOfArgsTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Uri", new List<IValueNode>() { new StringNode("icon.ico"), new StringNode("hello") })
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void CtorWrongCountOfArgs2Test()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Uri")
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void CtorWrongTypesTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Uri", new List<IValueNode>() { new NumberNode(1) })
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void SamePropsTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window", null, new List<PropertyNode>() { new PropertyNode("Width", new NumberNode(1)), new PropertyNode("Width", new NumberNode(1)) })
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void PropNotFoundTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window", null, new List<PropertyNode>() { new PropertyNode("hello", new StringNode("hello")) })
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        public void PropNumberTypeTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window", null, new List<PropertyNode>() { new PropertyNode("Width", new NumberNode(1024)) })
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void PropIsNotNumberTypeTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window", null, new List<PropertyNode>() { new PropertyNode("Title", new NumberNode(1024)) })
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        public void PropStrTypeTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window", null, new List<PropertyNode>() { new PropertyNode("Title", new StringNode("Hello")) })
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void PropIsNotStrTypeTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window", null, new List<PropertyNode>() { new PropertyNode("Width", new StringNode("Hello")) })
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        public void PropRefTypeTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new ObjectNode("Uri", "uri", new List<IValueNode>() { new StringNode("ico.ico") }),
                    new RootObjectNode("Window", null, new List<PropertyNode>() { new PropertyNode("Content", new ReferenceNode("uri", "Uri")) })
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void PropIsNotRefTypeTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new ObjectNode("Uri", "uri", new List<IValueNode>() { new StringNode("ico.ico") }),
                    new RootObjectNode("Window", null, new List<PropertyNode>() { new PropertyNode("Title", new ReferenceNode("uri", "Uri")) })
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        public void PropEnumTypeTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window", null, new List<PropertyNode>() { new PropertyNode("WindowState", new EnumNode("WindowState", "Normal")) })
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void PropIsNotEnumTypeTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window", null, new List<PropertyNode>() { new PropertyNode("Title", new EnumNode("WindowState", "Normal")) })
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void PropIsNotEnumValueTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window", null, new List<PropertyNode>() { new PropertyNode("WindowState", new EnumNode("WindowState", "Hello")) })
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        public void PropArrayTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode("Content",
                                new ArrayNode("TextBox",
                                    new IValueNode[]
                                    {
                                        new ReferenceNode("tb1", "TextBox"),
                                        new ReferenceNode("tb2", "TextBox")
                                    }))
                        }),
                    new ObjectNode("TextBox", "tb1"),
                    new ObjectNode("TextBox", "tb2")
                });

            analyzer.Analyze(st);
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeAnalyzerException))]
        public void PropIsNotArrayTest()
        {
            var st = new SyntaxTree(null,
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode("Title",
                                new ArrayNode("TextBox",
                                    new IValueNode[]
                                    {
                                        new ReferenceNode("tb1", "TextBox"),
                                        new ReferenceNode("tb2", "TextBox")
                                    }))
                        }),
                    new ObjectNode("TextBox", "tb1"),
                    new ObjectNode("TextBox", "tb2")
                });

            analyzer.Analyze(st);
        }

    }

}
