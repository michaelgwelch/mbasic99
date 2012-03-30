using System;
using NUnit.Framework;

namespace mbasic
{
    [TestFixture()]
	public class ArrayTest : OutputExpectationTest
    {
        public ArrayTest() : base("../../../samples/array.mbas")
        {
            AddExpectedLine(" 1  2   1  4   1  6   1  8  ");
            AddExpectedLine(" 3  2   3  4   3  6   3  8  ");
            AddExpectedLine(" 5  2   5  4   5  6   5  8  ");
            AddExpectedLine(" 7  2   7  4   7  6   7  8  ");
        }
        
        [Test()]
        public override void Invoke()
        {
            base.Invoke();
        }
    }
}

