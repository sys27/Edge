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

    }

}
