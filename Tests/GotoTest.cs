using System;
using NUnit.Framework;

namespace mbasic
{
    [TestFixture()]
	public class GotoTest : OutputExpectationTest
    {
        public GotoTest()
        {
            AddExpectedLine("Total number of gifts is 78 ");
        }
        
        [Test()]
        public void Invoke()
        {
            // Arrange
            var program = "../../../samples/goto.mbas";
            
            // Act
            Run(program);
            
            // Assert
            AssertMatch();
                
        }
    }
}

