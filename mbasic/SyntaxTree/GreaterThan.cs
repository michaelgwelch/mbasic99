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
    class GreaterThan : RelationalExpression
    {
        
        public GreaterThan(Expression expr1, Expression expr2, bool not,
            LineId line)
            : base(expr1, expr2, not, line)
        {
        }

        protected override void EmitOperation(ILGenerator gen)
        {
            if (argType == BasicType.Number)
            {
                // Do a > comparison
                gen.Emit(OpCodes.Cgt);
            }
            else
            {
                // Do a > comparison
                gen.Emit(OpCodes.Call, compareMethod);
                gen.Emit(OpCodes.Ldc_I4_1);
                gen.Emit(OpCodes.Ceq);
            }

        }

    }
}
