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

        private void Test(IEnumerable<IToken> tokens, RootNode expected)
        {
            lexer.Tokens = tokens;

            var root = parser.Parse("-- // --");

            Assert.AreEqual(expected, root);
        }

        private void TestFail(IEnumerable<IToken> tokens)
        {
            lexer.Tokens = tokens;

            var root = parser.Parse("-- // --");
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
                new WordToken("System.Windows"),
                new SymbolToken(';'),
                new TypeToken("Window"),
                new SymbolToken('{'),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    typeof(System.Windows.Window),
                    "window1"),
                new List<NamespaceNode>()
                {
                    new NamespaceNode("System"),
                    new NamespaceNode("System.Windows")
                }));
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeParserException))]
        public void UsingFailTest()
        {
            TestFail(new List<IToken>()
            {
                new UsingToken(),
                new TypeToken("Window"),
                new SymbolToken('{'),
                new SymbolToken('}')
            });
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeParserException))]
        public void NamespaceWithEmptyWordTest()
        {
            TestFail(new List<IToken>()
            {
                new UsingToken(),
                new WordToken(""),
                new SymbolToken(';'),
                new TypeToken("Window"),
                new SymbolToken('{'),
                new SymbolToken('}')
            });
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
                    "window1",
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
                    "window1",
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
                    "window1",
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("Content"), new ObjectNode(typeof(System.Windows.Controls.Grid), "grid1"))
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
                    "window1",
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("WindowState"), System.Windows.WindowState.Maximized)
                    })));
        }

        [TestMethod]
        public void ObjectWithFullNameEnumPropertyTest()
        {
            var type = typeof(System.Windows.Window);
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("WindowState"),
                new SymbolToken(':'),
                new WordToken("WindowState"),
                new SymbolToken('.'),
                new WordToken("Maximized"),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    "window1",
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("WindowState"), System.Windows.WindowState.Maximized)
                    })));
        }

        [TestMethod]
        public void ObjectWithPropertiesTest()
        {
            var type = typeof(System.Windows.Window);
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Title"),
                new SymbolToken(':'),
                new StringToken("Hello"),
                new SymbolToken(','),
                new PropertyToken("Width"),
                new SymbolToken(':'),
                new NumberToken(1024.6),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    "window1",
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("Title"), "Hello"),
                        new PropertyNode(type.GetProperty("Width"), 1024.6)
                    })));
        }

        [TestMethod]
        public void ObjectWithIdPropertyTest()
        {
            var type = typeof(System.Windows.Window);
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Icon"),
                new SymbolToken(':'),
                new TypeToken("BitmapImage"),
                new SymbolToken('#'),
                new IdToken("bitmap"),
                new SymbolToken(','),
                new PropertyToken("Content"),
                new SymbolToken(':'),
                new SymbolToken('#'),
                new IdToken("bitmap"),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    "window1",
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("Icon"), new ObjectNode(typeof(System.Windows.Media.Imaging.BitmapImage), "bitmap")),
                        new PropertyNode(type.GetProperty("Content"), new ReferenceNode("bitmap"))
                    })));
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeParserException))]
        public void ObjectFailTest()
        {
            TestFail(new List<IToken>()
            {
                new WordToken("Window"),
                new SymbolToken('{'),
                new SymbolToken('}')
            });
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeParserException))]
        public void PropertyAsWordTest()
        {
            TestFail(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new WordToken("Width"),
                new SymbolToken(':'),
                new NumberToken(1024),
                new SymbolToken('}')
            });
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeParserException))]
        public void PropertyWithCommaTest()
        {
            TestFail(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new WordToken("Width"),
                new SymbolToken(':'),
                new NumberToken(1024),
                new SymbolToken(','),
                new SymbolToken('}')
            });
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeParserException))]
        public void PropertyIsNotInTypeTest()
        {
            TestFail(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new WordToken("Column"),
                new SymbolToken(':'),
                new NumberToken(1),
                new SymbolToken('}')
            });
        }

        [TestMethod]
        public void ObjectWithIdTest()
        {
            var type = typeof(System.Windows.Window);
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('#'),
                new IdToken("mainWindow"),
                new SymbolToken('{'),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    "mainWindow")));
        }

        [TestMethod]
        public void PropertyObjectWithoutPropertiesTest()
        {
            var type = typeof(System.Windows.Window);
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Content"),
                new SymbolToken(':'),
                new WordToken("Grid"),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    "window1",
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("Content"), new ObjectNode(typeof(System.Windows.Controls.Grid), "grid1"))
                    })));
        }

        [TestMethod]
        public void ObjectCtorTest()
        {
            var type = typeof(System.Windows.Window);
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Icon"),
                new SymbolToken(':'),
                new TypeToken("BitmapImage"),
                new SymbolToken('('),
                new TypeToken("Uri"),
                new SymbolToken('('),
                new StringToken("Icon.ico"),
                new SymbolToken(')'),
                new SymbolToken(')'),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    "window1",
                    new List<PropertyNode>()
                    {
                        new PropertyNode(
                            type.GetProperty("Icon"),
                            new ObjectNode(
                                typeof(System.Windows.Media.Imaging.BitmapImage), 
                                "bitmapImage1", 
                                new List<object>()
                                {
                                    new ObjectNode(
                                        typeof(System.Uri),
                                        "uri1",
                                        new List<object>()
                                        {
                                            "Icon.ico"
                                        })
                                }))
                    })));
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeParserException))]
        public void ObjectCtorCommaFailTest()
        {
            TestFail(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Icon"),
                new SymbolToken(':'),
                new TypeToken("BitmapImage"),
                new SymbolToken('('),
                new TypeToken("Uri"),
                new SymbolToken('('),
                new StringToken("Icon.ico"),
                new SymbolToken(')'),
                new SymbolToken(','),
                new SymbolToken(')'),
                new SymbolToken('}')
            });
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeParserException))]
        public void ObjectCtorWrongTokenTest()
        {
            TestFail(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Icon"),
                new SymbolToken(':'),
                new TypeToken("BitmapImage"),
                new SymbolToken('('),
                new IdToken("fail"),
                new SymbolToken(')'),
                new SymbolToken('}')
            });
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeParserException))]
        public void ObjectCtorWrongArgsTest()
        {
            TestFail(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Icon"),
                new SymbolToken(':'),
                new TypeToken("BitmapImage"),
                new SymbolToken('('),
                new NumberToken(1024),
                new SymbolToken(')'),
                new SymbolToken('}')
            });
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeParserException))]
        public void TypeInNotNamespaceTest()
        {
            TestFail(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Content"),
                new SymbolToken(':'),
                new TypeToken("ArrayList"),
                new SymbolToken('}')
            });
        }

        [TestMethod]
        public void ShortBindingTest()
        {
            var type = typeof(System.Windows.Window);
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Title"),
                new SymbolToken(':'),
                new SymbolToken('@'),
                new WordToken("tb.Text"),
                new SymbolToken(','),
                new PropertyToken("Content"),
                new SymbolToken(':'),
                new TypeToken("TextBox"),
                new SymbolToken('#'),
                new IdToken("tb"),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    "window1",
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("Title"), new BindingNode("tb", "Text")),
                        new PropertyNode(type.GetProperty("Content"), new ObjectNode(typeof(System.Windows.Controls.TextBox), "tb"))
                    })));
        }

        [TestMethod]
        public void ShortBindingOnlyPathTest()
        {
            var type = typeof(System.Windows.Window);
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Title"),
                new SymbolToken(':'),
                new SymbolToken('@'),
                new WordToken("WindowState"),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    "window1",
                    new List<PropertyNode>()
                    {
                        new PropertyNode(type.GetProperty("Title"), new BindingNode("WindowState"))
                    })));
        }

        [TestMethod]
        public void ArrayTest()
        {
            var type = typeof(System.Windows.Window);
            var textBox = typeof(System.Windows.Controls.TextBox);
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Content"),
                new SymbolToken(':'),
                new TypeToken("TextBox"),
                new SymbolToken('['),
                new WordToken("TextBox"),
                new SymbolToken(','),
                new WordToken("TextBox"),
                new SymbolToken(']'),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    "window1",
                    new List<PropertyNode>()
                    {
                        new PropertyNode(
                            type.GetProperty("Content"), 
                            new ArrayNode(
                                textBox.MakeArrayType(), 
                                new object[] 
                                { 
                                    new ObjectNode(textBox, "textBox1"), 
                                    new ObjectNode(textBox, "textBox2") 
                                }))
                    })));
        }

        [TestMethod]
        public void ArrayWithoutTypeTest()
        {
            var type = typeof(System.Windows.Window);
            var textBox = typeof(System.Windows.Controls.TextBox);
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Content"),
                new SymbolToken(':'),
                new SymbolToken('['),
                new WordToken("TextBox"),
                new SymbolToken(','),
                new WordToken("TextBox"),
                new SymbolToken(']'),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    "window1",
                    new List<PropertyNode>()
                    {
                        new PropertyNode(
                            type.GetProperty("Content"), 
                            new ArrayNode(
                                textBox.MakeArrayType(), 
                                new object[] 
                                { 
                                    new ObjectNode(textBox, "textBox1"), 
                                    new ObjectNode(textBox, "textBox2") 
                                }))
                    })));
        }

        [TestMethod]
        public void WindowResourceTest()
        {
            var type = typeof(System.Windows.Window);
            var style = typeof(System.Windows.Style);
            var brush = typeof(System.Windows.Media.Brush);

            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Resources"),
                new SymbolToken(':'),
                new SymbolToken('['),
                new TypeToken("Style"),
                new SymbolToken('#'),
                new IdToken("baseStyle"),
                new SymbolToken(','),
                new TypeToken("Brush"),
                new SymbolToken('#'),
                new IdToken("newBrush"),
                new SymbolToken(']'),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    "window1",
                    new List<PropertyNode>()
                    {
                        new PropertyNode(
                            type.GetProperty("Resources"), 
                            new ArrayNode(
                                typeof(object[]), 
                                new object[] 
                                { 
                                    new ObjectNode(style, "baseStyle"), 
                                    new ObjectNode(brush, "newBrush") 
                                }))
                    })));
        }

    }

}
