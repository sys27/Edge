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
            var type = typeof(System.Windows.Window);
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
                    new RootObjectNode(type)
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(type.GetProperty("Width"), new NumberNode(1024.6))
                        })
                }));
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(type.GetProperty("Title"), new StringNode("Hello"))
                        })
                }));
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(type.GetProperty("Content"), new ReferenceNode("grid1"))
                        }),
                    new ObjectNode(typeof(System.Windows.Controls.Grid), "grid1")
                }));
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(type.GetProperty("WindowState"), new EnumNode(typeof(System.Windows.WindowState), System.Windows.WindowState.Maximized))
                        })
                }));
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(type.GetProperty("WindowState"), new EnumNode(typeof(System.Windows.WindowState), System.Windows.WindowState.Maximized))
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
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(type.GetProperty("Title"), new StringNode("Hello")),
                            new PropertyNode(type.GetProperty("Width"), new NumberNode(1024.6))
                        })
                }));
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new ObjectNode(typeof(System.Windows.Media.Imaging.BitmapImage), "bitmap"),
                    new RootObjectNode(
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(type.GetProperty("Icon"), new ReferenceNode("bitmap")),
                            new PropertyNode(type.GetProperty("Content"), new ReferenceNode("bitmap"))
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
            var type = typeof(System.Windows.Window);
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
                    new ObjectNode(typeof(System.Windows.Controls.Grid), "grid"),
                    new RootObjectNode(
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(type.GetProperty("Content"), new ReferenceNode("grid"))
                        })
                }));
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(type.GetProperty("Content"), new ReferenceNode("grid1"))
                        }),
                    new ObjectNode(typeof(System.Windows.Controls.Grid), "grid1")
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
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(type.GetProperty("Icon"), new ReferenceNode("bitmapImage1"))
                        }),
                    new ObjectNode(
                        typeof(System.Windows.Media.Imaging.BitmapImage),
                        "bitmapImage1",
                        new List<IValueNode>()
                        {
                            new ReferenceNode("uri1")
                        }),
                    new ObjectNode(
                        typeof(System.Uri),
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new ObjectNode(typeof(System.Windows.Controls.TextBox), "tb"),
                    new RootObjectNode(
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(type.GetProperty("Title"), new BindingNode("tb", "Text")),
                            new PropertyNode(type.GetProperty("Content"), new ReferenceNode("tb"))
                        })
                }));
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(type.GetProperty("Title"), new BindingNode("WindowState"))
                        })
                }));
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(
                                type.GetProperty("Content"), 
                                new ArrayNode(
                                    textBox, 
                                    new IValueNode[] 
                                    { 
                                        new ReferenceNode("textBox1"),
                                        new ReferenceNode("textBox2")
                                    }))
                        }),
                    new ObjectNode(textBox, "textBox1"), 
                    new ObjectNode(textBox, "textBox2") 
                }));
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(
                                type.GetProperty("Content"), 
                                new ArrayNode(
                                    textBox, 
                                    new IValueNode[] 
                                    { 
                                        new ReferenceNode("textBox1"),
                                        new ReferenceNode("textBox2")
                                    }))
                        }),
                    new ObjectNode(textBox, "textBox1"),
                    new ObjectNode(textBox, "textBox2")
                }));
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new ObjectNode(style, "baseStyle"),
                    new ObjectNode(brush, "newBrush"),
                    new RootObjectNode(
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(
                                type.GetProperty("Resources"), 
                                new ArrayNode(
                                    typeof(object), 
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
            new SyntaxTree(
                parser.Namespaces,
                new List<ObjectNode>()
                {
                    new RootObjectNode(
                        type,
                        new List<PropertyNode>()
                        {
                            new PropertyNode(
                                type.GetProperty("ColumnDefinitions"), 
                                new CollectionNode(
                                    typeof(System.Windows.Controls.ColumnDefinitionCollection),
                                    cd, 
                                    new IValueNode[] 
                                    { 
                                        new ReferenceNode("columnDefinition1"), 
                                        new ReferenceNode("columnDefinition2") 
                                    }))
                        }),
                    new ObjectNode(cd,"columnDefinition1"),
                    new ObjectNode(cd,"columnDefinition2")
                }));
        }

    }

}
