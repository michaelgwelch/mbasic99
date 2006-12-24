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
    public static class BuiltIns
    {
        const string badArgumentIn = "* BAD ARGUMENT IN ";
        const string badValueIn = "* BAD VALUE IN ";

        static Random rand;

        static BuiltIns()
        {
            // By default (if Randomize is not called)
            // We get a known seed
            RandomizeWithSeed(0);
        }


        public static void Randomize()
        {
            rand = new Random();
        }

        public static void RandomizeWithSeed(int seed)
        {
            rand = new Random(seed);
        }

        public static double Rnd()
        {
            return rand.NextDouble();
        }

        public static double Int(double val)
        {
            return Math.Floor(val);
        }

        public static double Asc(string label, string s)
        {
            if (s.Length == 0) throw new Exception(badArgumentIn + label);
            return (double) s[0];
        }

        public static string Chr(string label, double val)
        {
            int ascii = (int) Int(val);
            if (ascii < 0 || ascii > 32767) throw new Exception(badValueIn + label);
            return ((char)ascii).ToString();
        }

        public static double Len(string s)
        {
            return (double)s.Length;
        }

        /// <summary>
        /// Searches for s2 in s1 beginning at startPos. The first char is position 1.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public static double Pos(string label, string s1, string s2, double startPos)
        {
            // TI-Basic is 1 based for strings. .NET is 0 based.
            int startIndex = ((int)Int(startPos)) - 1;
            if (startIndex < 0) throw new Exception(badValueIn + label);
            return (double)(s1.IndexOf(s2, startIndex) + 1);
        }

        /// <summary>
        /// Retrieves a substring from s. The substring starts at the specified
        /// startPos (first char is at position 1) and has the specified length.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="s"></param>
        /// <param name="startPos"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Seg(string label, string s, double startPos, double length)
        {
            int start = (int)Int(startPos) - 1;

            // len + start must be inside s. So we may have to reduce len
            // to avoid exceptions. This will give the expected behavior.
            int len = Math.Min((int)Int(length), s.Length-start);

            if (start < 0 || len < 0) throw new Exception(badValueIn + label);
            return s.Substring(start, len);
        }

        public static string Str(double val)
        {
            return val.ToString();
        }

        public static double Val(string label, string s)
        {
            double val;
            if (double.TryParse(s, out val)) return val;
            throw new Exception(badArgumentIn + label);
        }

        public static double Log(string label, double val)
        {
            if (val <= 0) throw new Exception(badArgumentIn + label);
            return Math.Log(val);
        }

        public static double Sqr(string label, double val)
        {
            if (val < 0) throw new Exception(badArgumentIn + label);
            return Math.Sqrt(val);
        }

        // Statements




        public static void Trace(string label)
        {
            currentLabel = label;
            if (trace) Console.Write(label);
        }

        static bool trace;
        static string currentLabel;
        public static bool TraceEnabled
        {
            get
            {
                return trace;
            }
            set
            {
                trace = value;
            }
        }
    }
}
