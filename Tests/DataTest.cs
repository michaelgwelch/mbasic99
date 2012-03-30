using System;
using NUnit.Framework;

namespace mbasic
{
    [TestFixture()]
	public class DataTest : OutputExpectationTest
    {
        public DataTest() : base("../../../samples/data.mbas")
        {
            AddExpectedLine(" 2  4 ");
            AddExpectedLine(" 6  7 ");
            AddExpectedLine(" 8  1 ");
            AddExpectedLine(" 2  3 ");
            AddExpectedLine(" 4  5 ");
            AddExpectedLine("HELLO");
            AddExpectedLine("JONES, MARY");
            AddExpectedLine(" 28 ");
            AddExpectedLine(" 3.1416 ");
            AddExpectedLine("A$ IS HI");
            AddExpectedLine("B$ IS ");
            AddExpectedLine("C IS  2 ");
            AddExpectedLine("A$ IS ");
            AddExpectedLine("this is the first string.");
            AddExpectedLine("B$ IS .");
            AddExpectedLine("C$ IS .");
            AddExpectedLine("D IS  5 .");
        }
        [Test()]
        public override void Invoke()
        {
            base.Invoke();
        }
    }
}

