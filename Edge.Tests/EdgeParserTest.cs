using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Edge.Tokens;
using System.Collections.Generic;
using Edge.SyntaxNodes;

namespace Edge.Tests
{

    [TestClass]
    public class EdgeParserTest
    {

        private MockEdgeLexer lexer;
        private IParser parser;

        public EdgeParserTest()
        {
            lexer = new MockEdgeLexer();
            parser = new EdgeParser(lexer)
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
                }
            };
        }

        private void Test(IEnumerable<IToken> tokens, RootNode expected)
        {
            lexer.Tokens = tokens;

            var root = parser.Parse("-- // --");

            Assert.AreEqual(expected, root);
        }

        [TestMethod]
        public void NamespaceWithObjectTest()
        {
            Test(new List<IToken>()
            {
                new UsingToken(),
                new WordToken("System"),
                new SymbolToken(';'),
                new UsingToken(),
                new WordToken("System"),
                new SymbolToken('.'),
                new WordToken("Windows"),
                new SymbolToken(';'),
                new TypeToken("Window"),
                new SymbolToken('{'),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    typeof(System.Windows.Window)),
                    new List<NamespaceNode>()
                    {
                        new NamespaceNode("System"),
                        new NamespaceNode("System.Windows")
                    }));
        }

        [TestMethod]
        public void ObjectWithNumberPropertyTest()
        {
            var type = typeof(System.Windows.Window);
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Width"),
                new SymbolToken(':'),
                new NumberToken(1024.6),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("Width"), 1024.6)
                    })));
        }

        [TestMethod]
        public void ObjectWithStringPropertyTest()
        {
            var type = typeof(System.Windows.Window);
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Title"),
                new SymbolToken(':'),
                new StringToken("Hello"),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("Title"), "Hello")
                    })));
        }

        [TestMethod]
        public void ObjectWithObjectPropertyTest()
        {
            var type = typeof(System.Windows.Window);
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Content"),
                new SymbolToken(':'),
                new TypeToken("Grid"),
                new SymbolToken('{'),
                new SymbolToken('}'),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("Content"), new ObjectNode(typeof(System.Windows.Controls.Grid)))
                    })));
        }

        [TestMethod]
        public void ObjectWithEnumPropertyTest()
        {
            var type = typeof(System.Windows.Window);
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("WindowState"),
                new SymbolToken(':'),
                new WordToken("Maximized"),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("WindowState"), System.Windows.WindowState.Maximized)
                    })));
        }

    }

}
