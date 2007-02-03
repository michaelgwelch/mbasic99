/*******************************************************************************
    Copyright 2007 Michael Welch
    
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

namespace mbasic.SyntaxTree
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using TiBasicRuntime;

    class OnGoto : Statement
    {
        private List<string> targets;
        private Expression numericExpression;
        private BasicType expressionType;

        public OnGoto(Expression number, List<string> targets, LineId line)
            : base(line)
        {
            this.numericExpression = number;
            this.targets = targets;
        }

        public override void CheckTypes()
        {
            // Need to make sure that numericExpression is either
            // a number or a boolean
            expressionType = numericExpression.GetBasicType();

            // In this case we can't even allow Booleans which can only have
            // value of -1 or 0, neither of which is legal in an On-Goto
            if (expressionType != BasicType.Number)
            {
                throw new TypeCheckException("The expression used in a On-Goto statement must be numeric", line);
            }

            foreach (string target in targets)
            {
                if (!labels.ContainsKey(target))
                {
                    throw new TypeCheckException(
                        String.Format("Non existent line number {0} in On-Goto statement", target),
                        line);
                }
            }
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);
            MarkSequencePoint(gen);
            numericExpression.Emit(gen);
            gen.Emit(OpCodes.Call, typeof(BuiltIns).GetMethod("Round"));

            // We need to subtract one from the expression. OnGoto starts
            // counting at 1 and switch starts counting at 0.
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Sub);

            List<Label> destinations = new List<Label>();
            foreach (string target in targets)
            {
               destinations.Add(labels[target]);
            }
            gen.Emit(OpCodes.Switch, destinations.ToArray());
            
            // if the integer was larger than number of labels, then
            // switch falls thru to next statement. An On-Goto should
            // output BAD VALUE IN xx  error.
            gen.Emit(OpCodes.Ldstr, line.Label);
            BuiltInsMethodCall.BadValueError().Emit(gen);

        }
    }
}
