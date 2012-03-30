using System;
using NUnit.Framework;

namespace mbasic
{
    [TestFixture()]
	public class DataTest : OutputExpectationTest
    {
        public DataTest()
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
        public void Invoke()
        {
            // Arrange
            var program = "../../../samples/data.mbas";
            
            // Act
            Run(program);
            
            // Assert
            AssertMatch();
        }
    }
}

