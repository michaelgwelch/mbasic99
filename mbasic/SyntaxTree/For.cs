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
    using StatementList = System.Collections.Generic.List<Statement>;

    internal class For : Statement
    {

        Assign init;
        Assign update;
        Expression comparison;
        Statement stmt;

        public For(Assign init, Expression comparison, Assign update, Statement stmt, LineId line)
            : base(line)
        {
            this.init = init;
            this.comparison = comparison;
            this.stmt = stmt;
            this.update = update;
        }

        public override void CheckTypes()
        {
            init.CheckTypes();
            if (comparison.GetBasicType() != BasicType.Number) throw new Exception("Type mismatch in comparison of for loop");
            update.CheckTypes();
            stmt.CheckTypes();
        }

        public override void Emit(ILGenerator gen)
        {
            Emit(gen, false);
        }
        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);

            Label condition = gen.DefineLabel();
            Label start = gen.DefineLabel();

            // Initialize
            init.Emit(gen, true);

            // Branch to condition label
            gen.Emit(OpCodes.Br, condition);

            // Mark start of loop
            gen.MarkLabel(start);

            // Execute loop
            stmt.Emit(gen);

            // Update counter
            update.Emit(gen);

            gen.MarkLabel(condition);
            comparison.Emit(gen);
            gen.Emit(OpCodes.Brtrue, start);

        }

        public override void RecordLabels(ILGenerator gen)
        {
            init.RecordLabels(gen);
            update.RecordLabels(gen);
            stmt.RecordLabels(gen);
        }

    }
}
