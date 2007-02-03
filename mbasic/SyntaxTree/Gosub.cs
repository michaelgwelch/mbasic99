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
    class Gosub : Statement
    {
        private static readonly MethodInfo pushMethod =
            typeof(BuiltIns).GetMethod("PushReturnAddress");

        string destLabel; // This is the label identifier we jump to
        Label returnLabel; // This is the Label the subroutine should return to.
        int gosubIndex;
        public Gosub(string destLabel, LineId line)
            : base(line)
        {
            this.destLabel = destLabel;
        }

        public override void CheckTypes()
        {
            if (!labels.ContainsKey(destLabel))
            {
                throw new TypeCheckException(
                    String.Format("Non existent line number {0} in Gosub statement", destLabel),
                    line);
            }
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);
            MarkSequencePoint(gen);
            gen.Emit(OpCodes.Ldc_I4, gosubIndex);
            gen.Emit(OpCodes.Call, pushMethod);
            Label dest = labels[destLabel];
            MarkSequencePoint(gen);
            gen.Emit(OpCodes.Br, dest);
            gen.MarkLabel(returnLabel);// Mark the target to return to.
        }

        public override void RecordLabels(ILGenerator gen)
        {
            base.RecordLabels(gen);
            returnLabel = gen.DefineLabel();
            returnLabels.Add(returnLabel);
            gosubIndex = returnLabels.Count - 1;
        }
    }
}
