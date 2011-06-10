/*******************************************************************************
    Copyright 2006 Michael Welch
    
    This file is part of MBasic99.

    MBasic99 is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    MBasic99 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MBasic99; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using TIBasicRuntime;

namespace mbasic.SyntaxTree
{
    class Tab : Expression
    {
        Expression expr;
        BasicType exprType;
        public Tab(Expression expr, LineId line)
            : base(line)
        {
            this.expr = expr;
        }
        public override BasicType GetBasicType()
        {
            exprType = expr.GetBasicType();
            if (exprType == BasicType.Number || exprType == BasicType.Boolean) return BasicType.String;
            return BasicType.Error;
        }

        private static readonly MethodInfo concatenateMethod =
            typeof(String).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
        private static readonly MethodInfo toStringMethod =
            typeof(Radix100).GetMethod("ToString", new Type[] { typeof(double) });

        public override void Emit(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldstr, "\t");
            expr.Emit(gen); // will load a double (or boolean) on the stack
            if (exprType == BasicType.Boolean) EmitConvertToDouble(gen);
            gen.Emit(OpCodes.Call, toStringMethod); // will convert number to string
            gen.Emit(OpCodes.Call, concatenateMethod); // will concatenate them into one string
        }
    }
}
