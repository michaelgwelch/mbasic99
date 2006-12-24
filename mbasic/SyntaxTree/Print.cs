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
    internal class Print : Statement
    {
        static readonly MethodInfo methodInfoString = 
            typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });

        static readonly MethodInfo methodInfoNum =
            typeof(Console).GetMethod("WriteLine", new Type[] { typeof(double) });

        Expression value;
        BasicType printItemType;
        public Print(Expression value, int line) : base(line)
        {
            this.value = value;
        }

        public override void Emit(ILGenerator gen)
        {
            Emit(gen, false);
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);
            value.Emit(gen);
            MarkSequencePoint(gen);
            if (printItemType == BasicType.String) gen.EmitCall(OpCodes.Call, methodInfoString, new Type[0]);
            else gen.EmitCall(OpCodes.Call, methodInfoNum, new Type[0]);

        }

        public override void CheckTypes()
        {
            printItemType = value.GetBasicType();
            if (printItemType == BasicType.Error) throw new Exception("Bad type in print statement");
        }




    }
}
