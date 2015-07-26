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
    
    public class PropertyNode : INode
    {

        private string property;
        private IValueNode value;

        public PropertyNode(string property, IValueNode value)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            this.property = property;
            this.value = value;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (typeof(PropertyNode) != obj.GetType())
                return false;

            var prop = obj as PropertyNode;

            return property.Equals(prop.property) && value.Equals(prop.value);
        }

        public override string ToString()
        {
            return $"Property: {property} = {value.ToString()}";
        }

        public string Property
        {
            get
            {
                return property;
            }
        }

        public IValueNode Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

    }

}
