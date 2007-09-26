using System;
using System.Collections.Generic;
using System.Text;

namespace TIBasicRuntime
{
    public struct Array<T> where T : struct
    {
        private const int DefaultLowerBound = 0;
        private const int DefaultUpperBound = 10;

        // indicates the lower bound, -1 means it hasn't been initialized yet (it can be 0 or 1)
        private static int _lowerBound = -1;

        // The raw data storage.
        T[] _array;
        int length1;
        int length2;
        int length3;
        int upperBound1;
        int upperBound2;
        int upperBound3;


        public Array(int upperBound)
        {
            if (upperBound < LowerBound) throw new ArgumentOutOfRangeException("upperBound");

            this.upperBound1 = upperBound;
            this.upperBound2 = -1;
            this.upperBound3 = -1;
            this._array = Initialize(upperBound1, upperBound2, upperBound3, out length1, out length2, out length3);

        }

        private static T[] Initialize(int upperBound1, int upperBound2, int upperBound3, 
            out int length1, out int length2, out int length3)
        {
            int lowerBound = LowerBound;
            length1 = GetLength(upperBound1);
            length2 = GetLength(upperBound2);
            length3 = GetLength(upperBound3);

            int length = length1 * (length2 == 0 ? 1 : length2) * (length3 == 0 ? 1 : length3);
            return new T[length];
        }

        public Array(int upperBound1, int upperBound2)
        {
            if (upperBound1 < LowerBound) throw new ArgumentOutOfRangeException("upperBound1");
            if (upperBound2 < LowerBound) throw new ArgumentOutOfRangeException("upperBound2");
            this.upperBound1 = upperBound1;
            this.upperBound2 = upperBound2;
            this.upperBound3 = -1;
            this._array = Initialize(upperBound1, upperBound2, upperBound3, 
                out length1, out length2, out length3);
        }

        public Array(int upperBound1, int upperBound2, int upperBound3)
        {
            if (upperBound1 < LowerBound) throw new ArgumentOutOfRangeException("upperBound1");
            if (upperBound2 < LowerBound) throw new ArgumentOutOfRangeException("upperBound2");
            if (upperBound3 < LowerBound) throw new ArgumentOutOfRangeException("upperBound3"); _array = null;
            this.upperBound1 = upperBound1;
            this.upperBound2 = upperBound2;
            this.upperBound3 = upperBound3;
            this._array = Initialize(upperBound1, upperBound2, upperBound3,
                out length1, out length2, out length3);
        }

        /// <summary>
        /// Calculates the length from LowerBound and the passed in upperBound.
        /// If upperBound is less than 0 then 0 is returned. 
        /// </summary>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        private static int GetLength(int upperBound)
        {
            if (upperBound < 0) return 0;
            return upperBound - LowerBound + 1;
        }

        private T[] Items
        {
            get
            {
                return _array;
            }
        }

        public static int LowerBound
        {
            set
            {
                if (_lowerBound != -1) throw new InvalidOperationException("LowerBound already set");
                if (value != 0 && value != 1) throw new ArgumentOutOfRangeException("LowerBound value must be 0 or 1");
                _lowerBound = value;
            }
            private get
            {
                if (_lowerBound == -1) _lowerBound = DefaultLowerBound;
                return _lowerBound; 
            }
        }

        #region Public Getters/Setters

        public T this[Number x]
        {
            get
            {
                return GetElement(x);
            }
            set
            {
                SetElement(x, value);
            }
        }

        public T GetElement(Number index1)
        {
            EnsureInitialized(1);
            int rawIndex = CalculateRawIndex(index1);
            return Items[rawIndex];
        }

        public void SetElement(Number index1, T value)
        {
            EnsureInitialized(1);
            int rawIndex = CalculateRawIndex(index1);
            Items[rawIndex] = value;
        }

        private int CalculateRawIndex(Number index1)
        {
            if (index1 < LowerBound || index1 > upperBound1) throw new ArgumentOutOfRangeException("index1");
            if (length2 != 0) throw new InvalidOperationException("incorrect number of indices specified");

            Number rounded = Number.Round(index1);
            int rawIndex = (int)(LowerBound == 0 ? rounded : rounded - Number.One);
            return rawIndex;
        }

        private void EnsureInitialized(int numDimensions)
        {
            if (_array == null)
            {
                // Initialize upper bounds (assume 2 and 3 won't be used. We'll get 'em in the switch if we have to
                upperBound1 = DefaultUpperBound;
                upperBound2 = -1;
                upperBound3 = -1;
                switch (numDimensions)
                {
                    case 1:
                        _array = Initialize(DefaultUpperBound, -1, -1, out length1, out length2, out length3);
                        break;
                        
                    case 2:
                        upperBound2 = DefaultUpperBound;
                        _array = Initialize(DefaultUpperBound, DefaultUpperBound, -1, out length1, out length2, out length3);
                        break;

                    case 3:
                        upperBound2 = DefaultUpperBound;
                        upperBound3 = DefaultUpperBound;
                        _array = Initialize(DefaultUpperBound, DefaultUpperBound, DefaultUpperBound, out length1, out length2, out length3);
                        break;
                }
            }
        }

        public T this[Number x, Number y]
        {
            get
            {
                return GetElement(x, y);
            }
            set
            {
                SetElement(x, y, value);
            }
        }

        public T GetElement(Number index1, Number index2)
        {
            EnsureInitialized(2);
            int rawIndex = CalculateRawIndex(index1, index2);
            return Items[rawIndex];

        }

        public void SetElement(Number index1, Number index2, T value)
        {
            EnsureInitialized(2);
            int rawIndex = CalculateRawIndex(index1, index2);
            Items[rawIndex] = value;
        }

        private int CalculateRawIndex(Number index1, Number index2)
        {
            if (index1 < LowerBound || index1 > upperBound1) throw new ArgumentOutOfRangeException("index1");
            if (index2 < LowerBound || index1 > upperBound2) throw new ArgumentOutOfRangeException("index2");
            if (length2 == 0 || length3 != 0) throw new InvalidOperationException("incorrect number of indices specified");

            index1 = Number.Round(index1);
            index2 = Number.Round(index2);

            index1 = LowerBound == 0 ? index1 : index1 - Number.One;
            index2 = LowerBound == 0 ? index2 : index2 - Number.One;

            int rawIndex = (int)((index1 * length2) + index2);
            return rawIndex;
        }

        public T this[Number x, Number y, Number z]
        {
            get
            {
                return GetElement(x, y, z);
            }
            set
            {
                SetElement(x, y, z, value);
            }
        }

        public T GetElement(Number index1, Number index2, Number index3)
        {
            EnsureInitialized(3);
            int rawIndex = CalculateRawIndex(index1, index2, index3);
            return Items[rawIndex];
        }

        public void SetElement(Number index1, Number index2, Number index3, T value)
        {
            EnsureInitialized(3);
            int rawIndex = CalculateRawIndex(index1, index2, index3);
            Items[rawIndex] = value;
        }

        private int CalculateRawIndex(Number index1, Number index2, Number index3)
        {
            if (index1 < LowerBound || index1 > upperBound1) throw new ArgumentOutOfRangeException("index1");
            if (index2 < LowerBound || index1 > upperBound2) throw new ArgumentOutOfRangeException("index2");
            if (index3 < LowerBound || index1 > upperBound3) throw new ArgumentOutOfRangeException("index3");

            if (length3 == 0) throw new InvalidOperationException("incorrect number of indices specified");

            index1 = Number.Round(index1);
            index2 = Number.Round(index2);
            index3 = Number.Round(index3);

            index1 = LowerBound == 0 ? index1 : index1 - Number.One;
            index2 = LowerBound == 0 ? index2 : index2 - Number.One;
            index3 = LowerBound == 0 ? index3 : index3 - Number.One;

            int rawIndex = (int)((index1 * length2 * length3) + (index2 * length3) + index3);
            return rawIndex;
        }

        #endregion

    }
}
