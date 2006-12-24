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
using System.Reflection;
using System.Reflection.Emit;
namespace mbasic.SyntaxTree
{
    class Input : Statement
    {
        private static readonly MethodInfo readLineMethod =
            typeof(Console).GetMethod("ReadLine");

        private static readonly MethodInfo tryParseMethod =
            typeof(double).GetMethod("TryParse",
            new Type[] { typeof(string), Type.GetType("System.Double&") });
        int index;
        BasicType varType;
        public Input(int index, int line) : base(line)
        {
            this.index = index; 
        }

        public override void Emit(ILGenerator gen)
        {
            Emit(gen, false);
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);
            Label readLine = gen.DefineLabel();
            MarkSequencePoint(gen);
            gen.MarkLabel(readLine);
            gen.EmitCall(OpCodes.Call, readLineMethod, new Type[0]);
            if (varType == BasicType.Number)
            {
                // Try to parse it into a double, if not possible give a warning and read again.
                gen.Emit(OpCodes.Ldloca, (short) index);
                gen.EmitCall(OpCodes.Call, tryParseMethod, new Type[0]);
                // if return value is false, the parse failed. go back to read line
                gen.Emit(OpCodes.Brfalse_S, readLine);

                // Else it was successful, the local variable should have the value.
                return;
            }
            else gen.Emit(OpCodes.Stloc, locals[index]);

        }

        public override void CheckTypes()
        {
            Type t = locals[index].LocalType;

            if (t == typeof(string))varType = BasicType.String;
            else varType = BasicType.Number;


        }

    }
}
