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

namespace mbasic.SyntaxTree
{
    abstract class RelationalExpression : Expression
    {
        Expression e1;
        Expression e2;
        protected BasicType argType;
        bool not;

        protected static readonly MethodInfo compareMethod =
            typeof(string).GetMethod("Compare",
            new Type[] { typeof(string), typeof(string) });

        protected RelationalExpression(Expression e1,
            Expression e2, bool not, int line)
            : base(line)
        {
            this.e1 = e1;
            this.e2 = e2;
            this.not = not;
        }
        public override BasicType GetBasicType()
        {
            BasicType t1 = e1.GetBasicType();
            BasicType t2 = e2.GetBasicType();
            if (t1 == t2)
            {
                argType = t1;
                return BasicType.Number;
            }
            return BasicType.Error;
        }

        public override void Emit(ILGenerator gen)
        {
            e1.Emit(gen);
            e2.Emit(gen);

            EmitOperation(gen);

            if (not) EmitNot(gen);

            // TI Basic uses -1/0, .NET uses 1/0, 
            // plus we need to convert from Int32 to double
            gen.Emit(OpCodes.Conv_R8);
            gen.Emit(OpCodes.Neg);

        }

        private void EmitNot(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ceq);
        }
        protected abstract void EmitOperation(ILGenerator gen);

        public static RelationalExpression CompareEquals(
            Expression e1, Expression e2, int line)
        {
            return new Equals(e1, e2, false, line);
        }

        public static RelationalExpression CompareNotEquals(
            Expression e1, Expression e2, int line)
        {
            return new Equals(e1, e2, true, line);
        }

        public static RelationalExpression CompareLessThan(
            Expression e1, Expression e2, int line)
        {
            return new LessThan(e1, e2, false, line);
        }

        public static RelationalExpression CompareLessThanEquals(
            Expression e1, Expression e2, int line)
        {
            return new GreaterThan(e1, e2, true, line);
        }

        public static RelationalExpression CompareGreaterThan(
            Expression e1, Expression e2, int line)
        {
            return new GreaterThan(e1, e2, false, line);
        }

        public static RelationalExpression CompareGreaterThanEquals(
            Expression e1, Expression e2, int line)
        {
            return new LessThan(e1, e2, true, line);
        }

    }

    
}
