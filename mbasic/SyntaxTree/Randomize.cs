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
    class Randomize : Statement
    {
        private static readonly Type builtinsType = typeof(BuiltIns);
        private static readonly MethodInfo randomize =
            builtinsType.GetMethod("Randomize");
        private static readonly MethodInfo randomizeWithSeed =
            builtinsType.GetMethod("RandomizeWithSeed");

        Expression seedExpression;
        BasicType exprType;
        bool seedSpecified;

        public Randomize(LineId line) : base(line) 
        {
            seedSpecified = false;
        }

        public Randomize(Expression seedExpression, LineId line)
            : base(line)
        {
            this.seedExpression = seedExpression;
            seedSpecified = true;
        }

        public override void CheckTypes()
        {
            if (seedSpecified)
            {
                exprType = seedExpression.GetBasicType();
                if (exprType == BasicType.Number || exprType == BasicType.Boolean) return;
                throw new Exception("Type error");
            }
        }

        public override void Emit(ILGenerator gen)
        {
            Emit(gen, false);
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);
            MarkSequencePoint(gen);
            if (seedSpecified)
            {
                seedExpression.Emit(gen);
                if (exprType == BasicType.Boolean) EmitConvertToDouble(gen);

                gen.Emit(OpCodes.Call, randomizeWithSeed);
            }
            else
            {
                gen.Emit(OpCodes.Call, randomize);
            }
        }
    }
}
