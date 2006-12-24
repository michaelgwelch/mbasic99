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
    using LabelList = System.Collections.Generic.SortedList<string, Label>;

    abstract class Statement : Node
    {
        protected string label; // The actual string label from source (e.g. 100)
        Label lineLabel;        // A .NET label used to mark this location.

        protected Statement(int line)
            : base(line)
        {
            this.label = lexer.Label;
        }
        public abstract void CheckTypes();
        public virtual void RecordLabels(ILGenerator gen)
        {
            this.lineLabel = gen.DefineLabel();
            if (labels.ContainsKey(label)) return;

            labels.Add(label, lineLabel);
        }

        private static List<int> lines = new List<int>();
        protected void MarkSequencePoint(ILGenerator gen)
        {
            if (debug & lines.BinarySearch(line) < 0)
            {
                gen.MarkSequencePoint(writer, line, 1, line, 100);
                lines.Add(line);
            }
        }

        public void MarkLabel(ILGenerator gen)
        {
            gen.MarkLabel(lineLabel);
        }

        public abstract void Emit(ILGenerator gen, bool labelSetAlready);
        public override void Emit(ILGenerator gen)
        {
            Emit(gen, false);
        }
    }
}
