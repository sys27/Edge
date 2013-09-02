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
    
    public class StringToken : IToken
    {

        private string str;

        public StringToken(string str)
        {
            this.str = str;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (typeof(StringToken) != obj.GetType())
                return false;

            var token = obj as StringToken;

            return token.str == this.str;
        }

        public override string ToString()
        {
            return "String: " + str;
        }

        public string Str
        {
            get
            {
                return str;
            }
        }

    }

}
