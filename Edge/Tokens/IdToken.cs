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
    
    public class IdToken : IToken
    {

        private string id;

        public IdToken(string id)
        {
            this.id = id;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (typeof(IdToken) != obj.GetType())
                return false;

            var token = obj as IdToken;

            return token.id == this.id;
        }

        public override string ToString()
        {
            return "Id: " + id;
        }

        public string Id
        {
            get
            {
                return id;
            }
        }

    }

}
