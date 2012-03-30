using System;
using NUnit.Framework;

namespace mbasic
{
    [TestFixture()]
	public class Array2Test : OutputExpectationTest
    {
        public Array2Test() : base("../../../samples/array2.mbas")
        {
            AddExpectedLine(" 1 |  2  3  4  5 ");
            AddExpectedLine("");
            AddExpectedLine("~~~~~~~~~~~~~~~~~~");
            AddExpectedLine(" 2 |  4  6  8  10 ");
            AddExpectedLine(" 3 |  6  9  12  15 ");
            AddExpectedLine(" 4 |  8  12  16  20 ");
            AddExpectedLine(" 5 |  10  15  20  25 ");
        }
        
        [Test()]
        public override void Invoke()
        {
            base.Invoke();
        }
    }
}

