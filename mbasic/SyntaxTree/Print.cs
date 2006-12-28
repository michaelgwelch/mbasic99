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
using TiBasicRuntime;
namespace mbasic.SyntaxTree
{
    internal class Print : Statement
    {
        static readonly MethodInfo printMethod = 
            typeof(BuiltIns).GetMethod("Print");

        Expression[] values;
        BasicType[] printItemTypes;
        public Print(Expression[] values, LineId line) : base(line)
        {
            this.values = values;
            printItemTypes = new BasicType[values.Length];
        }

        public override void Emit(ILGenerator gen)
        {
            Emit(gen, false);
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);
            MarkSequencePoint(gen);


            // create object array
            gen.Emit(OpCodes.Ldc_I4, values.Length);
            gen.Emit(OpCodes.Newarr, typeof(object));
            

            for (int i = 0; i < values.Length; i++)
            {
                gen.Emit(OpCodes.Dup); // Duplicate the array reference, so it's still on stack after this use
                gen.Emit(OpCodes.Ldc_I4, i);
                values[i].Emit(gen);
                if (printItemTypes[i] == BasicType.Number) gen.Emit(OpCodes.Box, typeof(double));
                gen.Emit(OpCodes.Stelem_Ref);
            }
            gen.Emit(OpCodes.Call, printMethod);

        }

        public override void CheckTypes()
        {
            for (int i = 0; i < values.Length; i++)
            {
                printItemTypes[i] = values[i].GetBasicType();
                if (printItemTypes[i] == BasicType.Error) throw new Exception("Bad type in print statement");
            }
        }




    }
}
