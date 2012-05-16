using System;
using System.IO;

namespace mbasic
{
    internal class StreamEnumerator
    {
        int lineNumber = 1;
        int column = 1;
        StreamReader reader;

        public StreamEnumerator(Stream stream)
        {
            reader = new StreamReader(stream);
        }

        /// <summary>
        /// Gets the character at the current position but does not change the current position.
        /// </summary>
        /// <returns></returns>
        public char Current
        {
            get
            {
                return (char)reader.Peek();
            }
        }

        /// <summary>
        /// Moves the reader to the next character.
        /// </summary>
        public void Advance()
        {
            int val = reader.Read();
            if (val == '\n')
            {
                column = 1;
                lineNumber++;
            } else
                column++;
        }

        /// <summary>
        /// Moves the reader to the next character and returns it.
        /// Identical to Advance() followed by reading Current;
        /// </summary>
        /// <returns></returns>
        public char Read()
        {
            Advance();
            return Current;
        }

        public bool EndOfStream { get { return reader.EndOfStream; } }

        public int LineNumber { get { return lineNumber; } }

        public int Column { get { return column; } }
    }

}
