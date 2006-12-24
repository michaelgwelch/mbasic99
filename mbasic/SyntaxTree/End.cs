using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;

namespace mbasic.SyntaxTree
{
    class End : Statement
    {
        public End(int line) : base(line) { }

        public override void CheckTypes()
        {
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);
            MarkSequencePoint(gen);
            gen.Emit(OpCodes.Leave, Node.endLabel);
        }
    }
}
