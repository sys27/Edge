using System;

namespace Edge.SyntaxNodes
{

    public class BindingNode : IValueNode
    {

        private string elementName;
        private string path;

        public BindingNode(string path)
            : this(null, path)
        {

        }

        public BindingNode(string elementName, string path)
        {
            this.elementName = elementName;
            this.path = path;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var binding = obj as BindingNode;
            if (binding == null)
                return false;

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

    }

}
