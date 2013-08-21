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
using System.Linq;

namespace Edge.SyntaxNodes
{

    public class ArrayNode : IValueNode
    {

        private Type arrayType;
        private object[] array;

        public ArrayNode(Type arrayType, object[] array)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            this.arrayType = arrayType;
            this.array = array;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var arr = obj as ArrayNode;
            if (arr == null)
                return false;

            return arrayType.Equals(arr.arrayType) && array.SequenceEqual(arr.array);
        }

        public Type ArrayType
        {
            get
            {
                return arrayType;
            }
        }

        public object[] Array
        {
            get
            {
                return array;
            }
        }

    }

}
