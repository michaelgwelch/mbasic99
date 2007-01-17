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
    class Assign : Statement
    {
        int localIndex;
        Expression value;
        BasicType valueType;
        public Assign(int index, Expression value, LineId line)
            : base(line)
        {
            this.localIndex = index;
            this.value = value;
        }

        public override void Emit(ILGenerator gen)
        {
            Emit(gen, false);
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);
            MarkSequencePoint(gen);
            value.Emit(gen);
            if (valueType == BasicType.Boolean) EmitConvertToDouble(gen);
            gen.Emit(OpCodes.Stloc, locals[localIndex]);
        }

        public override void CheckTypes()
        {
            Variable var = symbols[localIndex];
            var.ConstrainType();
            BasicType varBasicType = var.BasicType;

            valueType = value.GetBasicType();

            if (varBasicType == BasicType.String && valueType == BasicType.String) return;

            if (valueType == BasicType.Number || valueType == BasicType.Boolean) return;

            throw new Exception("Type mismatch exception in an assignment");
            
        }


        public static Assign ReadStringFromData(int symbolIndex, LineId line)
        {
            return new Assign(symbolIndex, BuiltInsMethodCall.ReadStringFromData(), line);
        }

        public static Assign ReadNumberFromData(int symbolIndex, LineId line)
        {
            return new Assign(symbolIndex, BuiltInsMethodCall.ReadNumberFromData(), line);
        }

        public static Assign ReadStringFromConsole(int symbolIndex, LineId line)
        {
            return new Assign(symbolIndex, BuiltInsMethodCall.ReadStringFromConsole(), line);
        }

        public static Assign ReadNumberFromConsole(int symbolIndex, LineId line)
        {
            return new Assign(symbolIndex, BuiltInsMethodCall.ReadNumberFromConsole(), line);
        }

    }
}
