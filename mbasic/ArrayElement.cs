using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using mbasic.SyntaxTree;

namespace mbasic
{
    class ArrayElement : Location
    {
        VariableLocation location;
        Expression expr;
        public ArrayElement(VariableLocation loc, Expression expr)
        {
            this.location = loc;
            this.expr = expr;
        }

        public override void ConstrainType(SymbolTable symbols, bool isArray)
        {
            location.ConstrainType(symbols, true);
            BasicType indexType = expr.GetBasicType();
            if (indexType != BasicType.Number && indexType != BasicType.Boolean) throw new Exception("type error");
        }

        public override mbasic.SyntaxTree.BasicType BasicType
        {
            get 
            {
                switch (location.BasicType)
                {
                    case BasicType.NumberArray:
                        return BasicType.Number;
                    case BasicType.StringArray:
                        return BasicType.String;
                    default:
                        throw new Exception("What?");
                }
            }
        }



        public override void EmitStore(ILGenerator gen, List<LocalBuilder> locals, Expression value)
        {
            this.location.EmitLoad(gen, locals);
            expr.Emit(gen);
            gen.Emit(OpCodes.Conv_I4);
            value.Emit(gen);
            if (value.GetBasicType() == BasicType.Boolean) gen.Emit(OpCodes.Conv_R8);
            if (BasicType == BasicType.Number) gen.Emit(OpCodes.Stelem_R8);
            else gen.Emit(OpCodes.Stelem, typeof(string));

        }

        public override void EmitLoad(ILGenerator gen, List<LocalBuilder> locals)
        {
            this.location.EmitLoad(gen, locals);
            expr.Emit(gen);
            gen.Emit(OpCodes.Conv_I4);
            if (BasicType == BasicType.Number) gen.Emit(OpCodes.Ldelem_R8);
            else gen.Emit(OpCodes.Ldelem, typeof(string));
        }

    }
}
