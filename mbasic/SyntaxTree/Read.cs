using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using TiBasicRuntime;

namespace mbasic.SyntaxTree
{
    class Read : Statement
    {
        int[] indexes;
        BasicType[] varType;

        private static readonly MethodInfo readNumberMethod =
            typeof(BuiltIns).GetMethod("ReadDouble");

        private static readonly MethodInfo readStringMethod =
            typeof(BuiltIns).GetMethod("ReadString");

        public Read(int[] indexes, LineId line)
            : base(line)
        {
            this.indexes = indexes;
            varType = new BasicType[indexes.Length];
        }

        public override void CheckTypes()
        {
            for (int i = 0; i < indexes.Length; i++)
            {
                Type t = locals[indexes[i]].LocalType;

                if (t == typeof(string)) varType[i] = BasicType.String;
                else varType[i] = BasicType.Number;
            }
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);
            MarkSequencePoint(gen);
            for (int i = 0; i < indexes.Length; i++)
            {
                gen.Emit(OpCodes.Ldloca, locals[indexes[i]]);

                if (varType[i] == BasicType.Number)
                {
                    gen.Emit(OpCodes.Call, readNumberMethod);
                }
                else if (varType[i] == BasicType.String)
                {
                    gen.Emit(OpCodes.Call, readStringMethod);
                }
            }
                
        }
    }
}
