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

        public override bool Equals(object obj)
        {
            var token = obj as WordToken;
            if (token != null && token.word == this.word)
                return true;

            return false;
        }

        public override string ToString()
        {
            return "Word: " + word;
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
