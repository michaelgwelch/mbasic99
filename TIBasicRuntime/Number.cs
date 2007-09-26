using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TIBasicRuntime
{
    public struct Number
    {
        readonly double val;

        public static readonly Number Zero = new Number(0);
        public static readonly Number One = new Number(1);
        public static readonly Number MinusOne = new Number(-1);

        public static readonly Number True = new Number(-1);
        public static readonly Number False = new Number(0);

        static Random rand;

        static Number()
        {
            // By default (if Randomize is not called)
            // We get a known seed
            RandomizeWithSeed(Zero);
        }

        #region Conversions to Number from .NET and to .NET from Number
        public Number(byte[] bytes)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(bytes));
            val = reader.ReadDouble();
        }

        public Number(double d)
        {
            this.val = d;
        }

        public Number(int i) : this((double)i)
        {
        }

        public static implicit operator Number(double d)
        {
            return new Number(d);
        }

        public static bool operator true(Number num)
        {
            return (num.val != False.val);
        }

        public static bool operator false(Number num)
        {
            return (num.val == False.val);
        }

        public static explicit operator bool(Number num)
        {
            if (num) return true;
            else return false;
        }

        public static explicit operator int(Number num)
        {
            return (int)num.val;
        }

        public override string ToString()
        {
            return val.ToString();
        }
        #endregion

        #region Basic Arithmetic Operations

        public static Number Negate(Number num)
        {
            return new Number(-num.val);
        }

        public static Number Add(Number num1, Number num2)
        {
            return new Number(num1.val + num2.val);
        }

        public static Number Subtract(Number num1, Number num2)
        {
            return new Number(num1.val - num2.val);
        }

        public static Number Multiply(Number num1, Number num2)
        {
            return new Number(num1.val * num2.val);
        }

        public static Number Divide(Number num1, Number num2)
        {
            return new Number(num1.val / num2.val);
        }

        public static Number Power(Number num1, Number num2)
        {
            return new Number(Math.Pow(num1.val, num2.val));
        }

        public static Number Increment(Number num1)
        {
            return num1 + Number.One; 
        }

        public static Number Decrement(Number num1)
        {
            return num1 - Number.One;
        }

        public static Number operator -(Number num1)
        {
            return Number.Negate(num1);
        }

        public static Number operator +(Number num1)
        {
            return num1;
        }

        public static Number operator +(Number num1, Number num2)
        {
            return Number.Add(num1, num2);
        }

        public static Number operator -(Number num1, Number num2)
        {
            return Number.Subtract(num1, num2);
        }

        public static Number operator *(Number num1, Number num2)
        {
            return Number.Multiply(num1, num2);
        }

        public static Number operator /(Number num1, Number num2)
        {
            return Number.Divide(num1, num2);
        }

        public static Number operator ++(Number num1)
        {
            return Number.Increment(num1);
        }

        public static Number operator --(Number num1)
        {
            return Number.Decrement(num1);
        }

        public static Number Remainder(Number num1, Number num2)
        {
            return new Number(num1.val % num2.val);
        }
        public static Number operator %(Number num1, Number num2)
        {
            return Remainder(num1, num2);
        }
        #endregion

        #region Relational Operations

        public static Number Compare(Number num1, Number num2)
        {
            if (num1.val < num2.val) return MinusOne;
            else if (num1.val == num2.val) return Zero;
            else return One;
        }

        public static Number operator < (Number num1, Number num2)
        {
            if (Compare(num1, num2).val < 0) return True;
            else return False;
        }

        public static Number operator <= (Number num1, Number num2)
        {
            if (Compare(num1, num2).val <= 0) return True;
            else return False;
        }

        public static Number operator > (Number num1, Number num2)
        {
            if (Compare(num1, num2).val > 0) return True;
            else return False;
        }

        public static Number operator >=(Number num1, Number num2)
        {
            if (Compare(num1, num2).val >= 0) return True;
            else return False;
        }


        #endregion

        #region Equality Operations

        public static Number Equals(Number num1, Number num2)
        {
            return num1.Equals(num2);
        }

        public static Number operator == (Number num1, Number num2)
        {
            return Number.Equals(num1, num2);
        }

        public static Number operator !=(Number num1, Number num2)
        {
            return !Number.Equals(num1, num2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Number)) return false;

            // the following line looks odd I know, but
            // since Equals returns a Number we must convert
            // it to a boolean. We can do it in the following fashion
            // because Number implements the true operator. (It doesn't
            // however implement an implicit or explicit conversion to bool.
            return (Equals((Number)obj) ? true : false);
        }

        public Number Equals(Number num)
        {
            if (val == num.val) return True;
            else return False;
        }

        #endregion

        #region Boolean Operations

        public static Number operator !(Number num1)
        {
            if (num1.val == False.val) return True;
            else return False;
        }

        public static Number operator &(Number num1, Number num2)
        {
            return Number.Multiply(num1, num2);
        }

        public static Number operator |(Number num1, Number num2)
        {
            return Number.Add(num1, num2);
        }

        public static Number operator ^(Number num1, Number num2)
        {
            return ((num1 && !num2) || (!num1 && num2));
        }

        #endregion

        #region TI Basic Builtin Numeric Functions

        public static Number AbsoluteValue(Number num)
        {
            return new Number(Math.Abs(num.val));
        }

        public static Number Arctangent(Number num)
        {
            return new Number(Math.Atan(num.val));
        }

        public static Number Cosine(Number num)
        {
            return new Number(Math.Cos(num.val));
        }

        /// <summary>
        /// Returns the value Power(e, num);
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Number Exponential(Number num)
        {
            return new Number(Math.Exp(num.val));
        }

        public static Number Integer(Number num)
        {
            return new Number(Math.Floor(num.val));
        }

        /// <summary>
        /// Returns the natural logarithm of num.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Number Logarithm(Number num)
        {
            return new Number(Math.Log(num.val));
        }


        public static void Randomize()
        {
            rand = new Random();
        }

        public static void RandomizeWithSeed(Number seed)
        {
            // Get the first four bytes of the double.
            byte[] buffer = new byte[8];
            Stream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(seed.val);

            stream.Position = 0;
            BinaryReader reader = new BinaryReader(stream);
            int intSeed = reader.ReadInt32();

            rand = new Random(intSeed);
        }

        public static Number RandomNumber()
        {
            return new Number(rand.NextDouble());
        }

        public static Number Sign(Number num)
        {
            return new Number(Math.Sign(num.val));
        }

        public static Number Sine(Number num)
        {
            return new Number(Math.Sin(num.val));
        }

        public static Number SquareRoot(Number num)
        {
            return new Number(Math.Sqrt(num.val));
        }

        public static Number Tangent(Number num)
        {
            return new Number(Math.Tan(num.val));
        }

        public static TIString Character(Number num)
        {
            Number ascii = Number.Integer(num);
            if (ascii < Number.Zero || ascii > new Number(32767)) throw new ArgumentOutOfRangeException("num", "Number must not be less than 0 or greater than 32767");
            return new TIString(((char)((int)ascii)).ToString());
        }

        public static TIString ToTIString(Number num)
        {
            return new TIString(num.val.ToString());
        }

        public static Number Parse(TIString str)
        {
            double val;
            if (double.TryParse(str.ToString(), out val)) return new Number(val);
            throw new FormatException("str is not a valid number");
        }
        #endregion

        #region Nice to haves
        public static Number Minimum(Number num1, Number num2)
        {
            if (num1 < num2) return num1;
            return num2;
        }

        public static Number Round(Number num)
        {
            return new Number(Math.Round(num.val));
        }
        #endregion

        public override int GetHashCode()
        {
            return val.GetHashCode();
        }



        public static double ToDouble(Number num)
        {
            return num.val;
        }

        /// <summary>
        /// Should return the TI representation, (which will result in loss of precision),
        /// but for now returns the double bit pattern
        /// </summary>
        /// <returns></returns>
        public static byte[] GetBits(Number num)
        {
            byte[] buffer = new byte[8];
            BinaryWriter writer = new BinaryWriter(new MemoryStream(buffer));
            writer.Write(num.val);
            return buffer;
        }
    }
}
