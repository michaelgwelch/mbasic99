using System;
using System.Collections.Generic;
using System.Text;

namespace mbasic.SyntaxTree
{
    class CompilerException : ApplicationException
    {
        LineId line;
        public CompilerException()
        {
        }

        public CompilerException(string message, LineId line)
            : base(message)
        {
            this.line = line;
        }

        public CompilerException(string message, LineId line, Exception innerException)
            : base(message, innerException)
        {
            this.line = line;
        }

        public LineId LineId { get { return line; } }


    }
}
