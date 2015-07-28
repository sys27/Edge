// Copyright 2013 - 2015 Dmitry Kischenko
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

namespace Edge.SyntaxNodes
{

    public class ReferenceNode : IValueNode
    {

        private readonly string type;
        private readonly string id;

        public ReferenceNode(string id, string type)
        {
            this.id = id;
            this.type = type;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (typeof(ReferenceNode) != obj.GetType())
                return false;

            var reference = obj as ReferenceNode;

            return id == reference.id && type == reference.type;
        }

        public override int GetHashCode()
        {
            return 21737 ^ id.GetHashCode();
        }

        public override string ToString()
        {
            return $"Reference: #{id} ({type})";
        }

        public string Type
        {
            get
            {
                return type;
            }
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
