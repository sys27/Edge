using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edge.Tests
{

    [TestClass]
    public class EdgeParserTest
    {

        private ILexer lexer;
        private IParser parser;

        public EdgeParserTest()
        {
            lexer = new MockEdgeLexer();
            parser = new EdgeParser(lexer);
        }

    }

}
