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
        public static List<FieldBuilder> fields;
        public static Lexer lexer;
        public static LabelList labels;
        public static List<Label> returnLabels = new List<Label>();// these are the labels that mark lines after gosub statements. 
        public static Label endLabel;
        public static Label returnSwitch;

        protected Node(LineId line) { this.line = line; }

        public abstract void Emit(ILGenerator gen);
        protected readonly LineId line;

        public static bool debug;
        public static ISymbolDocumentWriter writer;

        /// <summary>
        /// converts a boolean to a double
        /// </summary>
        /// <param name="gen"></param>
        protected static void EmitConvertToDouble(ILGenerator gen)
        {
            gen.Emit(OpCodes.Conv_R8);
            gen.Emit(OpCodes.Neg);
        }


        private static readonly MethodInfo convertToBooleanMethod =
            typeof(Convert).GetMethod("ToBoolean", new Type[] { typeof(double) });
        /// <summary>
        /// converts a double to a boolean
        /// </summary>
        /// <param name="gen"></param>
        protected static void EmitConvertToBoolean(ILGenerator gen)
        {
            gen.Emit(OpCodes.Call, convertToBooleanMethod);
        }
    }
}
