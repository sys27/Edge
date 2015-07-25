using Edge.SyntaxNodes;
using Edge.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edge.Tests
{

    [TestClass]
    public class EdgeParserTest
    {

        private readonly MockEdgeLexer lexer;
        private readonly EdgeParser parser;

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

        private void Test(IEnumerable<IToken> tokens, SyntaxTree expected)
        {
            lexer.Tokens = tokens;

            var tree = parser.Parse("-- // --");

            Assert.AreEqual(expected, tree);
        }

        private void TestFail(IEnumerable<IToken> tokens)
        {
            lexer.Tokens = tokens;

            parser.Parse("-- // --");
        }

        [TestMethod]
        public void NamespaceWithObjectTest()
        {
            var tokens = new List<IToken>
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
            };

            var namespaces = parser.Namespaces.Union(new List<string>
            {
                "System",
                "System.Windows"
            });
            var root = new RootObjectNode("Window");
            var objectNodes = new List<ObjectNode> { root };

            Test(tokens, new SyntaxTree(namespaces, root, objectNodes));
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeParserException))]
        public void UsingFailTest()
        {
            TestFail(new List<IToken>
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
            TestFail(new List<IToken>
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
            var root = new RootObjectNode("Window",
                                          new List<PropertyNode>
                                          {
                                              new PropertyNode("Width", new NumberNode(1024.6))
                                          });
            Test(new List<IToken>
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Width"),
                new SymbolToken(':'),
                new NumberToken(1024.6),
                new SymbolToken('}')
            },
            new SyntaxTree(parser.Namespaces, root, new List<ObjectNode> { root }));
        }

        [TestMethod]
        public void ObjectWithStringPropertyTest()
        {
            var tokens = new List<IToken>
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Title"),
                new SymbolToken(':'),
                new StringToken("Hello"),
                new SymbolToken('}')
            };

            var root = new RootObjectNode(
                "Window",
                new List<PropertyNode>
                {
                    new PropertyNode("Title", new StringNode("Hello"))
                });

            Test(tokens, new SyntaxTree(parser.Namespaces, root, new List<ObjectNode> { root }));
        }

        [TestMethod]
        public void ObjectWithObjectPropertyTest()
        {
            var tokens = new List<IToken>
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

            var root = new RootObjectNode(
                "Window",
                new List<PropertyNode>
                {
                    new PropertyNode("Content", new ReferenceNode("grid1","Grid"))
                });

            Test(tokens, new SyntaxTree(parser.Namespaces, root,
                new List<ObjectNode>
                {
                    root,
                    new ObjectNode("Grid", "grid1")
                }));
        }

        [TestMethod]
        public void ObjectWithEnumPropertyTest()
        {
            var tokens = new List<IToken>
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("WindowState"),
                new SymbolToken(':'),
                new WordToken("Maximized"),
                new SymbolToken('}')
            };

            var root = new RootObjectNode(
                "Window",
                new List<PropertyNode>
                {
                    new PropertyNode("WindowState", new EnumNode(null, "Maximized"))
                });
            Test(tokens, new SyntaxTree(parser.Namespaces, root,
                new List<ObjectNode>
                {
                    root
                }));
        }

        [TestMethod]
        public void ObjectWithFullNameEnumPropertyTest()
        {
            var tokens = new List<IToken>
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("WindowState"),
                new SymbolToken(':'),
                new WordToken("WindowState"),
                new SymbolToken('.'),
                new WordToken("Maximized"),
                new SymbolToken('}')
            };

            var root = new RootObjectNode(
                "Window",
                new List<PropertyNode>
                {
                    new PropertyNode("WindowState", new EnumNode("WindowState", "Maximized"))
                });

            Test(tokens, new SyntaxTree(parser.Namespaces, root,
                new List<ObjectNode>
                {
                    root
                }));
        }

        [TestMethod]
        public void ObjectWithPropertiesTest()
        {
            var tokens = new List<IToken>
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
            };

            var root = new RootObjectNode(
                "Window",
                new List<PropertyNode>
                {
                    new PropertyNode("Title", new StringNode("Hello")),
                    new PropertyNode("Width", new NumberNode(1024.6))
                });

            Test(tokens, new SyntaxTree(parser.Namespaces, root,
                new List<ObjectNode>
                {
                    root
                }));
        }

        [TestMethod]
        public void ObjectWithIdPropertyTest()
        {
            var tokens = new List<IToken>
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
            };

            var root = new RootObjectNode(
                "Window",
                new List<PropertyNode>
                {
                    new PropertyNode("Icon", new ReferenceNode("bitmap","BitmapImage")),
                    new PropertyNode("Content", new ReferenceNode("bitmap","BitmapImage"))
                });

            Test(tokens, new SyntaxTree(parser.Namespaces, root,
                new List<ObjectNode>
                {
                    new ObjectNode("BitmapImage", "bitmap"),
                    root
                }));
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeParserException))]
        public void ObjectFailTest()
        {
            TestFail(new List<IToken>
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
            TestFail(new List<IToken>
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
            TestFail(new List<IToken>
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
        public void RootObjectWithIdTest()
        {
            TestFail(new List<IToken>
            {
                new TypeToken("Window"),
                new SymbolToken('#'),
                new IdToken("mainWindow"),
                new SymbolToken('{'),
                new SymbolToken('}')
            });
        }

        [TestMethod]
        public void ObjectWithIdTest()
        {
            var tokens = new List<IToken>
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Content"),
                new SymbolToken(':'),
                new TypeToken("Grid"),
                new SymbolToken('#'),
                new IdToken("grid"),
                new SymbolToken('}')
            };

            var root = new RootObjectNode(
                "Window",
                new List<PropertyNode>
                {
                    new PropertyNode("Content", new ReferenceNode("grid","Grid"))
                });

            Test(tokens, new SyntaxTree(parser.Namespaces, root,
                new List<ObjectNode>
                {
                    new ObjectNode("Grid", "grid"),
                    root
                }));
        }

        [TestMethod]
        public void PropertyObjectWithoutPropertiesTest()
        {
            var tokens = new List<IToken>
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Content"),
                new SymbolToken(':'),
                new TypeToken("Grid"),
                new SymbolToken('('),
                new SymbolToken(')'),
                new SymbolToken('}')
            };

            var root = new RootObjectNode(
                "Window",
                new List<PropertyNode>
                {
                    new PropertyNode("Content", new ReferenceNode("grid1","Grid"))
                });

            Test(tokens, new SyntaxTree(parser.Namespaces, root,
                new List<ObjectNode>
                {
                    root,
                    new ObjectNode("Grid", "grid1")
                }));
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeParserException))]
        public void SamePropertiesTest()
        {
            TestFail(new List<IToken>
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
            var tokens = new List<IToken>
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
            };

            var root = new RootObjectNode(
                "Window",
                new List<PropertyNode>
                {
                    new PropertyNode("Icon", new ReferenceNode("bitmapImage1","BitmapImage"))
                });

            Test(tokens, new SyntaxTree(parser.Namespaces, root,
                new List<ObjectNode>
                {
                    root,
                    new ObjectNode(
                        "BitmapImage",
                        "bitmapImage1",
                        new List<IValueNode>
                        {
                            new ReferenceNode("uri1","Uri")
                        }),
                    new ObjectNode(
                        "Uri",
                        "uri1",
                        new List<IValueNode>
                        {
                            new StringNode("Icon.ico")
                        })
                }));
        }

        [TestMethod]
        [ExpectedException(typeof(EdgeParserException))]
        public void ObjectCtorCommaFailTest()
        {
            TestFail(new List<IToken>
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
            TestFail(new List<IToken>
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
        public void BindingShortTest()
        {
            var tokens = new List<IToken>
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
            };

            var root = new RootObjectNode(
                "Window",
                new List<PropertyNode>
                {
                    new PropertyNode("Title", new BindingNode("tb", "Text")),
                    new PropertyNode("Content", new ReferenceNode("tb","TextBox"))
                });

            Test(tokens, new SyntaxTree(parser.Namespaces, root,
                new List<ObjectNode>
                {
                    new ObjectNode("TextBox", "tb"),
                    root
                }));
        }

        [TestMethod]
        public void BindingFullTest()
        {
            var tokens = new List<IToken>
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Title"),
                new SymbolToken(':'),
                new SymbolToken('@'),
                new SymbolToken('('),
                new WordToken("Path"),
                new SymbolToken('='),
                new WordToken("Text"),
                new SymbolToken(','),
                new WordToken("ElementName"),
                new SymbolToken('='),
                new WordToken("tb"),
                new SymbolToken(','),
                new WordToken("Mode"),
                new SymbolToken('='),
                new WordToken("OneTime"),
                new SymbolToken(')'),
                new SymbolToken(','),
                new PropertyToken("Content"),
                new SymbolToken(':'),
                new TypeToken("TextBox"),
                new SymbolToken('#'),
                new IdToken("tb"),
                new SymbolToken('}')
            };

            var root = new RootObjectNode(
                "Window",
                new List<PropertyNode>
                {
                    new PropertyNode("Title", new BindingNode("tb", "Text", BindingMode.OneTime)),
                    new PropertyNode("Content", new ReferenceNode("tb","TextBox"))
                });

            Test(tokens, new SyntaxTree(parser.Namespaces, root,
                new List<ObjectNode>
                {
                    new ObjectNode("TextBox", "tb"),
                    root
                }));
        }

        [TestMethod]
        public void ShortBindingOnlyPathTest()
        {
            var tokens = new List<IToken>
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Title"),
                new SymbolToken(':'),
                new SymbolToken('@'),
                new WordToken("WindowState"),
                new SymbolToken('}')
            };

            var root = new RootObjectNode(
                "Window",
                new List<PropertyNode>
                {
                    new PropertyNode("Title", new BindingNode("WindowState"))
                });

            Test(tokens, new SyntaxTree(parser.Namespaces, root,
                new List<ObjectNode>
                {
                    root
                }));
        }

        [TestMethod]
        public void ArrayTest()
        {
            var tokens = new List<IToken>
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Content"),
                new SymbolToken(':'),
                new TypeToken("TextBox"),
                new SymbolToken('['),
                new TypeToken("TextBox"),
                new SymbolToken('('),
                new SymbolToken(')'),
                new SymbolToken(','),
                new TypeToken("TextBox"),
                new SymbolToken('('),
                new SymbolToken(')'),
                new SymbolToken(']'),
                new SymbolToken('}')
            };

            var root = new RootObjectNode(
                "Window",
                new List<PropertyNode>
                {
                    new PropertyNode(
                        "Content",
                        new ArrayNode(
                            "TextBox",
                            new IValueNode[]
                            {
                                new ReferenceNode("textBox1", "TextBox"),
                                new ReferenceNode("textBox2", "TextBox")
                            }))
                });

            Test(tokens, new SyntaxTree(parser.Namespaces, root,
                new List<ObjectNode>
                {
                    root,
                    new ObjectNode("TextBox", "textBox1"),
                    new ObjectNode("TextBox", "textBox2")
                }));
        }

        [TestMethod]
        public void ArrayWithoutTypeTest()
        {
            var tokens = new List<IToken>
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Content"),
                new SymbolToken(':'),
                new SymbolToken('['),
                new TypeToken("TextBox"),
                new SymbolToken('('),
                new SymbolToken(')'),
                new SymbolToken(','),
                new TypeToken("TextBox"),
                new SymbolToken('('),
                new SymbolToken(')'),
                new SymbolToken(']'),
                new SymbolToken('}')
            };

            var root = new RootObjectNode(
                "Window",
                new List<PropertyNode>
                {
                    new PropertyNode(
                        "Content",
                        new ArrayNode(
                            null,
                            new IValueNode[]
                            {
                                new ReferenceNode("textBox1", "TextBox"),
                                new ReferenceNode("textBox2", "TextBox")
                            }))
                });

            Test(tokens, new SyntaxTree(parser.Namespaces, root, new List<ObjectNode>
                {
                    root,
                    new ObjectNode("TextBox", "textBox1"),
                    new ObjectNode("TextBox", "textBox2")
                }));
        }

        [TestMethod]
        public void WindowResourceTest()
        {
            var tokens = new List<IToken>
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
            };

            var root = new RootObjectNode(
                "Window",
                new List<PropertyNode>
                {
                    new PropertyNode(
                        "Resources",
                        new ArrayNode(
                            null,
                            new IValueNode[]
                            {
                                new ReferenceNode("baseStyle", "Style"),
                                new ReferenceNode("newBrush", "Brush")
                            }))
                });

            Test(tokens, new SyntaxTree(parser.Namespaces, root,
                new List<ObjectNode>
                {
                    new ObjectNode("Style", "baseStyle"),
                    new ObjectNode("Brush", "newBrush"),
                    root
                }));
        }

        [TestMethod]
        public void GridColumnDefinitionsTest()
        {
            var tokens = new List<IToken>
            {
                new TypeToken("Grid"),
                new SymbolToken('{'),
                new PropertyToken("ColumnDefinitions"),
                new SymbolToken(':'),
                new SymbolToken('['),
                new TypeToken("ColumnDefinition"),
                new SymbolToken('('),
                new SymbolToken(')'),
                new SymbolToken(','),
                new TypeToken("ColumnDefinition"),
                new SymbolToken('('),
                new SymbolToken(')'),
                new SymbolToken(']'),
                new SymbolToken('}')
            };

            var root = new RootObjectNode(
                "Grid",
                new List<PropertyNode>
                {
                    new PropertyNode(
                        "ColumnDefinitions",
                        new ArrayNode(
                            null,
                            new IValueNode[]
                            {
                                new ReferenceNode("columnDefinition1", "ColumnDefinition"),
                                new ReferenceNode("columnDefinition2", "ColumnDefinition")
                            }))
                });

            Test(tokens, new SyntaxTree(parser.Namespaces, root,
                new List<ObjectNode>
                {
                    root,
                    new ObjectNode("ColumnDefinition","columnDefinition1"),
                    new ObjectNode("ColumnDefinition","columnDefinition2")
                }));
        }
    }

}
