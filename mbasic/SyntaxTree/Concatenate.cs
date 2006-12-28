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

namespace mbasic.SyntaxTree
{
    class Concatenate : Expression
    {
        private static readonly MethodInfo concatMethod =
            typeof(String).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
        Expression s1;
        Expression s2;
        BasicType type;
        public Concatenate(Expression s1, Expression s2, LineId line)
            : base(line)
        {
            this.s1 = s1;
            this.s2 = s2;
        }

        public override BasicType GetBasicType()
        {
            if (s1.GetBasicType() == BasicType.String &&
                s2.GetBasicType() == BasicType.String)
            {
                type = BasicType.String;
                return type;
            }
            return BasicType.Error;
        }

        public override void Emit(ILGenerator gen)
        {
            s1.Emit(gen);
            s2.Emit(gen);
            gen.EmitCall(OpCodes.Call, concatMethod, new Type[0]);
        }
    }
}
