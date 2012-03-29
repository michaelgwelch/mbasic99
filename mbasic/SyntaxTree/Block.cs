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

namespace mbasic.SyntaxTree
{
    using StatementList = System.Collections.Generic.List<Statement>;
    using System.Reflection.Emit;

    internal class Block : Statement
    {
        StatementList stmts;
        public Block(StatementList stmts)
            : base(LineId.None)
        {
            this.stmts = stmts;
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            foreach (Statement stmt in stmts) stmt.Emit(gen, labelSetAlready);

        }

        public override void Emit(ILGenerator gen)
        {
            Emit(gen, false);
        }

        public override void CheckTypes()
        {
            foreach (Statement stmt in stmts) stmt.CheckTypes();
        }

        public override void RecordLabels(ILGenerator gen)
        {
            foreach (Statement stmt in stmts) stmt.RecordLabels(gen);
        }

        public int Length { get { return stmts.Count; } }
    }
}