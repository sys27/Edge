using System;

namespace Edge.SyntaxNodes
{

    public class ReferenceNode : INode
    {

        private string id;

        public ReferenceNode(string id)
        {
            this.id = id;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var reference = obj as ReferenceNode;
            if (reference == null)
                return false;

            return id == reference.id;
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
