﻿// Copyright 2013 - 2015 Dmitry Kischenko
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

    public class NumberNode : IValueNode
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

            if (typeof(NumberNode) != obj.GetType())
                return false;

            var num = obj as NumberNode;

            return number == num.number;
        }

        public override int GetHashCode()
        {
            return 15121 ^ number.GetHashCode();
        }

        public override string ToString()
        {
            return $"Number: {number}";
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
