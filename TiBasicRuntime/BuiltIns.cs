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
using System.Collections;

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

        public static void RandomizeWithSeed(double seed)
        {
            if (seed > Int32.MaxValue) seed = Int32.MaxValue;
            if (seed < Int32.MinValue) seed = Int32.MinValue;
            int intSeed = Convert.ToInt32(seed);
            rand = new Random(intSeed);
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
            return Radix100.FromDouble(val).ToString();
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




        //public static void Trace(string label)
        //{
        //    currentLabel = label;
        //    if (trace) Console.Write(label);
        //}

        static bool trace;
        //static string currentLabel;
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

        static int printCol = 1;
        public static void Print(params object[] items)
        {
            if (items.Length == 0) 
            {
                Console.WriteLine();
                printCol = 1;
                return;
            }

            bool printSepInEffect = false;
            foreach (object o in items)
            {
                string s;
                if (o is string)
                {
                    s = o as string;
                    // handle zero length strings first.
                    if (s.Length == 0)
                    {
                        printSepInEffect = false;
                        continue;
                    }

                    char ch = s[0];
                    switch (ch)
                    {
                        case '\0': // nothing to print.
                            printSepInEffect = true;
                            break;
                        case '\t':
                            if (s.Length == 1) PrintComma();
                            else PrintTab(s);
                            printSepInEffect = true;
                            break;
                        case '\n':
                            PrintNewLine();
                            printSepInEffect = true;
                            break;
                        default:
                            PrintString(s);
                            printSepInEffect = false;
                            break;
                    }
                }
                else
                {
                    PrintNumber((double)o);
                    printSepInEffect = false;
                }

            }

            if (!printSepInEffect) PrintNewLine();

            
        }

        private static void PrintComma()
        {
            if (printCol < 15) PrintItem(new String(' ', 15 - printCol));
            else PrintNewLine();
        }
        private static void PrintTab(string s)
        {
            int doubleLength = s.Length - 1;
            double d = double.Parse(s.Substring(1, doubleLength));
            int n = (int) (Math.Round(d) % 28);
            if (n < 1) n = 1;
            if (printCol > n) PrintNewLine();
            PrintItem(new String(' ', n - printCol));
        }

        private static void PrintSpace()
        {
            PrintItem(" ");
        }

        private static void PrintNumber(double d)
        {
            string s = Radix100.ToString(d);
            if (s.Length > RemainingPrintColumns) PrintNewLine();
            if (d >= 0) PrintSpace(); // Positive numbers are printed with leading space 
            PrintItem(s);
            if (printCol < 29) PrintSpace();
        }

        private static void PrintItem(string s)
        {
            Console.Write(s);
            printCol += s.Length;
        }

        private static void PrintString(string s)
        {
            if (s.Length > RemainingPrintColumns)
            {
                if (printCol != 1) PrintNewLine();
            }
            if (s.Length <= 28)
            {
                PrintItem(s);
                return;
            }

            int index = 0;
            while (index < s.Length)
            {
                int charsToPrint = Math.Min(28, s.Length - index);
                PrintItem(s.Substring(index, charsToPrint));
                index += charsToPrint;
                if (charsToPrint == 28) PrintNewLine();
            }
        }

        private static void PrintNewLine()
        {
            Console.WriteLine();
            ResetColumnPos();
        }

        private static void ResetColumnPos() { printCol = 1; }
        private static int RemainingPrintColumns { get { return 28 - printCol + 1; } }

        private static SortedList<string, object[]> data = new SortedList<string, object[]>();
        private static int labelIndex = 0;
        private static int pos = 0;
        public static void AddData(string label, params object[] objects)
        {
            data.Add(label, objects);
        }

        public static double ReadNumberFromData()
        {
            double d;
            object o = Read();
            if (o is double)
            {
                d = (double)o;
                return d;
            }
            else
            {
                throw new Exception("DATA ERROR");
            }
        }

        public static string ReadStringFromData()
        {
            string s;
            object o = Read();
            if (o is string)
            {
                s = (string)o;
            }
            else
            {
                s = Radix100.ToString((double)o);
            }
            return s;
        }

        private static object Read()
        {
            if (labelIndex == data.Count) throw new Exception("DATA ERROR");
            object[] dataList = data.Values[labelIndex];

            if (pos == dataList.Length) throw new Exception("DATA ERROR");
            object o = dataList[pos];

            pos++;
            if (pos == dataList.Length)
            {
                labelIndex++;
                pos = 0;
            }
            return o;
        }

        public static void RestoreToBeginning()
        {
            labelIndex = 0;
            pos = 0;
        }

        public static void RestoreToLabel(string label)
        {
            // TODO: What if labelIndex is not found? Need to throw data error
            labelIndex = data.IndexOfKey(label);
            pos = 0;
        }

        #region GOSUB/RETURN Helpers
        private static readonly Stack<int> gosubs = new Stack<int>();
        public static void PushReturnAddress(int index)
        {
            gosubs.Push(index);
        }

        public static int PopReturnAddress()
        {
            return gosubs.Pop();
        }
        #endregion

        private static string[] consoleValues;
        private static int consoleValuesIndex;
        public static void ReadLineFromConsoleIntoBuffer(int expectedNumberOfValues)
        {
            string line = Console.ReadLine();
            printCol = 1; 
            InputParser parser = new InputParser(line);
            List<string> strings = new List<string>();
            while (!parser.EndOfString) strings.Add(parser.Next());

            if (strings.Count == 0) // then the user just hit enter, or spaces and enter, which is considered a valid zero length string
            {
                strings.Add(String.Empty); // This entry should be considered a zero length string.
            }


            while (strings.Count != expectedNumberOfValues)
            {
                Console.WriteLine("* WARNING:");
                Console.WriteLine("  INPUT ERROR");
                Print("TRY AGAIN: ","\0");
                line = Console.ReadLine();
                parser = new InputParser(line);
                strings = new List<string>();
                while (!parser.EndOfString) strings.Add(parser.Next());
            }
            consoleValues = strings.ToArray();
            consoleValuesIndex = 0;

        }

        public static double ReadNumberFromConsole()
        {
            string s = consoleValues[consoleValuesIndex];
            double d;
            if (!double.TryParse(s, out d))
            {
                Console.WriteLine("* WARNING:");
                Console.WriteLine("  INPUT ERROR");
                Print("TRY AGAIN: ","\0");
                throw new InvalidCastException();
            }
            consoleValuesIndex++;
            return d;
        }

        public static string ReadStringFromConsole()
        {
            string s = consoleValues[consoleValuesIndex];
            consoleValuesIndex++;
            return s;
        }

        private static int optionBase = 0;
        private static bool optionBaseSet = false;

        public static void OptionBase(int optionBase)
        {
            if (optionBaseSet) throw new InvalidOperationException("Option Base already set");
            if (optionBase != 0 && optionBase != 1) throw new InvalidOperationException("Option Base must be 0 or 1");
            BuiltIns.optionBase = optionBase;
            BuiltIns.optionBaseSet = true;
        }

        public static Array CreateNumberArray(params int[] dimensions)
        {
            return CreateArray(dimensions, typeof(double));
        }

        public static Array CreateStringArray(params int[] dimensions)
        {
            Array retVal = CreateArray(dimensions, typeof(string));

            int firstDimensionUpperBound = retVal.GetUpperBound(0);
            int secondDimensionUpperBound = (dimensions.Length > 1) ? retVal.GetUpperBound(1) : 0;
            int thirdDimensionUpperBound = (dimensions.Length > 2) ? retVal.GetUpperBound(2) : 0;
            for (int i = optionBase; i <= firstDimensionUpperBound; i++)
            {
                if (dimensions.Length == 1) retVal.SetValue("", i);
                else
                {
                    for (int j = optionBase; j <= secondDimensionUpperBound; j++)
                    {
                        if (dimensions.Length == 2) retVal.SetValue("", i, j);
                        else
                        {
                            for (int k = optionBase; k <= thirdDimensionUpperBound; k++)
                                retVal.SetValue("", i, j, k);
                        }
                    }
                }
            }
            return retVal;
        }

        private static Array CreateArray(int[] dimensions, Type elementType)
        {
            int[] lowerBounds = new int[dimensions.Length];
            for (int i = 0; i < lowerBounds.Length; i++) lowerBounds[i] = optionBase;

            int[] lengths;
            if (optionBase == 1) lengths = dimensions;
            else
            {
                lengths = new int[dimensions.Length];
                for (int i = 0; i < lengths.Length; i++) lengths[i] = dimensions[i] + 1;
            }

            Array newArr = Array.CreateInstance(elementType, lengths, lowerBounds);
            return newArr;
        }
        
        #region Array Get and Set methods
        
        // These methods shouldn't be needed, but currently mono doesn't
        // synthesis the Get and Set methods of arrays. So an alternative would
        // be to use GetValue and SetValue but they only deal with objects, so
        // doubles would need to be boxed and unboxed. These go away when the bug 
        // is fixed. See http://bugzilla.ximian.com/show_bug.cgi?id=80567
        
        public static string GetStringValue1(string[] strings, int i)
        {
        	return strings[i];
        }
        
        public static string GetStringValue2(string[,] strings, int i, int j)
        {
        	return strings[i,j];
        }
        
        public static string GetStringValue3(string[,,] strings, int i, int j, int k)
        {
        	return strings[i,j,k];
        }
        
        public static void SetStringValue1(string[] strings, int i, string value)
        {
        	strings[i] = value;
        }
        
        public static void SetStringValue2(string[,] strings, int i, int j, string value)
        {
        	strings[i,j] = value;
        }
        
        public static void SetStringValue3(string[,,] strings, int i, int j, int k, string value)
        {
        	strings[i,j,k] = value;
        }
 
 		public static double GetNumberValue1(double[] doubles, int i)
 		{
 			return doubles[i];
 		}
 		
 		public static double GetNumberValue2(double[,] doubles, int i, int j)
 		{
 			return doubles[i,j];
 		}
 		
 		public static double GetNumberValue3(double[,,] doubles, int i, int j, int k)
 		{
 			return doubles[i,j,k];
 		}
 		
 		public static void SetNumberValue1(double[] nums, int i, double val)
 		{
 			nums[i] = val;
 		}
 		
 		public static void SetNumberValue2(double[,] nums, int i, int j, double val)
 		{
 			nums[i,j] = val;
 		}
 		
 		public static void SetNumberValue3(double[,,] nums, int i, int j, int k, double val)
 		{
 			nums[i,j,k] = val;
 		}
 		
 		
 		#endregion
 	
    }
}
