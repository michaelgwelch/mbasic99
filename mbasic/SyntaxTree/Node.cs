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
using System.Diagnostics.SymbolStore;

namespace mbasic.SyntaxTree
{
    using LabelList = System.Collections.Generic.SortedList<string, Label>;
using System.Reflection;

    internal abstract class Node
    {
        public static SymbolTable symbols;
        public static List<LocalBuilder> locals;
        public static Lexer lexer;
        public static LabelList labels;
        public static Label endLabel;

        public abstract void Emit(ILGenerator gen);

        public static bool debug;
        public static ISymbolDocumentWriter writer;
        protected int line;
        protected Node(int lineNumber)
        {
            this.line = lineNumber;
        }




        protected void TypeMismtach()
        {
            string msg = String.Format("Type Mismatch on line {0}", line);
            throw new Exception(msg);
        }

        private static readonly MethodInfo writeMethod =
            typeof(Console).GetMethod("Write", new Type[] { typeof(string) });
        protected void EmitWrite(ILGenerator gen, String s)
        {
            gen.Emit(OpCodes.Call, writeMethod);
        }
    }
}
