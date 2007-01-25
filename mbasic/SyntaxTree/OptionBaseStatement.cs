using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;

namespace mbasic.SyntaxTree
{
    class OptionBaseStatement : Statement
    {
        int optionBase;
        public OptionBaseStatement(int optionBase)
            : base(LineId.None)
        {
            this.optionBase = optionBase;
        }

        public override void CheckTypes()
        {
            // Constraint/set the option base (can only be done once)
            OptionBase = optionBase;
        }

        public override void Emit(ILGenerator gen, bool labelSetAlready)
        {
        }
    }
}
