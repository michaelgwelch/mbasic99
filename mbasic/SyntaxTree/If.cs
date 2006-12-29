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
namespace mbasic.SyntaxTree
{
    class If : Statement
    {
        Goto jmp;
        Goto elseJmp;
        Expression conditional;
        BasicType exprType;
        public If(Expression conditional, string label, LineId line) : base(line)
        {
            jmp = new Goto(label, line);
            this.conditional = conditional;
        }

        public If(Expression conditional, string label, string elseLabel,
            LineId line)
            : base(line)
        {
            jmp = new Goto(label, line);
            elseJmp = new Goto(elseLabel, line);
            this.conditional = conditional;
        }

        public override void CheckTypes()
        {
            exprType = conditional.GetBasicType();
            
            if (conditional.GetBasicType() == BasicType.Number || 
                conditional.GetBasicType() == BasicType.Boolean) return;
            throw new Exception(String.Format("Type error in conditional of If on {0}", line.Label));
        }

        public override void Emit(ILGenerator gen)
        {
            Emit(gen, false);
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);
            MarkSequencePoint(gen);
            Label falseCase = gen.DefineLabel();
            conditional.Emit(gen);
            if (exprType == BasicType.Number) EmitConvertToBoolean(gen);
            gen.Emit(OpCodes.Brfalse_S, falseCase);
            jmp.Emit(gen, true);
            gen.MarkLabel(falseCase);
            if (elseJmp != null) elseJmp.Emit(gen, true);

        }
    }
}
