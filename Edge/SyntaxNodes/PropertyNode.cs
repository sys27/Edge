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
using System.Reflection;

namespace Edge.SyntaxNodes
{
    
    public class PropertyNode : INode
    {

        private PropertyInfo propertyInfo;
        private object value;

        public PropertyNode(PropertyInfo propertyInfo, object value)
        {
            this.propertyInfo = propertyInfo;
            this.value = value;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var prop = obj as PropertyNode;
            if (prop == null)
                return false;

            return propertyInfo.Equals(prop.propertyInfo) && value.Equals(prop.value);
        }

        public PropertyInfo Info
        {
            get
            {
                return propertyInfo;
            }
        }

        public object Value
        {
            get
            {
                return value;
            }
        }

    }

}
