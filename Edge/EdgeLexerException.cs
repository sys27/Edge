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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Edge
{

    [Serializable]
    public class EdgeLexerException : Exception
    {

        public EdgeLexerException() { }

        public EdgeLexerException(string message) : base(message) { }

        public EdgeLexerException(string message, Exception inner) : base(message, inner) { }

        protected EdgeLexerException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }

}
