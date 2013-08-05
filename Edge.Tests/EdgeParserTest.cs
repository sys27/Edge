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
            parser = new EdgeParser(lexer);
        }

        [TestMethod]
        public void NamespaceWithObjectTest()
        {
            lexer.Tokens = new List<IToken>()
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
            };
            var expected = new RootNode(
                new ObjectNode(typeof(System.Windows.Window)),
                new List<NamespaceNode>()
                {
                    new NamespaceNode("System"),
                    new NamespaceNode("System.Windows")
                });

            var root = parser.Parse("using System;using System.Windows;Window { }");

            Assert.AreEqual(expected, root);
        }

        [TestMethod]
        public void ObjectWithNumberPropertyTest()
        {
            lexer.Tokens = new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Width"),
                new SymbolToken(':'),
                new NumberToken(1024.6),
                new SymbolToken('}')
            };
            var type = typeof(System.Windows.Window);
            var expected = new RootNode(
                new ObjectNode(
                    type,
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("Width"), 1024.6)
                    }));

            var root = parser.Parse("Window { Width: 1024.6 }");

            Assert.AreEqual(expected, root);
        }

        [TestMethod]
        public void ObjectWithStringPropertyTest()
        {
            lexer.Tokens = new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Title"),
                new SymbolToken(':'),
                new StringToken("Hello"),
                new SymbolToken('}')
            };
            var type = typeof(System.Windows.Window);
            var expected = new RootNode(
                new ObjectNode(
                    type,
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("Title"), "Hello")
                    }));

            var root = parser.Parse("Window { Title: \"Hello\" }");

            Assert.AreEqual(expected, root);
        }

        [TestMethod]
        public void ObjectWithObjectPropertyTest()
        {
            lexer.Tokens = new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Content"),
                new SymbolToken(':'),
                new TypeToken("Grid"),
                new SymbolToken('{'),
                new SymbolToken('}'),
                new SymbolToken('}')
            };
            var type = typeof(System.Windows.Window);
            var expected = new RootNode(
                new ObjectNode(
                    type,
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("Content"), new ObjectNode(typeof(System.Windows.Controls.Grid)))
                    }));

            var root = parser.Parse("Window { Content: Grid { } }");

            Assert.AreEqual(expected, root);
        }

        [TestMethod]
        public void ObjectWithEnumPropertyTest()
        {
            lexer.Tokens = new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("WindowState"),
                new SymbolToken(':'),
                new WordToken("Maximized"),
                new SymbolToken('}')
            };
            var type = typeof(System.Windows.Window);
            var expected = new RootNode(
                new ObjectNode(
                    type,
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("WindowState"), System.Windows.WindowState.Maximized)
                    }));

            var root = parser.Parse("Window { WindowState: Maximized }");

            Assert.AreEqual(expected, root);
        }

    }

}
