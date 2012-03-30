using System;
using NUnit.Framework;

namespace mbasic
{
    [TestFixture()]
	public class GotoTest : OutputExpectationTest
    {
        public GotoTest() : base("../../../samples/goto.mbas")
        {
            AddExpectedLine("Total number of gifts is 78 ");
        }
        
        [Test()]
        public override void Invoke()
        {
            Invoke();
        }
    }
}

