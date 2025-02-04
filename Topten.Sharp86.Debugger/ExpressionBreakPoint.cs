﻿// Sharp86 - 8086 Emulator
// Copyright © 2017-2021 Topten Software. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may 
// not use this product except in compliance with the License. You may obtain 
// a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
// License for the specific language governing permissions and limitations 
// under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topten.JsonKit;

namespace Topten.Sharp86
{
    class ExpressionBreakPoint : BreakPoint
    {
        public ExpressionBreakPoint()
        {
        }

        public ExpressionBreakPoint(Expression expression)
        {
            _expression = expression;
        }

        [Json("expression")]
        public string ExpressionText
        {
            get { return _expression == null ? null : _expression.OriginalExpression; }
            set { _expression = value == null ? null : new Expression(value); }
        }

        Expression _expression;
        object _prevValue;

        public override string ToString()
        {
            return base.ToString(string.Format("expr {0}", _expression == null ? "null" : _expression.OriginalExpression));
        }

        public override string EditString
        {
            get
            {
                return string.Format("expr {0}", _expression.OriginalExpression);
            }
        }

        public override bool ShouldBreak(DebuggerCore debugger)
        {
            var newValue = _expression.Eval(debugger.ExpressionContext);

            if (_prevValue == null)
            {
                _prevValue = newValue;
                return false;
            }

            bool changed = (bool)Operators.compare_ne(newValue, _prevValue);
            _prevValue = newValue;
            return changed;
        }
    }
}
