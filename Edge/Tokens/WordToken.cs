using System;

namespace Edge.Tokens
{
    
    public class WordToken : IToken
    {

        private string word;

        public WordToken(string word)
        {
            this.word = word;
        }

        public string Word
        {
            get
            {
                return word;
            }
        }

    }

}
