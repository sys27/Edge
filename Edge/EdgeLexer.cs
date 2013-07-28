using Edge.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Edge
{

    public class EdgeLexer : ILexer
    {

        public IEnumerable<IToken> Tokenize(string text)
        {
            var tokens = new List<IToken>();

            for (int i = 0; i < text.Length; )
            {
                char peek = text[i];
                if (peek == '"')
                {
                    int length = 0;

                    for (int j = i + 1; j < text.Length && text[j] != '"'; j++)
                        length++;

                    string str = text.Substring(i + 1, length);
                    tokens.Add(new StringToken(str));

                    i += length + 2;
                    continue;
                }
                if (char.IsDigit(peek))
                {
                    int length = 1;
                    int j;

                    for (j = i + 1; j < text.Length && char.IsDigit(text[j]); j++)
                        length++;

                    if (j < text.Length && text[j] == '.')
                    {
                        length++;
                        for (j += 1; j < text.Length && char.IsDigit(text[j]); j++)
                            length++;
                    }

                    string strNumber = text.Substring(i, length);
                    double number = double.Parse(strNumber, CultureInfo.InvariantCulture);

                    tokens.Add(new NumberToken(number));

                    i += length;
                    continue;
                }

                i++;
            }

            return tokens;
        }

    }

}
