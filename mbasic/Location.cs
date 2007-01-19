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
        public abstract void ConstrainType(SymbolTable symbols, bool isArray);
        public virtual void ConstrainType(SymbolTable symbols) { ConstrainType(symbols, false); }
        public abstract void EmitStore(ILGenerator gen, List<LocalBuilder> locals, Expression value);
        public abstract void EmitLoad(ILGenerator gen, List<LocalBuilder> locals);
    }
}
