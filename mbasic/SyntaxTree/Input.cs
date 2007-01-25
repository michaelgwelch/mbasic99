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
using TiBasicRuntime;

namespace mbasic.SyntaxTree
{
    class Input : Statement
    {

        private static readonly MethodInfo readLineMethod =
            typeof(BuiltIns).GetMethod("ReadLineFromConsoleIntoBuffer");

        Block inputs;
        Print inputPrompt;
        public Input(Expression inputPromptExpr, Block inputs, LineId line)
            : base(line)
        {
            this.inputPrompt = new Print(
                new Expression[] { inputPromptExpr, new StringLiteral("\0", line) }, line);
            this.inputs = inputs;
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);
            MarkSequencePoint(gen);

            inputPrompt.Emit(gen, true);

            Label begin = gen.DefineLabel();
            gen.MarkLabel(begin);
            gen.BeginExceptionBlock();

            // Expected number of inputs:
            gen.Emit(OpCodes.Ldc_I4, inputs.Length);
            gen.Emit(OpCodes.Call, readLineMethod); // reads a line from console into a buffer and checks to make sure number of values equals expected number of values.

            inputs.Emit(gen, true);

            gen.BeginCatchBlock(typeof(InvalidCastException));
            gen.Emit(OpCodes.Leave, begin);
            gen.EndExceptionBlock();
        }

        public override void CheckTypes()
        {
            inputPrompt.CheckTypes();
            inputs.CheckTypes();
        }


    }
}
