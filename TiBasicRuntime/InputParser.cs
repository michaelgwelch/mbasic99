using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TiBasicRuntime
{
    /// <summary>
    /// Reads a line of input and breaks it into comma seperated values.
    /// </summary>
    class InputParser
    {
        Reader reader;
        public InputParser(string s)
        {
            this.reader = new Reader(s.Trim() + " ");
        }

        public bool EndOfString { get { return reader.EndOfStream; } }

        public string Next()
        {
            while (true)
            {
                char ch = reader.Current;
                if (char.IsWhiteSpace(ch))
                {
                    reader.Advance();
                    continue;
                }

                string val = NextString(ch == '\"');

                while (!reader.EndOfStream)
                {
                    ch = reader.Current;
                    if (char.IsWhiteSpace(ch) || ch == ',') reader.Advance();
                    else break;
                }
                return val;
            }
        }

        private string NextString(bool quoted)
        {
            Char endChar = quoted ? '\"' : ',';
            StringBuilder bldr = new StringBuilder();
            if (!quoted) bldr.Append(reader.Current); // If not quoted then append current char, otherwise skip it

            do
            {
                for (char ch = reader.Read(); ch != endChar && !reader.EndOfStream; ch = reader.Read())
                {
                    bldr.Append(ch);
                }

                if (quoted) reader.Advance(); // to consume the quotation mark

                // if the next character is a quote then we haven't reached the end of
                // the string. We just read a double quote "" in the string which
                // should be replaced with a "

                if (reader.Current == '\"') bldr.Append('\"');

            } while (quoted && reader.Current == '\"');

            string value = bldr.ToString();
            return value;

        }

        private class Reader
        {
            int index = 0;
            string s;
            int length;

            public Reader(String s)
            {
                this.s = s;
                this.length = s.Length;
            }

            /// <summary>
            /// Gets the character at the current position but does not change the current position.
            /// </summary>
            /// <returns></returns>
            public char Current
            {
                get
                {
                    return s[index];
                }
            }

            /// <summary>
            /// Moves the reader to the next character.
            /// </summary>
            public void Advance()
            {
                index++;
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

            public bool EndOfStream { get { return index == length - 1; } }

        }

    }
}
