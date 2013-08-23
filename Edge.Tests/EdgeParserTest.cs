using Edge.SyntaxNodes;
using Edge.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
                    "window1",
                    true),
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
                        new PropertyNode(type.GetProperty("Width"), new NumberNode(1024.6))
                    },
                    true)));
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
                        new PropertyNode(type.GetProperty("Title"), new StringNode("Hello"))
                    },
                    true)));
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
                    },
                    true)));
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
                        new PropertyNode(type.GetProperty("WindowState"), new EnumNode(typeof(System.Windows.WindowState), System.Windows.WindowState.Maximized))
                    },
                    true)));
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
                        new PropertyNode(type.GetProperty("WindowState"), new EnumNode(typeof(System.Windows.WindowState), System.Windows.WindowState.Maximized))
                    },
                    true)));
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
                        new PropertyNode(type.GetProperty("Title"), new StringNode("Hello")),
                        new PropertyNode(type.GetProperty("Width"), new NumberNode(1024.6))
                    },
                    true)));
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
                    },
                    true)));
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
                new PropertyToken("Column"),
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
                    "mainWindow",
                    true)));
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
                    },
                    true)));
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeParserException))]
        public void SamePropertiesTest()
        {
            TestFail(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Title"),
                new SymbolToken(':'),
                new StringToken("Hello"),
                new SymbolToken(','),
                new PropertyToken("Title"),
                new SymbolToken(':'),
                new StringToken("Hello!!!"),
                new SymbolToken('}')
            });
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
                                new List<IValueNode>()
                                {
                                    new ObjectNode(
                                        typeof(System.Uri),
                                        "uri1",
                                        new List<IValueNode>()
                                        {
                                            new StringNode("Icon.ico")
                                        })
                                }))
                    },
                    true)));
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
                    },
                    true)));
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
                    },
                    true)));
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
                                textBox, 
                                new object[] 
                                { 
                                    new ObjectNode(textBox, "textBox1"), 
                                    new ObjectNode(textBox, "textBox2") 
                                }))
                    },
                    true)));
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
                                textBox, 
                                new object[] 
                                { 
                                    new ObjectNode(textBox, "textBox1"), 
                                    new ObjectNode(textBox, "textBox2") 
                                }))
                    },
                    true)));
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
                                typeof(object), 
                                new object[] 
                                { 
                                    new ObjectNode(style, "baseStyle"), 
                                    new ObjectNode(brush, "newBrush") 
                                }))
                    },
                    true)));
        }

        [TestMethod]
        public void GridColumnDefinitionsTest()
        {
            var type = typeof(System.Windows.Controls.Grid);
            var cd = typeof(System.Windows.Controls.ColumnDefinition);

            Test(new List<IToken>()
            {
                new TypeToken("Grid"),
                new SymbolToken('{'),
                new PropertyToken("ColumnDefinitions"),
                new SymbolToken(':'),
                new SymbolToken('['),
                new TypeToken("ColumnDefinition"),
                new SymbolToken(','),
                new TypeToken("ColumnDefinition"),
                new SymbolToken(']'),
                new SymbolToken('}')
            },
            new RootNode(
                new ObjectNode(
                    type,
                    "grid1",
                    new List<PropertyNode>()
                    {
                        new PropertyNode(
                            type.GetProperty("ColumnDefinitions"), 
                            new ArrayNode(
                                cd, 
                                new object[] 
                                { 
                                    new ObjectNode(cd, "columnDefinition1"), 
                                    new ObjectNode(cd, "columnDefinition2") 
                                }))
                    },
                    true)));
        }

    }

}
