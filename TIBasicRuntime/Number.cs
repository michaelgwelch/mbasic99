using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TIBasicRuntime
{
  
    public struct Number : IComparable, IComparable<Number>, IEquatable<Number>
    {
        // Radix 100 is laid out like this
        //      7   6   5   4   3   2   1
        // EXP MSD                     LSD
        //  XX  XX  XX  XX  XX  XX  XX  XX

        // EXP - exponent
        // MSD, LSD: Most significant digit, Least Significant digit.
        // Each digit is base 100

        // Each digit therefore can take value 0 - 99 (each is stored in a byte).

        // The number is normalized so that the decimal point is immediately
        // to the right of the MSD.

        // The exponent can range from -64 to 63 and is biased by 0x40. (So 
        // an exponent of 0 is 0x40. An exponent of 1 is 0x41. An exponent of -64 is 0x00.
        // An exponent of 63 is 0x7F. 

        // If the number is negative then the exponent and MSD are complemented.
        // So the number negative one (1E0) would have an exponent of
        // 0xBF (because 0xBF is the complement of 0x40 which is an exponent of 0)
        // and the MSD would be 0xFE (which is the complement of 0x01).

        public static readonly Number MaxValue = new Number(0x7F63636363636363, true); // 9.9999999999999E127 or 99.999999999999E126
        public static readonly Number MinValue = new Number(0x809C636363636363, true); //-9.9999999999999E127
        public static readonly Number Epsilon = new Number(0x0001000000000000, true);  // 1E-128
        public static readonly Number Zero = new Number(0x0000000000000000, true); // there is actually a zero for every exponent.

        public static readonly Number MaxInteger = new Number(0x4663636363636363, true); //  99,999,999,999,999
        public static readonly Number MinInteger = new Number(0xC99C636363636363, true); // -99,999,999,999,999

        public static readonly Number MaxUInt32 = uint.MaxValue;

        public static readonly Number MaxInt32 = (uint)int.MaxValue;
        public static readonly Number MinInt32 = -MaxInt32;

        public static readonly Number One = new Number(0x4001000000000000, true);
        public static readonly Number MinusOne = new Number(0xBFFE000000000000, true);

        private static readonly byte exponentBias = 0x40;

        static readonly ulong[] digitMasks = new ulong[] {
            0x00000000000000FF,
            0x000000000000FF00,
            0x0000000000FF0000,
            0x00000000FF000000,
            0x000000FF00000000,
            0x0000FF0000000000,
            0x00FF000000000000,
            0xFF00000000000000};


        readonly ulong value;

        #region Constructors

        /// <summary>
        /// Initializes 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="isRadix100BitPattern">When this is false,
        /// this constructor converts a ulong to a Number, when
        /// it is true the val is treated as a legal radix 100 bit pattern</param>
        private Number(ulong val, bool isRadix100BitPattern)
        {
            if (isRadix100BitPattern) this.value = val;
            else throw new NotImplementedException();
        }

        /// <summary>
        /// Converts nval to Radix100.
        /// </summary>
        /// <param name="nval"></param>
        private Number(ulong nval)
        {
            this.value = UInt64BitPatternToRadix100BitPattern(nval);
        }

        /// <summary>
        /// Converts a uint to a Number
        /// </summary>
        /// <param name="nval"></param>
        private Number(uint nval)
        {
            this.value = UInt32BitPatternToRadix100BitPattern(nval);
        }

        #endregion Constructors

        #region Conversions Operators
        [CLSCompliant(false)]
        public static explicit operator Number(ulong nval)
        {
            return new Number(nval);

        }

        public static explicit operator Number(long val)
        {
            if (val >= 0) return new Number((ulong)val);
            return -(new Number((ulong)(-val)));

        }

        private static ulong UInt64BitPatternToRadix100BitPattern(ulong nval)
        {
            // the maximum ulong would take a Radix100 with 10 digits plus an exponent.
            ulong[] d = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // room for 10 digits. Only 7 can be kept.
            ulong n7, n6, n5, n4, n3, n2, n1, n0;
            ulong q;

            n0 = nval & 0xFF;
            n1 = (nval >> 8) & 0xFF;
            n2 = (nval >> 16) & 0xFF;
            n3 = (nval >> 24) & 0xFF;
            n4 = (nval >> 32) & 0xFF;
            n5 = (nval >> 40) & 0xFF;
            n6 = (nval >> 48) & 0xFF;
            n7 = (nval >> 56) & 0xFF;

            d[0] = 36 * n7 + 56 * n6 + 76 * n5 + 96 * n4 + 16 * n3 + 36 * n2 + 56 * n1 + n0;
            q = d[0] / 100;
            d[0] = d[0] % 100;

            d[1] = q + 79 * n7 + 6 * n6 + 77 * n5 + 72 * n4 + 72 * n3 + 55 * n2 + 2 * n1;
            q = d[1] / 100;
            d[1] = d[1] % 100;

            d[2] = q + 92 * n7 + 71 * n6 + 62 * n5 + 96 * n4 + 77 * n3 + 6 * n2;
            q = d[2] / 100;
            d[2] = d[2] % 100;

            d[3] = q + 37 * n7 + 76 * n6 + 11 * n5 + 94 * n4 + 16 * n3;
            q = d[3] / 100;
            d[3] = d[3] % 100;

            d[4] = q + 40 * n7 + 49 * n6 + 95 * n5 + 42 * n4;
            q = d[4] / 100;
            d[4] = d[4] % 100;

            d[5] = q + 59 * n7 + 47 * n6 + 9 * n5;
            q = d[5] / 100;
            d[5] = d[5] % 100;

            d[6] = q + 57 * n7 + 81 * n6 + n5;
            q = d[6] / 100;
            d[6] = d[6] % 100;

            d[7] = q + 20 * n7 + 2 * n6;
            q = d[7] / 100;
            d[7] = d[7] % 100;

            d[8] = q + 7 * n7;
            q = d[8] / 100;
            d[8] = d[8] % 100;

            d[9] = q;


            int i = 9;
            while (d[i] == 0 && i >= 0) --i;
            ulong exp = (ulong)(0x40 + i);
            ulong value = 0;
            int shift = 48;
            int pos;
            for (pos = i; pos >= 0 && shift >= 0; --pos)
            {
                value += ((ulong)d[pos]) << shift;
                shift -= 8;
            }

            // This can happen if the UInt64 value > Number.MaxInteger
            if (pos >= 0)
            {
                // round properly
                if (d[pos] >= 50)
                {
                    // Okay we need to add 1 to LSD, which may cause us to have to carry all the way up to MSD.
                    value += 1;
                    int bytePos = 0;
                    byte byteVal = GetByte(value, bytePos);
                    while (byteVal == 100 && bytePos < 6)
                    {
                        // decrement current byteVal by 100
                        SetByte(ref value, bytePos, (byte)0); 

                        // increment the next highest byte by 1.
                        SetByte(ref value, bytePos + 1, (byte)(GetByte(value, bytePos + 1) + 1));

                        ++bytePos;
                        byteVal = GetByte(value, bytePos);
                    }
                    // Okay if the MSD is greater than 99 then put a 1 in the MSD increment exponent by 1.
                    if (byteVal > 99)
                    {
                        SetByte(ref value, 6, 1);
                        //normally this isn't necessarily safe because exponent could "roll-over"
                        //but since we started with a ulong we know this won't be possible.
                        exp++;
                    }
                }

            }
            value += (exp << 56);
            return value;
        }

        private static ulong UInt32BitPatternToRadix100BitPattern(uint nval)
        {
            //byte exp = (byte)(0 + exponentBias);
            uint[] d = { 0, 0, 0, 0, 0 };
            int shift;
            uint q; // carry bit, quotient
            //uint d3, d2, d1, d0, d4;
            d[0] = nval & 0xFF;
            d[1] = (nval >> 8) & 0xFF;
            d[2] = (nval >> 16) & 0xFF;
            d[3] = (nval >> 24) & 0xFF;

            d[0] = (16 * d[3]) + (36 * d[2]) + (56 * d[1]) + (d[0]);
            q = d[0] / 100;
            d[0] = d[0] % 100;

            d[1] = q + (72 * d[3]) + (55 * d[2]) + (2 * d[1]);
            q = d[1] / 100;
            d[1] = d[1] % 100;

            d[2] = q + (77 * d[3]) + (6 * d[2]);
            q = d[2] / 100;
            d[2] = d[2] % 100;

            d[3] = q + (16 * d[3]);
            q = d[3] / 100;
            d[3] = d[3] % 100;

            d[4] = q;

            // Need to place the first non zero d value in MSD spot.

            int i = 4;
            while (d[i] == 0 && i >= 0) --i;
            ulong exp = (ulong)(0x40 + i);
            ulong value = 0;
            shift = 48;
            for (int pos = i; pos >= 0; --pos)
            {
                value += ((ulong)d[pos]) << shift;
                shift -= 8;
            }

            // Add the exponent.
            value += (exp << 56);
            return value;
        }

        public static implicit operator Number(int val)
        {
            if (val >= 0) return new Number((uint)val);
            return -(new Number((uint)(-val)));

        }

        public static explicit operator int(Number number)
        {
            bool neg = number.IsNegative;
            if (neg) number = -number;
            uint val = (uint)number;
            if (neg) return (int)-val;
            else return (int)val;
        }

        [CLSCompliant(false)]
        public static implicit operator Number(uint nval)
        {
            return new Number(nval); 
        }

        [CLSCompliant(false)]
        public static explicit operator uint(Number num)
        {
            return ToUInt32(num);
        }

        [CLSCompliant(false)]
        public static uint ToUInt32(Number num)
        {
            if (num > Number.MaxUInt32) throw new InvalidCastException("num is too large");
            if (num < Number.Zero) throw new NotImplementedException("negative numbers not supported for this yet.");
            if (!num.IsInteger) throw new NotImplementedException("floating point not supported yet.");
            if (num.IsZero) return 0;

            sbyte exponent = num.Exponent;
            long mantissa = (long)num.Mantissa;

            mantissa = mantissa >> (8 * (6 - exponent));

            // can assume since UInt32.MaxValue is max value that d6, and d5 are empty.
            int[] d = { 0, 0, 0, 0, 0 };
            int q;

            d[0] = (int) mantissa & 0xFF;
            d[1] = (int) ((mantissa >> 8) & 0xFF);
            d[2] = (int) ((mantissa >> 16) & 0xFF);
            d[3] = (int) ((mantissa >> 24) & 0xFF);
            d[4] = (int) ((mantissa >> 32) & 0xFF);

            d[0] = 64 * d[3] + 16 * d[2] + 100 * d[1] + d[0];
            q = d[0] >> 8; // divide by 256
            d[0] = (d[0] - (q * 256)); // this gives us d[0] % 256 without expensive mod calculation 

            d[1] = q+ 225 * d[4] + 66 * d[3] + 39 * d[2];
            q = d[1] >> 8;
            d[1] = d[1] - (q * 256);

            d[2] = q + 245 * d[4] + 15 * d[3];
            q = d[2] >> 8;
            d[2] = d[2] - (q*256);

            d[3] = q + 5 * d[4];
            // since we guaranteed above that the value fits in uint we know that d[3] can't have a carry bit.

            return (uint) (d[0] + (d[1] << 8) + (d[2] << 16) + (d[3] << 24));
        }


        #endregion

        #region Mathematical Operators
        public static int Sign(Number num1)
        {
            if (num1.IsNegative) return -1;
            if (num1.IsZero) return 0;
            return 1;
        }

        public static Number Add(Number num1, Number num2)
        {
            if (num1.IsNegative || num2.IsNegative) throw new NotImplementedException();
            return Number.Zero;
        }

        public static Number Negate(Number number)
        {
            return -number; 
        }

        public static Number operator +(Number number)
        {
            return number;
        }

        public static Number operator -(Number number)
        {
            // complement the 2 high order bytes using exclusive or operator;
            ulong newRawValue = number.value ^ (0xFFFF000000000000);
            return new Number(newRawValue, true);
        }
        #endregion

        #region ToString routines

        public override string ToString()
        {
            if (IsZero) return "0";
            if (IsInteger && !IsNegative) return ToIntegerForm();

            byte h0, h1, h2, h3, h4, h5, h6;

            h0 = (byte)(value & 0x0FF);
            h1 = (byte)((value >> 8) & 0xFF);
            h2 = (byte)((value >> 16) & 0xFF);
            h3 = (byte)((value >> 24) & 0xFF);
            h4 = (byte)((value >> 32) & 0xFF);
            h5 = (byte)((value >> 40) & 0xFF);
            h6 = (byte)((value >> 48) & 0xFF);

            StringBuilder bldr = new StringBuilder(20);
            bldr.AppendFormat("{0:00}", h6);
            bldr.AppendFormat("{0:00}", h5);
            bldr.AppendFormat("{0:00}", h4);
            bldr.AppendFormat("{0:00}", h3);
            bldr.AppendFormat("{0:00}", h2);
            bldr.AppendFormat("{0:00}", h1);
            bldr.AppendFormat("{0:00}", h0);

            return bldr.ToString();

        }

        public string ToIntegerForm()
        {
            StringBuilder bldr = new StringBuilder();
            if (IsNegative) bldr.Append("-");


            sbyte exponent = Exponent;

            ulong rawBytes = Mantissa;
            ulong integerPart;
            ulong fractionalPart;
            if (exponent < 0)
            {
                integerPart = 0;
                fractionalPart = rawBytes << 8;
            }
            else
            {
                integerPart = (rawBytes & (~isIntMasks[exponent])) << 8;
                fractionalPart = (rawBytes & (isIntMasks[exponent])) << ((exponent + 2) * 8);
            }

            // format integer part
            string stringFormat = "##";
            if (integerPart != 0)
            {
                for (int i = 7; i >= (7 - exponent); i--)
                {
                    byte theByte = GetByte(integerPart, i); ;
                    bldr.Append(theByte.ToString(stringFormat));
                    if (theByte > 0) stringFormat = "00"; // print all the digits of the rest of the values
                }
            }

            return bldr.ToString();

        }

        #endregion

        #region Properties

        /// <summary>
        /// Used to help determine if a Number is an integer. Only
        /// Numbers with exponents with values 0 - 6 can qualify to be a integer.
        /// A number with an exponent of 0 can only have MSD be non zero.
        /// A number with an exponent of 6 can have all digits be non zero.
        /// This is what these masks help calculate. See IsInteger.
        /// </summary>
        static readonly ulong[] isIntMasks = new ulong[] {
            0x0000FFFFFFFFFFFF,
            0x000000FFFFFFFFFF,
            0x00000000FFFFFFFF,
            0x0000000000FFFFFF,
            0x000000000000FFFF,
            0x00000000000000FF,
            0x0000000000000000 };

        /// <summary>
        /// Returns true if this Number is an integer, otherwise it retuns false.
        /// </summary>
        public bool IsInteger
        {
            get
            {
                // special case for 0, since its exponent is -64
                if (IsZero) return true;

                // Get Exponent Byte
                sbyte expValue = Exponent;

                if (expValue < 0 || expValue > 6) return false;

                // else use masks to see if this is an int --

                // if expValue = 0 then only digit 7 can have a value and this still be an int
                // if expValue = 1 then only digits 7 and 6 can have a value and this still be an int
                // if expValue = 2 then 7,6, and 5 can have values
                // if expValue < 0 then this is not an int.


                UInt64 mask = isIntMasks[expValue];
                return (mask & value) == 0;
            }
        }

        public bool IsZero
        {
            get
            {
                return ((0x00FFFFFFFFFFFFFF & value) == 0);
            }
        }
        public bool IsNegative
        {
            get
            {
                if (IsZero) return false;
                return (0x8000000000000000 & value) != 0;
            }
        }

        /// <summary>
        /// Returns the base 100 exponent of this number. The exponent for Zero is always -64.
        /// </summary>
        /// <returns></returns>
        public short GetExponent()
        {
            return Exponent;
        }

        /// <summary>
        /// The base 100 exponent of this number.
        /// </summary>
        [CLSCompliant(false)]
        public sbyte Exponent
        {
            get
            {
                // Define exponet of Zero to be -64, keeps ordering correct.
                if (IsZero) return -64;

                // the math is slightly non-obvious. We need to remember that
                // negative exponents are one's complement, not two's complement.

                // The quick fix. If the bit pattern is positive, just subtract bias.
                // If the bit pattern is negative, just add the bias.

                // If bit pattern is 0x00 that is a special case. Return -64

                // Get the raw bit pattern and treat it as a signed byte.
                sbyte exponentBitPattern = (sbyte)ExponentBitPattern;
                sbyte result;

                if (exponentBitPattern == 0) result = -64;
                else if (exponentBitPattern > 0)
                {
                    // subtract the bias
                    result = (sbyte)(exponentBitPattern - exponentBias);
                }
                else
                {
                    // add the bias
                    result = (sbyte)(exponentBitPattern + exponentBias);
                }

                return result;
            }
        }

        /// <summary>
        /// Retrieves the mantissa bit pattern for this instance.
        /// </summary>
        private ulong Mantissa
        {
            get
            {
                return (value & 0x00FFFFFFFFFFFFFF);
            }
        }

        /// <summary>
        /// Returns the exponent byte (the most significant byte) of this value.
        /// </summary>
        private byte ExponentBitPattern
        {
            get
            {
                return GetByte(value, 7);
            }
        }
        #endregion

        #region Equality 

        public override bool Equals(object obj)
        {
            if (!(obj is Number)) return false;
            return Equals((Number)obj);
        }


        public override int GetHashCode()
        {
            // There are many bit patterns that evaluate to Zero
            // and they must all give same hash code.
            if (IsZero) return 0;
            else return (int)(value % int.MaxValue); // just return low order bits.
        }

        public bool Equals(Number number)
        {
            // There are many bit patterns that evaluate to zero, 
            // so we can't just compare the values.
            if (this.IsZero) return number.IsZero;
            else return (this.value == number.value);

        }

        public static bool operator ==(Number n1, Number n2)
        {
            return n1.Equals(n2);
        }

        public static bool operator !=(Number n1, Number n2)
        {
            return !n1.Equals(n2);
        }
        #endregion

        #region Ordering
        public static bool operator <(Number n1, Number n2)
        {
            if (n1.IsNegative || n2.IsNegative) throw new NotImplementedException("negative numbers not yet implemented");

            // There are 128 different zero values. We want to treat them all like Number.Zero.
            Number num1 = n1.IsZero ? Number.Zero : n1;
            Number num2 = n2.IsZero ? Number.Zero : n2;

            // algorithm
            // 1. check the exponents. They provide a natural ordering. If they differ
            // they tell us all we need to know.
            sbyte exp1 = num1.Exponent;
            sbyte exp2 = num2.Exponent;
            if (exp1 != exp2) return exp1 < exp2;

            // 2. If the exponents are the same then we must compare the mantissas.
            // The following works for all positive numbers
            return num1.Mantissa < num2.Mantissa;
        }

        public static bool operator >(Number n1, Number n2)
        {
            if (n1.Equals(n2)) return false;
            return (!(n1 < n2));
        }

        /// <summary>
        /// Compares this instance to obj.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>-1 if this instance is less than obj, 0 if this instance is equal to obj,
        /// 1 if this instance is greater than obj.</returns>
        public int CompareTo(object obj)
        {
            if (obj is Number) return CompareTo((Number)obj);

            throw new ArgumentException("obj is not a Number");
        }

        /// <summary>
        /// Compares this instance to other.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>-1 if this instance is less than other, 0 if this instance is equal to other,
        /// 1 if this instance is greater than other.</returns>
        public int CompareTo(Number other)
        {
            if (this == other) return 0;
            if (this < other) return -1;
            return 1;
        }

        #endregion

        /// <summary>
        /// Gets the byte at the specified index.
        /// An index of 0 returns the LSD (least significant digit).
        /// An index of 6 returns the MSD (most significant digit).
        /// An index of 7 returns the exponent byte.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private static byte GetByte(ulong number, int bytePos)
        {
            return (byte)((number & digitMasks[bytePos]) >> (bytePos * 8));
        }

        /// <summary>
        /// Modifies the specified number by setting the byte in the specified
        /// position to the specified byteVal. If byteVal > 99 then
        /// the byte is set to byteVal - 100 and a carry over is performed.
        /// This keeps all of the digits "normalized" to Radix 100.
        /// </summary>
        /// <param name="number">The ulong value to modify</param>
        /// <param name="bytePos">The byte within the ulong value to modify. 0 = least significant
        /// (right-most) byte. 7 is the most significant </param>
        /// <param name="byteVal"></param>
        private static void SetByte(ref ulong number, int bytePos, byte byteVal)
        {
            ulong newByte = ((ulong)byteVal) << (bytePos * 8);
            number = newByte + (number & ~digitMasks[bytePos]);
        }
    }

}
