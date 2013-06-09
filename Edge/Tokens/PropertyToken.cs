using System;

namespace Edge.Tokens
{

    public class PropertyToken : IToken
    {

        private string property;

        public PropertyToken(string property)
        {
            this.property = property;
        }

        public override bool Equals(object obj)
        {
            var token = obj as PropertyToken;
            if (token != null && token.property == this.property)
                return true;

            return false;
        }

        public override string ToString()
        {
            return "Property: " + property;
        }

        public string Property
        {
            get
            {
                return property;
            }
        }

    }

}
