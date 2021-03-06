﻿using Edge.Tokens;
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

        private void TestFail(string text)
        {
            lexer.Tokenize(text);
        }

        [TestMethod]
        public void IdTest()
        {
            TestTokens("#window", new List<IToken>()
            {
                new SymbolToken('#'),
                new IdToken("window")
            });
        }

        [TestMethod]
        public void IdWithNumberTest()
        {
            TestTokens("#window1", new List<IToken>()
            {
                new SymbolToken('#'),
                new IdToken("window1")
            });
        }

        [TestMethod]
        public void TypeAndIdTest()
        {
            TestTokens("Window#window", new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('#'),
                new IdToken("window")
            });
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeLexerException))]
        public void TypeAndSpaceIdTest()
        {
            TestFail("Window# window");
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeLexerException))]
        public void TypeAndNumberIdTest()
        {
            TestFail("Window#1window");
        }

        [TestMethod]
        public void TypeAndCurlyBracketTest()
        {
            TestTokens("Window {}", new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new SymbolToken('}')
            });
        }

        [TestMethod]
        public void TypeAndRoundBracketTest()
        {
            TestTokens("BitmapImage(\"Icon.ico\")", new List<IToken>()
            {
                new TypeToken("BitmapImage"),
                new SymbolToken('('),
                new StringToken("Icon.ico"),
                new SymbolToken(')')
            });
        }

        [TestMethod]
        public void TypeAndSquareBracketTest()
        {
            TestTokens("TextBox []", new List<IToken>()
            {
                new TypeToken("TextBox"),
                new SymbolToken('['),
                new SymbolToken(']')
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
        public void StringTest()
        {
            TestTokens("\"Hello\"", new List<IToken>()
            {
                new StringToken("Hello")
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
        public void PropertyStringTest()
        {
            TestTokens("Title: \"Hello\"", new List<IToken>()
            {
                new PropertyToken("Title"),
                new SymbolToken(':'),
                new StringToken("Hello")
            });
        }

        [TestMethod]
        public void PropertyWordTest()
        {
            TestTokens("WindowState: Maximized", new List<IToken>()
            {
                new PropertyToken("WindowState"),
                new SymbolToken(':'),
                new WordToken("Maximized")
            });
        }

        [TestMethod]
        public void UsingAndNamespaceTest()
        {
            TestTokens("using System.Windows;", new List<IToken>()
            {
                new UsingToken(),
                new WordToken("System.Windows"),
                new SymbolToken(';')
            });
        }

        [TestMethod]
        public void PropertiesTest()
        {
            TestTokens("Width: 100,\r\nWindowState: Maximized", new List<IToken>()
            {
                new PropertyToken("Width"),
                new SymbolToken(':'),
                new NumberToken(100),
                new SymbolToken(','),
                new PropertyToken("WindowState"),
                new SymbolToken(':'),
                new WordToken("Maximized")
            });
        }

        [TestMethod]
        public void ShortBindingTest()
        {
            TestTokens("Title: @tb.Text", new List<IToken>()
            {
                new PropertyToken("Title"),
                new SymbolToken(':'),
                new SymbolToken('@'),
                new WordToken("tb.Text")
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullStr()
        {
            TestFail(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EmptyStr()
        {
            TestFail(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhiteSpaceStr()
        {
            TestFail("              ");
        }

        [TestMethod]
        public void AttachedPropertyTest()
        {
            TestTokens("Grid.Column: 0", new List<IToken>()
            {
                new PropertyToken("Grid.Column"),
                new SymbolToken(':'),
                new NumberToken(0)
            });
        }

        [TestMethod]
        public void CommentTest()
        {
            TestTokens("// comment\r\nusing System;", new List<IToken>()
            {
                new UsingToken(),
                new WordToken("System"),
                new SymbolToken(';')
            });
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeLexerException))]
        public void CommentFailTest()
        {
            TestFail("/ comment\r\nusing System;");
        }

    }

}
