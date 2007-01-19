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
        Assign[] assignments;

        public Read(Assign[] assignments, LineId line)
            : base(line)
        {
            this.assignments = assignments;
        }

        public override void CheckTypes()
        {
            foreach (Assign assign in assignments) assign.CheckTypes();
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);
            MarkSequencePoint(gen);
            for (int i = 0; i < assignments.Length; i++)
            {
                assignments[i].Emit(gen, true);
            }
        }
    }
}
