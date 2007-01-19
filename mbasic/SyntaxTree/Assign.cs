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
        // constant strings that represent either an INPUT or a READ statement.
        private const string readExpr = "read";
        private const string inputExpr = "input";
        Location location;
        Expression value;
        BasicType valueType;
        string builtIn;
        public Assign(Location loc, Expression value, LineId line)
            : base(line)
        {
            this.location = loc;
            this.value = value;
        }

        private Assign(Location loc, string builtIn, LineId line)
            : base(line)
        {
            this.location = loc;
            this.value = null;
            this.builtIn = builtIn;
        }

        public override void Emit(ILGenerator gen)
        {
            Emit(gen, false);
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);
            MarkSequencePoint(gen);

            location.EmitStore(gen, locals, value);
            return;
        }

        public override void CheckTypes()
        {
            location.ConstrainType(symbols);
            BasicType locationType = location.BasicType;
            if (locationType != BasicType.String && locationType != BasicType.Number
                && locationType != BasicType.Boolean) throw new Exception("type error");

            if (builtIn != null)
            {
                switch (builtIn)
                {
                    case readExpr:
                        value = (locationType == BasicType.String) ?
                            BuiltInsMethodCall.ReadStringFromData() : BuiltInsMethodCall.ReadNumberFromData();
                        break;
                    case inputExpr:
                        value = (locationType == BasicType.String) ?
                            BuiltInsMethodCall.ReadStringFromConsole() : BuiltInsMethodCall.ReadNumberFromConsole();
                        break;
                }
            }

            valueType = value.GetBasicType();

            if (locationType == BasicType.String && valueType == BasicType.String) return;

            if (valueType == BasicType.Number || valueType == BasicType.Boolean) return;

            throw new Exception("Type mismatch exception in an assignment");
            
        }


        public static Assign ReadData(Location loc, LineId line)
        {
            return new Assign(loc, readExpr, line);
        }

        public static Assign ReadConsole(Location loc, LineId line)
        {
            return new Assign(loc, inputExpr, line);
        }

    }
}
