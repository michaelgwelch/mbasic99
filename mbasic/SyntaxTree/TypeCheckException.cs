using System;
using System.Collections.Generic;
using System.Text;

namespace mbasic.SyntaxTree
{
    class TypeCheckException : CompilerException
    {
        public TypeCheckException()
        {
        }

        public TypeCheckException(string message, LineId line)
            : base(message, line)
        {
        }

        public TypeCheckException(string message, LineId line, Exception innerException)
            : base(message, line, innerException)
        {
        }

    }
}
