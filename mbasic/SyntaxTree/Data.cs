using System;
using System.Collections.Generic;
using System.Text;

namespace mbasic.SyntaxTree
{
    /// <summary>
    /// Represents a DATA statement. 
    /// </summary>
    /// <remarks>
    /// While this is categorized as a Statement, it is never actually stored 
    /// in an abstract syntax tree. It is defined as a statement only for ease of 
    /// parsing. The data pulled out of a DATA statement contains numeric and string literals
    /// which are used to construct static data needed by the program.
    /// </remarks>
    class Data : Statement
    {
        private Data() : base(LineId.None) { }
        public static readonly Data Instance = new Data();
        public override void CheckTypes()
        {
        }

        public override void Emit(System.Reflection.Emit.ILGenerator gen, bool labelSetAlready)
        {
        }
    }
}
