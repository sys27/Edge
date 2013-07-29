// Copyright 2013 Dmitry Kischenko
//
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either 
// express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
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
