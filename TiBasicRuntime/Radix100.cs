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

namespace TiBasicRuntime
{
    public struct Radix100
    {
        // Radix 100 is laid out like this
        //      7   6   5   4   3   2   1
        // EXP MSD                     LSD
        //  XX  XX  XX  XX  XX  XX  XX  XX

        // If Negative the most significant bit is set.
        // The specs say that the Exp and MSD should be complemented but I don't see the need
        // so i won't for my implementation.

        public static readonly Radix100 MaxValue = new Radix100(0x7F63636363636363); // 9.9999999999999E127
        public static readonly Radix100 MinValue = new Radix100(0xFF63636363636363); //-9.9999999999999E127
        public static readonly Radix100 Epsilon = new Radix100(0x0001000000000000);  // 1E-128
        public static readonly Radix100 Zero = new Radix100(0x4000000000000000);
        public static readonly Radix100 One = new Radix100(0x4001000000000000);

        private readonly ulong val;

        private Radix100(UInt64 val) { this.val = val; }

        public static double ToDouble(Radix100 r)
        {
            int expValue = GetExponent(r);

            double val = 0;
            double powerValue = Math.Pow(100, expValue);
            for (int i = 6; i >= 0; i--)
            {
                ulong digit = (r.val & digitMasks[i]) >> (i * 8);
                if (digit != 0) val = val + digit * powerValue;

                powerValue /= 100;
            }
            return val * Radix100.Sign(r);
        }

        public static Radix100 FromInteger(long intVal)
        {
            // I think this should be okay.
            return FromDouble(intVal);
        }

        public static Radix100 FromDouble(double d)
        {
            // Let's retrieve the mantissa, exponent and sign in terms of Radix 100.
            int sign = Math.Sign(d);
            sbyte exponent = (sbyte) Math.Floor(Math.Log(Math.Abs(d), 100));
            double mantissa = Math.Abs(d / Math.Pow(100, exponent));


            ulong result = 0;
            // set exponent properly
            byte biasedExponent = BiasedExponentValue(exponent);
            SetByte(ref result, 7, biasedExponent);

            byte digit;

            // loop through digits
            for (int i = 6; i >= 0 && mantissa > 0; i--)
            {
                digit = (byte)Math.Truncate(mantissa);
                SetByte(ref result, i, digit);
                mantissa = (mantissa * 100) - (digit * 100);
            }

            // Now check the remaining mantissa. If it is >= 50 then we should round up
            // the last digit.
            bool roundUp = (mantissa >= 50.0);
            Radix100 roundUpValue = new Radix100(0);
            if (roundUp)
            {
                // Create a Radix100 with same exponent with a 1 in the least significant
                // digit of the mantissa. This can then be added to our result.
                ulong r = 0;
                SetByte(ref r, 7, biasedExponent);
                SetByte(ref r, 0, 1);
                roundUpValue = new Radix100(r);
            }


            Radix100 retVal = new Radix100(result);
            if (roundUp) retVal = retVal + roundUpValue;
            if (sign < 0) retVal = Radix100.Negate(retVal);
            return retVal;
        }

        static readonly ulong[] digitMasks = new ulong[] {
            0x00000000000000FF,
            0x000000000000FF00,
            0x0000000000FF0000,
            0x00000000FF000000,
            0x000000FF00000000,
            0x0000FF0000000000,
            0x00FF000000000000,
            0xFF00000000000000};

        const ulong exponentMask = 0x7F00000000000000;
        const byte exponentShift = 56; // 56 bits

        #region Properties 

        static readonly ulong[] isIntMasks = new ulong[] {
            0x0000FFFFFFFFFFFF,
            0x000000FFFFFFFFFF,
            0x00000000FFFFFFFF,
            0x0000000000FFFFFF,
            0x000000000000FFFF,
            0x00000000000000FF,
            0x0000000000000000 };
        public bool IsInteger
        {
            get
            {
                // Get Exponent Byte
                sbyte expValue = GetExponent(this);

                if (expValue < 0 || expValue > 6) return false;

                // else use masks to see if this is an int --

                // if expValue = 0 then only digit 7 can have a value and this still be an int
                // if expValue = 1 then only digits 7 and 6 can have a value and this still be an int
                // if expValue = 2 then 7,6, and 5 can have values
                // if expValue < 0 then this is not an int.


                UInt64 mask = isIntMasks[expValue];
                return (mask & val) == 0;
            }
        }

        /// <summary>
        /// Returns the number of digits in this number. It does not count leading
        /// or trailing zeros. This is not just the significant digits. It is the
        /// total number of digits (e.g.  1237637000000 has 13 digits).
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfDigits()
        {
            // Each byte is one Radix 100 digit and can therefore
            // contributes 2 decimal digits.
            // The most significant Radix 100 digit contributes 1 decimal digit
            // if it is less than 10, else it also contributes 2 digits.
            // So we initialize result to 1 or 2 based on what we find in most 
            // significant Radix 100 digit. 
            // Then starting with least significant Radix100 digit we start looking
            // for the first non-zero value. If the least significant digit is not
            // 0 then we add 12 more digits to result. If it is zero but the second to
            // least signficant digit is non zero then we add 10 digits and so on.
            // It can be sumarized in the formula y = -2x + 12
            // where y is the number of additional decimal digits to add to result
            // and x represents the index of the first non zero Radix 100 digit starting
            // with the least significant.

            sbyte exponent100 = GetExponent(this); // exponent for base 100
            short exponent10 = (short) (exponent100 * 2);

            int exponentSign = Math.Sign(exponent100);

            byte msd = GetByte(this.val, 6);
            int leftOfDecimalDigits = (msd > 9) ? 2 : 1;
            int result = leftOfDecimalDigits;
            for (int i = 0; i < 6; i++)
            {
                byte digit = GetByte(this.val, i);
                if (digit != 0)
                {  
                    result += ((-2 * i) + 12);
                    break;
                }
            }
            int rightOfDecimalDigits = result - leftOfDecimalDigits;

            int zeroPadding; // this is the number of zeros between mantissa and decimal point added when we account for exponent

            // Now we know number of significant digits in mantissa.
            // Now we need to account for exponent shifts.
            if (exponentSign < 0) result += (-exponent10 - 1);
            else if ((zeroPadding = exponent10 - rightOfDecimalDigits) > 0) result += zeroPadding;
            return result;
        }

        #endregion




        #region Helper functions, masks, constants,

        public static byte BiasedExponentValue(sbyte normalizedExponent)
        {
            return (byte)(normalizedExponent + 0x40);
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

        private static byte GetByte(ulong number, int bytePos)
        {
            return (byte)((number & digitMasks[bytePos]) >> (bytePos * 8));
        }

 /*       private static double GetMantissa10(Radix100 r)
        {
            double val = (double)r;
            double logValue = Math.Log(Math.Abs(val), 10);
            long exponent = (long)Math.Floor(logValue);
            double divisor = Math.Pow(10, exponent);
            double mantissa = val / divisor;
            return mantissa;
        }
 */

        private static sbyte GetExponent(Radix100 r)
        {
            byte expByte = (byte)((r.val & exponentMask) >> exponentShift);
            return (sbyte)(expByte - 0x40);
        }

        private const ulong MantissaMask = 0x00FFFFFFFFFFFFFF;
        private static ulong GetMantissa100(Radix100 r)
        {
            return (r.val & MantissaMask);
        }

        #endregion Helper functions, masks, constants,

        #region String Processing

        /// <summary>
        /// Converts the specified double to an instance of Radix100 and then converts
        /// that to a string.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToString(double d)
        {
            return ((Radix100)d).ToString();
        }

        const string normalDecimalFormat = " ##########.########## ;-##########.########## ;0 ";
        const string scientificFormatString = " 0.#####E+00 ;-0.#####E+00 ";
        public override string ToString()
        {
            int numDigits = GetNumberOfDigits();
            if (numDigits <= 10) return ToNormalDecimalForm();
            if (IsInteger) return ToScientificForm();
            else
            {
                int exponent = GetExponent(this);
                if (exponent < -4 || exponent > 5) return ToScientificForm();
                return ToNormalDecimalForm();
                
            }
        }

        public string ToScientificForm()
        {
            StringBuilder bldr = new StringBuilder();
            if (Math.Sign(this) < 0) bldr.Append("-");

            Radix100 rounded = Radix100.Round(this, 6);
            sbyte exponent = GetExponent(this);
            int decimalExponent = exponent * 2;
            string stringFormat = "##";

            ulong rawBytes = rounded.val & 0x00FFFFFFFFFFFFFF;
            ulong integerPart;
            ulong fractionalPart;

            integerPart = (rawBytes & (~isIntMasks[0])) << 8;
            fractionalPart = (rawBytes & (isIntMasks[0])) << 16;

            // format integer part
            if (integerPart != 0)
            {
                byte theByte = GetByte(integerPart, 7);
                if (theByte > 9)
                {
                    bldr.Append(theByte / 10);
                    bldr.Append(".");
                    bldr.Append(theByte % 10);
                    decimalExponent++;
                }
                else
                {
                    bldr.Append(theByte.ToString(stringFormat));
                    bldr.Append(".");
                }
            }

            // format fractional part
            if (fractionalPart != 0)
            {
                stringFormat = "00";
                int firstNonZeroByte = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (GetByte(fractionalPart, i) != 0)
                    {
                        firstNonZeroByte = i;
                        break;
                    }
                }
                for (int i = 7; i > firstNonZeroByte; i--)
                {
                    bldr.Append(GetByte(fractionalPart, i).ToString(stringFormat));
                }
                int lastByte = GetByte(fractionalPart, firstNonZeroByte);
                int firstDigit = lastByte / 10;
                int secondDigit = lastByte % 10;
                bldr.Append(firstDigit);
                bldr.Append(secondDigit.ToString("#")); // if it is zero it won't print.
            }

            bldr.Append("E");
            if (decimalExponent > 99) bldr.Append(decimalExponent.ToString("+**;-**"));
            else bldr.Append(decimalExponent.ToString("+00;-00"));
            return bldr.ToString();

        }

        public string ToNormalDecimalForm()
        {
            StringBuilder bldr = new StringBuilder();
            if (Math.Sign(this) < 0) bldr.Append("-");

            Radix100 rounded = Radix100.Round(this, 10);

            sbyte exponent = GetExponent(this);
            string stringFormat = "##";

            ulong rawBytes = rounded.val & 0x00FFFFFFFFFFFFFF;
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
            if (integerPart != 0)
            {
                for (int i = 7; i >= (7 - exponent); i--)
                {
                    byte theByte = GetByte(integerPart, i);
                    bldr.Append(theByte.ToString(stringFormat));
                    if (theByte > 0) stringFormat = "00"; // print all the digits of the rest of the values
                }
            }

            if (!IsInteger) bldr.Append(".");
            if (exponent < 0)
            {
                for (int i = 0; i > exponent+1; i--) bldr.Append("00");
            }

            // format fractional part
            if (fractionalPart != 0)
            {
                stringFormat = "00";
                int firstNonZeroByte = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (GetByte(fractionalPart, i) != 0)
                    {
                        firstNonZeroByte = i;
                        break;
                    }
                }
                for (int i = 7; i > firstNonZeroByte; i--)
                {
                    bldr.Append(GetByte(fractionalPart, i).ToString(stringFormat));
                }
                int lastByte = GetByte(fractionalPart, firstNonZeroByte);
                int firstDigit = lastByte / 10;
                int secondDigit = lastByte % 10;
                bldr.Append(firstDigit);
                bldr.Append(secondDigit.ToString("#")); // if it is zero it won't print.
            }
            return bldr.ToString();

        }

        #endregion String Processing

        #region Math Operations
        public static Radix100 Round(Radix100 r, int numOfDecimalDigits)
        {
            // We need to find the byte that controls rounding. This depends
            // on the input - numOfDecimalDigits, and also on the number of
            // decimal digits in the most significant Radix100 digit of r.

            int numOfDigitsInMsd = GetByte(r.val, 6) > 10 ? 2 : 1;

            // This "normalizes" the digit we are looking for, If the most
            // significant Radix100 digit of r had only 1 decimal digit, then it
            // is like we are looking for the digit numOfDecimalDigits + 1. (This
            // accounts for the leading 0 digit in the MSD).
            numOfDecimalDigits = numOfDecimalDigits + (2 - numOfDigitsInMsd);

            // The digit that controls rounding is in the byte we grab here:
            byte bytePos = (byte) (7 - (numOfDecimalDigits + 2) / 2);
            byte byteOfConcern = GetByte(r.val, bytePos);

            // Now the digit that controls rounding is either in the 10s position
            // or 1s position of the byte we just grabbed.
            bool onesPosition = (numOfDecimalDigits % 2) != 0;
            byte digitOfConcern =(byte) (onesPosition ? (byteOfConcern % 10) : (byteOfConcern / 10));

            Radix100 roundUpVal = Radix100.Zero;
            if (digitOfConcern >= 5)
            {
                // Create a Radix100 that can be added to the truncted value
                // we create below to get the rounded value.
                ulong newVal = 0;
                SetByte(ref newVal, 7, GetByte(r.val, 7));
                // If our rounding digit was in onesPosition, then we need to put a
                // 1 in the tens position. Else put a 1 in the ones position of next higher byte.
                if (onesPosition) SetByte(ref newVal, bytePos, 10);
                else SetByte(ref newVal, bytePos + 1, 1);
                roundUpVal = new Radix100(newVal);
            }

            // the last decimal digit is in the byte we are going to grab:
            bytePos = (byte)(7 - (numOfDecimalDigits + 1) / 2);
            byteOfConcern = GetByte(r.val, bytePos);
            onesPosition = (numOfDecimalDigits % 2) == 0;
            digitOfConcern = (byte)(onesPosition ? (byteOfConcern % 10) : (byteOfConcern / 10));

            ulong truncatedValue = 0;
            for (int i = 7; i > bytePos; i--)
            {
                SetByte(ref truncatedValue, i, GetByte(r.val, i));
            }

            if (onesPosition) SetByte(ref truncatedValue, bytePos, GetByte(r.val, bytePos));
            else SetByte(ref truncatedValue, bytePos, (byte)(digitOfConcern*10));

            Radix100 result = new Radix100(truncatedValue);
            return result + roundUpVal;


        }

        public static int Sign(Radix100 r)
        {
            if (r.Equals(Radix100.Zero)) return 0;
            if (r.val > 0x8000000000000000) return -1;
            return 1;
        }

        public static Radix100 operator +(Radix100 r1, Radix100 r2)
        {
            if (r1.Equals(Zero)) return r2;
            if (r2.Equals(Zero)) return r1;

            sbyte exp1 = GetExponent(r1);
            sbyte exp2 = GetExponent(r2);
            sbyte exp = Math.Max(exp1, exp2);
            sbyte diff = (sbyte)(exp1 - exp2);

            ulong m1 = GetMantissa100(r1);
            ulong m2 = GetMantissa100(r2);

            if (diff > 0) m2 = m2 >> (diff * 8);
            else m1 = m1 >> (-diff * 8);

            ulong sum = 0;
            byte carryOver = 0;
            for (int i = 0; i < 7; i++)
            {
                byte b1 = (byte)((m1 & digitMasks[i]) >> (i * 8));
                byte b2 = (byte)((m2 & digitMasks[i]) >> (i * 8));
                ulong digitSum = (ulong)(b1 + b2 + carryOver);
                if (digitSum != 0)
                {
                    if (digitSum > 99)
                    {
                        digitSum -= 100;
                        carryOver = 1;
                    }
                    else carryOver = 0;
                    sum += (digitSum << (i * 8));
                }
            }
            if (carryOver == 1) exp++;
            sum += ((ulong)(exp + 0x40)) << exponentShift;

            return new Radix100(sum);

        }

        public static Radix100 Negate(Radix100 r)
        {
            ulong val = r.val ^ 0x8000000000000000;
            return new Radix100(val);
        }

        public static Radix100 Exp(Radix100 r)
        {
            double d = Math.Exp(r);
            Radix100 result = (Radix100)d;
            return result;
        }

        public static Radix100 Log(Radix100 r)
        {
            return (Radix100) Math.Log(r);
        }

        #endregion

        #region Conversion Operators


        /// <summary>
        /// Converts a Radix100 to a double.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static implicit operator double(Radix100 r)
        {
            return Radix100.ToDouble(r);
        }

        public static explicit operator Radix100(double d)
        {
            return Radix100.FromDouble(d);
        }

        public static implicit operator Radix100(int i)
        {
            return Radix100.FromInteger(i);
        }

        #endregion Conversion Operators
    }
}
