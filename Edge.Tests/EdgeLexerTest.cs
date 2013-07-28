using Edge.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edge.Tests
{

    [TestClass]
    public class EdgeLexerTest
    {

        private ILexer lexer = new EdgeLexer();

        private void TestTokens(string text, List<IToken> expected)
        {
            var tokens = lexer.Tokenize(text).ToList();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void IdTest()
        {
            TestTokens("Window#window1", new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('#'),
                new IdToken("window1")
            });
        }

        [TestMethod]
        public void IntNumberTest()
        {
            TestTokens("1024", new List<IToken>()
            {
                new NumberToken(1024)
            });
        }

        [TestMethod]
        public void DoubleNumberTest()
        {
            TestTokens("1024.2", new List<IToken>()
            {
                new NumberToken(1024.2)
            });
        }

        [TestMethod]
        public void PropertyTest()
        {
            TestTokens("Width: 1024", new List<IToken>()
            {
                new PropertyToken("Width"),
                new SymbolToken(':'),
                new NumberToken(1024)
            });
        }

        [TestMethod]
        public void WordTest()
        {
            TestTokens("WindowState: Maximized", new List<IToken>()
            {
                new PropertyToken("Width"),
                new SymbolToken(':'),
                new WordToken("Maximized")
            });
        }

        [TestMethod]
        public void StringTest()
        {
            TestTokens("Title: \"Hello\"", new List<IToken>()
            {
                new PropertyToken("Title"),
                new SymbolToken(':'),
                new StringToken("Hello")
            });
        }

        [TestMethod]
        public void UsingAndNamespaceTest()
        {
            TestTokens("using System.Windows;", new List<IToken>()
            {
                new UsingToken(),
                new NamespaceToken("System.Windows"),
                new SymbolToken(';')
            });
        }

    }

}
