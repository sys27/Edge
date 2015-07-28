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
using System.Linq;

namespace Edge.SyntaxNodes
{

    public class ArrayNode : IValueNode
    {

        private string elementType;
        protected IValueNode[] array;

        public ArrayNode(string elementType, IValueNode[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            this.elementType = elementType;
            this.array = array;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (typeof(ArrayNode) != obj.GetType())
                return false;

            var arr = obj as ArrayNode;

            return ((elementType == null && arr.elementType == null) ||
                    (elementType != null && arr.elementType != null && elementType.Equals(arr.elementType))) &&
                   array.SequenceEqual(arr.array);
        }

        public override int GetHashCode()
        {
            var hash = 104659;

            hash = hash * 75781 + elementType.GetHashCode();
            hash = hash * 75781 + array.Length.GetHashCode();

            return hash;
        }

        public override string ToString()
        {
            return $"Array: [{string.Join<IValueNode>(", ", array)}]";
        }

        public string ElementType
        {
            get
            {
                return elementType;
            }
        }

        public IValueNode[] Array
        {
            get
            {
                return array;
            }
        }

    }

}
