using System;
using NUnit.Framework;

namespace mbasic
{
    [TestFixture()]
	public class GosubTest : OutputExpectationTest
    {
        public GosubTest()
        {
            AddExpectedLine("FIRST ARRAY");
            AddExpectedLine("");
            AddExpectedLine(" 1  2  3  4  5  6  7 ");
            AddExpectedLine(" 2  4  6  8  10  12  14 ");
            AddExpectedLine(" 3  6  9  12  15  18  21 ");
            AddExpectedLine(" 4  8  12  16  20  24  28 ");
            AddExpectedLine("");
            AddExpectedLine("3 TIMES VALUES IN FIRST ARRA");
            AddExpectedLine("Y");
            AddExpectedLine("");
            AddExpectedLine(" 3  6  9  12  15  18  21 ");
            AddExpectedLine(" 6  12  18  24  30  36  42 ");
            AddExpectedLine(" 9  18  27  36  45  54  63 ");
            AddExpectedLine(" 12  24  36  48  60  72  84 ");


        }
        
        [Test()]
        public void Invoke()
        {
            // Arrange
            var program = "../../../gosub.mbas";
            
            // Act
            Run(program);
            
            // Assert
            AssertMatch();
                   
        }
    }
}

