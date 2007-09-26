using System;
using System.Collections.Generic;
using System.Text;

namespace TIBasicRuntime
{
    // In this file I use 'string' to refer to the C# built-in string.
    // Everywhere I use String, it refers to TIBasicRuntime.String.

    public struct TIString
    {
        static readonly Encoding TIEncoding = Encoding.ASCII;
        public static readonly TIString Empty = new TIString(String.Empty);
        public static readonly TIString Space = new TIString(" ");

        readonly string _val; // should only be accessed by private Value property

        private string Value
        {
            get
            {
                return _val == null ? string.Empty : _val;
            }
        }

        public TIString(string val)
        {
            this._val = val;
        }

        public TIString(byte[] bytes)
        {
            this._val = TIEncoding.GetString(bytes);
        }

        public static implicit operator TIString(string str)
        {
            return new TIString(str);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        #region Relational Operators

        public static Number Compare(TIString str1, TIString str2)
        {
            return new Number(string.Compare(str1.Value, str2.Value));
        }

        public static Number operator >(TIString str1, TIString str2)
        {
            return Compare(str1, str2) > Number.Zero;
        }

        public static Number operator <(TIString str1, TIString str2)
        {
            return Compare(str1, str2) < Number.Zero;
        }

        public static Number operator ==(TIString str1, TIString str2)
        {
            return Compare(str1, str2) == Number.Zero;
        }

        public static Number operator !=(TIString str1, TIString str2)
        {
            return Compare(str1, str2) != Number.Zero;
        }

        public static TIString operator +(TIString str1, TIString str2)
        {
            return Concatenate(str1, str2);
        }

        public Number Equals(TIString str)
        {
            return Compare(this, str) == Number.Zero;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is TIString)) return false;

            if (Equals((Number)obj)) return true;
            else return false;

        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        #endregion

        #region TI Basic Builtin In String functions

        public static TIString Concatenate(TIString str1, TIString str2)
        {
            return new TIString(string.Concat(str1.Value, str2.Value));
        }

        public static Number AsciiValue(TIString str)
        {
            if (str.Length == Number.Zero) throw new ArgumentException("value must have length greater than 0.", "str");
            return new Number(str.Value[0]);
        }

        public Number Length
        {
            get
            {
                return new Number(Value.Length);
            }
        }

        public static Number Position(TIString s1, TIString s2, Number startPos)
        {
            Number startIndex = Number.Integer(startPos) - Number.One;
            if (startIndex < Number.Zero) throw new ArgumentOutOfRangeException("startPos", "Must be greater than or equal to 1");
            return new Number(s1.Value.IndexOf(s2.Value, (int)startIndex) + 1);
        }

        public static TIString Segment(TIString s, Number startPos, Number length)
        {
            Number start = Number.Integer(startPos) - Number.One;

            // len + start must be inside s. So we may have to reduce len
            // to avoid exceptions. This will give the expected behavior.
            Number len = Number.Minimum(Number.Integer(length), s.Length - start);

            if (start < Number.Zero || len < Number.Zero) throw new ArgumentOutOfRangeException("either startPos or length are incorrect");
            return new TIString(s.Value.Substring((int)start, (int)len));
        }


        #endregion

        public static byte[] GetBits(TIString str)
        {
            return TIEncoding.GetBytes(str.Value);
        }
    }
}
