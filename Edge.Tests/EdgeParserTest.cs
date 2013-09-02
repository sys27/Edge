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

        private MockEdgeLexer lexer;
        private EdgeParser parser;

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
            new SyntaxTree(
                parser.Namespaces.Union(
                    new List<string>()
                    {
                        "System",
                        "System.Windows"
                    }
                ),
                new List<ObjectNode>()
                {
                    new RootObjectNode("Window")
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
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Width"),
                new SymbolToken(':'),
                new NumberToken(1024.6),
                new SymbolToken('}')
            },
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode("Width", new NumberNode(1024.6))
                        })
                }));
        }

        [TestMethod]
        public void ObjectWithStringPropertyTest()
        {
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Title"),
                new SymbolToken(':'),
                new StringToken("Hello"),
                new SymbolToken('}')
            },
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode("Title", new StringNode("Hello"))
                        })
                }));
        }

        [TestMethod]
        public void ObjectWithObjectPropertyTest()
        {
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode("Content", new ReferenceNode("grid1"))
                        }),
                    new ObjectNode("Grid", "grid1")
                }));
        }

        [TestMethod]
        public void ObjectWithEnumPropertyTest()
        {
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("WindowState"),
                new SymbolToken(':'),
                new WordToken("Maximized"),
                new SymbolToken('}')
            },
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode("WindowState", new EnumNode("WindowState", System.Windows.WindowState.Maximized))
                        })
                }));
        }

        [TestMethod]
        public void ObjectWithFullNameEnumPropertyTest()
        {
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode("WindowState", new EnumNode("WindowState", System.Windows.WindowState.Maximized))
                        })
                }));
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode("Title", new StringNode("Hello")),
                            new PropertyNode("Width", new NumberNode(1024.6))
                        })
                }));
        }

        [TestMethod]
        public void ObjectWithIdPropertyTest()
        {
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new ObjectNode("BitmapImage", "bitmap"),
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode("Icon", new ReferenceNode("bitmap")),
                            new PropertyNode("Content", new ReferenceNode("bitmap"))
                        })
                }));
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
        [ExpectedException(typeof(EdgeParserException))]
        public void RootObjectWithIdTest()
        {
            TestFail(new List<IToken>()
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
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Content"),
                new SymbolToken(':'),
                new TypeToken("Grid"),
                new SymbolToken('#'),
                new IdToken("grid"),
                new SymbolToken('}')
            },
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new ObjectNode("Grid", "grid"),
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode("Content", new ReferenceNode("grid"))
                        })
                }));
        }

        [TestMethod]
        public void PropertyObjectWithoutPropertiesTest()
        {
            Test(new List<IToken>()
            {
                new TypeToken("Window"),
                new SymbolToken('{'),
                new PropertyToken("Content"),
                new SymbolToken(':'),
                new WordToken("Grid"),
                new SymbolToken('}')
            },
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode("Content", new ReferenceNode("grid1"))
                        }),
                    new ObjectNode("Grid", "grid1")
                }));
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode("Icon", new ReferenceNode("bitmapImage1"))
                        }),
                    new ObjectNode(
                        "BitmapImage",
                        "bitmapImage1",
                        new List<IValueNode>()
                        {
                            new ReferenceNode("uri1")
                        }),
                    new ObjectNode(
                        "Uri",
                        "uri1",
                        new List<IValueNode>()
                        {
                            new StringNode("Icon.ico")
                        })
                }));
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
        public void BindingShortTest()
        {
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new ObjectNode("TextBox", "tb"),
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode("Title", new BindingNode("tb", "Text")),
                            new PropertyNode("Content", new ReferenceNode("tb"))
                        })
                }));
        }

        [TestMethod]
        public void BindingFullTest()
        {
            Test(new List<IToken>()
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
            },
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new ObjectNode("TextBox", "tb"),
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode("Title", new BindingNode("tb", "Text", BindingMode.OneTime)),
                            new PropertyNode("Content", new ReferenceNode("tb"))
                        })
                }));
        }

        [TestMethod]
        public void ShortBindingOnlyPathTest()
        {
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode("Title", new BindingNode("WindowState"))
                        })
                }));
        }

        [TestMethod]
        public void ArrayTest()
        {
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode(
                                "Content", 
                                new ArrayNode(
                                    "TextBox", 
                                    new IValueNode[] 
                                    { 
                                        new ReferenceNode("textBox1"),
                                        new ReferenceNode("textBox2")
                                    }))
                        }),
                    new ObjectNode("TextBox", "textBox1"), 
                    new ObjectNode("TextBox", "textBox2") 
                }));
        }

        [TestMethod]
        public void ArrayWithoutTypeTest()
        {
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode(
                                "Content", 
                                new ArrayNode(
                                    "TextBox", 
                                    new IValueNode[] 
                                    { 
                                        new ReferenceNode("textBox1"),
                                        new ReferenceNode("textBox2")
                                    }))
                        }),
                    new ObjectNode("TextBox", "textBox1"),
                    new ObjectNode("TextBox", "textBox2")
                }));
        }

        [TestMethod]
        public void WindowResourceTest()
        {
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new ObjectNode("Style", "baseStyle"),
                    new ObjectNode("Brush", "newBrush"),
                    new RootObjectNode(
                        "Window",
                        new List<PropertyNode>()
                        {
                            new PropertyNode(
                                "Resources", 
                                new CollectionNode(
                                    "ResourceDictionary",
                                    "Object", 
                                    new IValueNode[] 
                                    { 
                                        new ReferenceNode("baseStyle"),
                                        new ReferenceNode("newBrush")
                                    }))
                        })
                }));
        }

        [TestMethod]
        public void GridColumnDefinitionsTest()
        {
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        "Grid",
                        new List<PropertyNode>()
                        {
                            new PropertyNode(
                                "ColumnDefinitions", 
                                new CollectionNode(
                                    "ColumnDefinitionCollection",
                                    "ColumnDefinition", 
                                    new IValueNode[] 
                                    { 
                                        new ReferenceNode("columnDefinition1"), 
                                        new ReferenceNode("columnDefinition2") 
                                    }))
                        }),
                    new ObjectNode("ColumnDefinition","columnDefinition1"),
                    new ObjectNode("ColumnDefinition","columnDefinition2")
                }));
        }

    }

}
