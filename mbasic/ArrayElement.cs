using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using mbasic.SyntaxTree;
using System.Reflection;
using TiBasicRuntime;
namespace mbasic
{
    class ArrayElement : Location
    {
        static readonly Type builtInsType = typeof(BuiltIns);
        VariableLocation location;
        Expression[] exprs;
        MethodInfo getMethod; // The Get method for string or number array
        MethodInfo setMethod;
        public ArrayElement(VariableLocation loc, params Expression[] exprs)
        {
            this.location = loc;
            this.exprs = exprs;
        }

        public override void ConstrainType(SymbolTable symbols, bool isArray, int numDimensions)
        {
            location.ConstrainType(symbols, true, exprs.Length);
            foreach (Expression expr in exprs)
            {
                BasicType indexType = expr.GetBasicType();
                if (indexType != BasicType.Number && indexType != BasicType.Boolean) throw new Exception("type error");
            }


            switch (exprs.Length)
            {
                case 1:
                    getMethod = (location.BasicType == BasicType.StringArray) ?
                        builtInsType.GetMethod("GetStringValue1") :
                        builtInsType.GetMethod("GetNumberValue1");
                    setMethod = (location.BasicType == BasicType.StringArray) ?
                        builtInsType.GetMethod("SetStringValue1") :
                        builtInsType.GetMethod("SetNumberValue1");
                    break;
                case 2:
                    getMethod = (location.BasicType == BasicType.StringArray) ?
                        builtInsType.GetMethod("GetStringValue2") :
                        builtInsType.GetMethod("GetNumberValue2");
                    setMethod = (location.BasicType == BasicType.StringArray) ?
                        builtInsType.GetMethod("SetStringValue2") :
                        builtInsType.GetMethod("SetNumberValue2");
                    break;
                case 3:
                    getMethod = (location.BasicType == BasicType.StringArray) ?
                        builtInsType.GetMethod("GetStringValue3") :
                        builtInsType.GetMethod("GetNumberValue3");
                    setMethod = (location.BasicType == BasicType.StringArray) ?
                        builtInsType.GetMethod("SetStringValue3") :
                        builtInsType.GetMethod("SetNumberValue3");
                    break;
            }

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
            foreach (Expression expr in exprs)
            {
                expr.Emit(gen);
                gen.Emit(OpCodes.Conv_I4);
            }
            value.Emit(gen);
            if (value.GetBasicType() == BasicType.Boolean)
            {
                gen.Emit(OpCodes.Conv_R8);
                gen.Emit(OpCodes.Neg);
            }
            gen.Emit(OpCodes.Call, setMethod);
        }

        public override void EmitLoad(ILGenerator gen, List<LocalBuilder> locals)
        {
            this.location.EmitLoad(gen, locals);
            foreach (Expression expr in exprs)
            {
                expr.Emit(gen);
                gen.Emit(OpCodes.Conv_I4);
            }
            gen.Emit(OpCodes.Call, getMethod);
        }

    }
}
