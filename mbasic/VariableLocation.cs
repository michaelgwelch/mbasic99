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

        public override void ConstrainType(SymbolTable symbols, bool isArray, int numDimensions)
        {
            symbols[symbolIndex].ConstrainType(isArray, numDimensions);
            basicType = symbols[symbolIndex].BasicType;
        }

        public override void EmitStore(ILGenerator gen, IList<FieldBuilder> fields, Expression value)
        {
            gen.Emit(OpCodes.Ldarg_0);
            // Emit the code to generate the value for the given expression
            value.Emit(gen);

            // Convert booleans (which are used internally for Booleans) to doubles
            // which is how they are stored in TIBasic. Plus it needs to be negated.
            if (value.GetBasicType() == BasicType.Boolean)
            {
                gen.Emit(OpCodes.Conv_R8);
                gen.Emit(OpCodes.Neg);
            }

            gen.Emit(OpCodes.Stfld, fields[symbolIndex]);
        }

        public override void EmitLoad(ILGenerator gen, IList<FieldBuilder> fields)
        {
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, fields[symbolIndex]);
        }

    }
}
