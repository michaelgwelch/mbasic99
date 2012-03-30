using System;
using NUnit.Framework;

namespace mbasic
{
    [TestFixture]
    public class ReadTest : OutputExpectationTest
    {
        public ReadTest() : base("../../../samples/read.mbas")
        {
            AddExpectedLine(" 22  15 ");
            AddExpectedLine(" 36  52 ");
            AddExpectedLine(" 48  96.5 ");
            
            AddExpectedLine(" 2  4  6  8  10  12  14  16 ");
            AddExpectedLine(" 12  14  16  18  20  22  24 ");
            AddExpectedLine(" 26 ");
        }
        
        [Test]
        public override void Invoke()
        {
            base.Invoke();
        }
    }
}

