using Edge.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Edge
{

    public class EdgeLexer : ILexer
    {

        private readonly HashSet<char> brackets;

        public EdgeLexer()
        {
            brackets = new HashSet<char>() { '{', '}', '[', ']', '(', ')' };
        }

        //private bool IsBalanced(string str)
        //{
        //    int roundBrackets = 0;
        //    int squareBrackets = 0;
        //    int curlyBrackets = 0;
        //    int quotes = 0;

        //    foreach (var item in str)
        //    {
        //        switch (item)
        //        {
        //            case '{':
        //                curlyBrackets++;
        //                break;
        //            case '}':
        //                curlyBrackets--;
        //                break;
        //            case '(':
        //                roundBrackets++;
        //                break;
        //            case ')':
        //                roundBrackets--;
        //                break;
        //            case '[':
        //                squareBrackets++;
        //                break;
        //            case ']':
        //                squareBrackets--;
        //                break;
        //            case '"':
        //                quotes++;
        //                break;
        //        }

        //        if (curlyBrackets < 0 || roundBrackets < 0 || squareBrackets < 0)
        //            return false;
        //    }

        //    return curlyBrackets == 0 || roundBrackets == 0 || squareBrackets == 0 || quotes % 2 == 0;
        //}

        public IEnumerable<IToken> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException("text");
            //if (!IsBalanced(text))
            //    // todo: error message
            //    throw new EdgeLexerException();

            var tokens = new List<IToken>();

            for (int i = 0; i < text.Length; )
            {
                char peek = text[i];

                if (peek == ' ' || peek == '\n' || peek == '\r' || peek == '\t')
                {
                    i++;
                    continue;
                }
                if (char.IsLetter(peek))
                {
                    int length = 1;

                    for (int j = i + 1; j < text.Length && char.IsLetter(text[j]); j++)
                        length++;

                    string word = text.Substring(i, length);
                    i += length;

                    if (i < text.Length)
                    {
                        if (text[i] == '#')
                        {
                            tokens.Add(new TypeToken(word));
                            continue;
                        }
                        if (text[i] == ':')
                        {
                            tokens.Add(new PropertyToken(word));
                            continue;
                        }

                        if (text[i] == ' ')
                        {
                            do
                                i++;
                            while (text[i] == ' ');
                        }
                        if (brackets.Contains(text[i]))
                        {
                            tokens.Add(new TypeToken(word));
                            continue;
                        }
                        
                        if (word == "using")
                        {
                            tokens.Add(new UsingToken());
                            continue;
                        }
                    }

                    tokens.Add(new WordToken(word));
                    continue;
                }
                if (peek == '#')
                {
                    int length = 0;

                    i++;
                    if (i < text.Length && (char.IsLetter(text[i]) || text[i] == '_'))
                        length++;
                    else
                        // todo: error message
                        throw new EdgeLexerException();

                    for (int j = i + 2; j < text.Length && (char.IsLetter(text[j]) || char.IsDigit(text[j]) || text[j] == '_'); j++)
                        length++;

                    if (length == 0)
                        // todo: error message
                        throw new EdgeLexerException();

                    string word = text.Substring(i + 1, length);
                    i += length + 1;

                    tokens.Add(new SymbolToken('#'));
                    tokens.Add(new IdToken(word));

                    continue;
                }
                if (peek == ':' || brackets.Contains(peek) || peek == ';' || peek == '.' || peek == ',')
                {
                    tokens.Add(new SymbolToken(peek));
                    i++;

                    continue;
                }
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
