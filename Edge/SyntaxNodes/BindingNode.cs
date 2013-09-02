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
using Edge.Builders;
using System;

namespace Edge.SyntaxNodes
{

    public class BindingNode : IValueNode
    {

        private string elementName;
        private string path;
        private BindingMode mode;

        public BindingNode(string path)
            : this(null, path)
        {

        }

        public BindingNode(string elementName, string path)
            : this(elementName, path, BindingMode.Default)
        {
        }

        public BindingNode(string elementName, string path, BindingMode mode)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            this.elementName = elementName;
            this.path = path;
            this.mode = mode;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (typeof(BindingNode) != obj.GetType())
                return false;

            var binding = obj as BindingNode;

            return elementName == binding.elementName && path == binding.path;
        }

        public string ElementName
        {
            get
            {
                return elementName;
            }
        }

        public string Path
        {
            get
            {
                return path;
            }
        }

        public BindingMode Mode
        {
            get
            {
                return mode;
            }
        }

    }

}
