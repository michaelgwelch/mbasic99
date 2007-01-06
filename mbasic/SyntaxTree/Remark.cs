using System;
using System.Collections.Generic;
using System.Text;

namespace mbasic.SyntaxTree
{
    /// <summary>
    /// Represents a remark. A remark has no code associated with
    /// it, but unlike comments in other languages, a remark
    /// has one significant detail. It has a line number and
    /// can therefore be the destination of a GOSUB or GOTO.
    /// </summary>
    class Remark : Statement
    {
        public Remark(LineId line)
            : base(line)
        {
        }

        public override void CheckTypes()
        {
        }

        public override void Emit(System.Reflection.Emit.ILGenerator gen, bool labelSetAlready)
        {
            if (!labelSetAlready) MarkLabel(gen);
            // no code to emit.
        }
    }
}
