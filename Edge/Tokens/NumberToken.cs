using System;

namespace Edge.Tokens
{

    public class NumberToken : IToken
    {

        private double number;

        public NumberToken(double number)
        {
            this.number = number;
        }

        public override bool Equals(object obj)
        {
            var token = obj as NumberToken;
            if (token != null && token.number == this.number)
                return true;

            return false;
        }

        public override string ToString()
        {
            return "Number: " + number;
        }

        public double Number
        {
            get
            {
                return number;
            }
        }

    }

}
