using System;
using System.Collections.Generic;
using System.Text;

namespace mbasic.SyntaxTree
{
    class ArrayDeclaration : Statement
    {
        int index;
        int[] dimensions;
        public ArrayDeclaration(int index, int[] dimensions)
            : base(LineId.None)
        {
            this.index = index;
            this.dimensions = dimensions;
        }

        public override void CheckTypes()
        {
            // Make sure option base is set
            // this will cause an exception at compile time
            // to be thrown if an OptionBase occurrs
            // after a DIM (which is what we want)
            ConstrainOptionBase();

            // Dimension the variable
            symbols[index].Dimension(dimensions);
        }

        public override void Emit(System.Reflection.Emit.ILGenerator gen, bool labelSetAlready)
        {
        }
    }
}
