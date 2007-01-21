using System;
using System.Collections.Generic;
using System.Text;
using mbasic.SyntaxTree;
using System.Reflection.Emit;

namespace mbasic
{
    abstract class Location
    {
        public abstract BasicType BasicType { get; }
        public abstract void ConstrainType(SymbolTable symbols, bool isArray, int numDimensions);
        public virtual void ConstrainType(SymbolTable symbols) { ConstrainType(symbols, false, -1); }
        public abstract void EmitStore(ILGenerator gen, List<LocalBuilder> locals, Expression value);
        public abstract void EmitLoad(ILGenerator gen, List<LocalBuilder> locals);
    }
}
