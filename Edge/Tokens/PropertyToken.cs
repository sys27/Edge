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

        public string Property
        {
            get
            {
                return property;
            }
        }

    }

}
