using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using mbasic.SyntaxTree;

namespace mbasic
{
    class VariableLocation : Location
    {
        int symbolIndex;
        BasicType basicType;

        public VariableLocation(int index)
        {
            this.symbolIndex = index;
        }

        public override mbasic.SyntaxTree.BasicType BasicType
        {
            get { return basicType; }
        }

        public override void ConstrainType(SymbolTable symbols, bool isArray)
        {
            symbols[symbolIndex].ConstrainType(isArray);
            basicType = symbols[symbolIndex].BasicType;
        }

        public override void EmitStore(ILGenerator gen, List<LocalBuilder> locals, Expression value)
        {
            value.Emit(gen);
            if (basicType == BasicType.Boolean) gen.Emit(OpCodes.Conv_R8);
            gen.Emit(OpCodes.Stloc, locals[symbolIndex]);
        }

        public override void EmitLoad(ILGenerator gen, List<LocalBuilder> locals)
        {
            gen.Emit(OpCodes.Ldloc, locals[symbolIndex]);
        }

    }
}
