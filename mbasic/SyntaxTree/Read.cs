using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using TIBasicRuntime;

namespace mbasic.SyntaxTree
{
    class Read : Statement
    {
        Block assignments;

        public Read(Block assignments, LineId line)
            : base(line)
        {
            this.assignments = assignments;
        }

        public override void CheckTypes()
        {
            assignments.CheckTypes();
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);
            MarkSequencePoint(gen);
            assignments.Emit(gen, true);
        }
    }
}
