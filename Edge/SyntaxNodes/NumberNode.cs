using System;

namespace Edge.SyntaxNodes
{

    public class NumberNode : INode
    {

        private double number;

        public NumberNode(double number)
        {
            this.number = number;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var num = obj as NumberNode;
            if (num == null)
                return false;

            return number == num.number;
        }

        public double Number
        {
            get
            {
                return number;
            }
        }

    }

}
