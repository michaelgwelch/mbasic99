/*******************************************************************************
    Copyright 2006 Michael Welch
    
    This file is part of MBasic99.

    MBasic99 is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    MBasic99 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MBasic99; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*******************************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using mbasic.SyntaxTree;


namespace mbasic
{
    internal class Lexer
    {
        Reader reader;
        SymbolTable symbols;

        string value;
        bool startOfLine;
        int index;
        string label;
        bool readingData = false; // Unfortunately the grammar for BASIC is not a context free grammar, for example normally something like 'H' would be considered a variable, but after a DATA it is a string.

        public Lexer(Stream stream, SymbolTable symbols)
        {
            this.symbols = symbols;
            reader = new Reader(stream);
            startOfLine = true;
        }

        public int SymbolIndex { get { return index; } }

        public string Value { get { return value; } }

        public double NumericValue { get { return double.Parse(value); } }

        //public string Label { get { return label; } }

        public Token Next()
        {
            char ch;
            while (true)
            {
                if (reader.EndOfStream) return Token.EOF;

                ch = reader.Current;

                if (ch == '\n')
                {
                    reader.Advance();

                    // Only return Token.EndOfLine if this is a non empty line.
                    // EndOfLine is used to signify that we have finished parsing a line
                    // An empty line should just be skipped as white space.
                    if (startOfLine)
                    {
                        continue;
                    }
                    else
                    {
                        startOfLine = true;
                        readingData = false;
                        return Token.EndOfLine;
                    }
                }

                if (Char.IsWhiteSpace(ch))
                {
                    reader.Advance();
                    continue;
                }

                if (Char.IsDigit(ch) && startOfLine)
                {
                    startOfLine = false;
                    ReadLabel();
                    continue;
                }

                if (startOfLine) return Token.Error;

                if (readingData) // special case, only happen if we have read Token.Data
                {
                    if (Char.IsLetter(ch)) return NextString(false);
                    else if (Char.IsDigit(ch)) return NextNumber();
                    else if (ch == '\"') return NextString();
                    else if (ch == ',') { reader.Advance(); return Token.Comma; }
                    else throw new Exception("Unexpected char" + ch.ToString());
                }
                else
                {
                    if (Char.IsDigit(ch) || ch == '.') return NextNumber();

                    if (Char.IsLetter(ch))
                    {
                        Token word = NextWord(); // keywords and variables, and remarks
                        if (word == Token.Data) readingData = true;
                        return word;
                    }

                    if (ch == '\"') return NextString();


                    if (ch == '<')
                    {
                        ch = reader.Read();
                        if (ch == '>')
                        {
                            reader.Advance();
                            return Token.NotEquals;
                        }
                        else if (ch == '=')
                        {
                            reader.Advance();
                            return Token.LessThanEqual;
                        }
                        else return Token.LessThan;
                    }

                    if (ch == '>')
                    {
                        ch = reader.Read();
                        if (ch == '=')
                        {
                            reader.Advance();
                            return Token.GreaterThanEqual;
                        }
                        else return Token.GreaterThan;
                    }

                    switch (ch)
                    {
                        case ',':
                        case '*':
                        case '/':
                        case '+':
                        case '-':
                        case '=':
                        case '&':
                        case '^':
                        case '(':
                        case ')':
                        case ';':
                        case ':':
                            reader.Advance();
                            return (Token)ch;
                        default:
                            throw new Exception("Unexpected character: " + ch.ToString());
                    }
                }


            }
        }

        private void ReadLabel()
        {
            char ch;
            StringBuilder bldr = new StringBuilder();

            ch = reader.Current;
            while (char.IsDigit(ch))
            {
                bldr.Append(ch);
                ch = reader.Read();
            }
            label = bldr.ToString();

        }

        private Token NextNumber()
        {
            char ch;
            StringBuilder bldr = new StringBuilder();

            ch = reader.Current;
            // get chars up until white space, decimal point or E
            while (char.IsDigit(ch))
            {
                bldr.Append(ch);
                ch = reader.Read();
            }

            if (ch == '.')
            {
                bldr.Append(ch);
                ch = reader.Read();
                while (char.IsDigit(ch))
                {
                    bldr.Append(ch);
                    ch = reader.Read();
                }
            }

            if (ch == 'E' || ch == 'e')
            {
                bldr.Append(ch);
                ch = reader.Read();
                if (ch == '-' || ch == '+')
                {
                    bldr.Append(ch);
                    ch = reader.Read();
                }

                while (char.IsDigit(ch))
                {
                    bldr.Append(ch);
                    ch = reader.Read();
                }
            }

            value = bldr.ToString();
            return Token.Number;
        }

        // reads the next alpha numeric word and returns the token for it.
        // this can contain keywords, vars, string vars
        private Token NextWord()
        {
            char ch;
            StringBuilder bldr = new StringBuilder();

            ch = reader.Current;
            while (char.IsLetterOrDigit(ch))
            {
                bldr.Append(ch);
                ch = reader.Read();
            }

            if (ch == '$')
            {
                bldr.Append(ch);
                reader.Advance();
            }

            value = bldr.ToString().ToUpper();
            if (symbols.ContainsKeyWord(value))
            {
                Token word = symbols.GetKeyWordToken(value);
                if (word != Token.Remark) return word;
                while (!reader.EndOfStream)
                {
                    // read remark and toss it.
                    ch = reader.Read();
                    if (ch == '\n')
                    {
                        return Token.Remark;
                    }
                }
                if (reader.EndOfStream) return Token.Remark;
            }

            index = symbols.Lookup(value);
            if (index == -1)
            {
                index = symbols.Insert(value);
            }
            return Token.Variable;
        }

        private Token NextString(bool quoted)
        {
            Char endChar = quoted ? '\"' : ',';
            StringBuilder bldr = new StringBuilder();
            if (!quoted) bldr.Append(reader.Current); // If not quoted then append current char, otherwise skip it


            do
            {
                for (char ch = reader.Read(); ch != endChar; ch = reader.Read())
                {
                    bldr.Append(ch);
                }

                if (quoted) reader.Advance(); // to consume the quotation mark

                // if the next character is a quote then we haven't reached the end of
                // the string. We just read a double quote "" in the string which
                // should be replaced with a "

                if (reader.Current == '\"') bldr.Append('\"');
                
            } while (quoted && reader.Current == '\"');

            value = bldr.ToString();
            return Token.String;

        }

        private Token NextString()
        {
            return NextString(true);
        }

        public LineId LineId { get { return new LineId(reader.LineNumber, label); } }
        //public int LineNumber { get { return reader.LineNumber; } }
        public int Column { get { return reader.Column; } }

        private class Reader
        {
            int lineNumber=1;
            int column=1;

            StreamReader reader;
            public Reader(Stream stream)
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
                }
                else column++;
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
}
