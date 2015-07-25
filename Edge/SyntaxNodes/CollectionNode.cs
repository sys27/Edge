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

    public class CollectionNode : ArrayNode
    {

        private string collectionType;

        public CollectionNode(string collectionType, string elementType, IValueNode[] array)
            : base(elementType, array)
        {
            this.collectionType = collectionType;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (typeof(CollectionNode) != obj.GetType())
                return false;

            var col = obj as CollectionNode;

            return collectionType.Equals(col.collectionType) &&
                   ElementType.Equals(col.ElementType) &&
                   Array.SequenceEqual(col.Array);
        }

        public string CollectionType
        {
            get
            {
                return collectionType;
            }
        }

    }

}
