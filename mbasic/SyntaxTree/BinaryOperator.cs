using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;

namespace mbasic.SyntaxTree
{
    abstract class BinaryOperator : Expression
    {
        Expression expr1;
        Expression expr2;
        BasicType expr1Type;
        BasicType expr2Type;
        BasicType ourType;

        protected BinaryOperator(Expression e1, Expression e2, LineId line)
            : base(line)
        {
            this.expr1 = e1;
            this.expr2 = e2;
        }

        public override void Emit(ILGenerator gen)
        {
            expr1.Emit(gen);
            if (expr1Type == BasicType.Boolean) EmitConvertToDouble(gen);
            expr2.Emit(gen);
            if (expr2Type == BasicType.Boolean) EmitConvertToDouble(gen);
            EmitOperation(gen);
        }



        public override BasicType GetBasicType()
        {
            expr1Type = expr1.GetBasicType();
            expr2Type = expr2.GetBasicType();
            if ((expr1Type == BasicType.Boolean || expr1Type == BasicType.Number)
                && (expr2Type == BasicType.Boolean || expr2Type == BasicType.Number))
            {
                ourType = BasicType.Number;
            }
            else
            {
                ourType = BasicType.Error;
            }
            return ourType;
        }
        protected abstract void EmitOperation(ILGenerator gen);

    }
}
