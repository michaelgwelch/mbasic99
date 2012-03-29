using System;
using System.IO;

namespace Tests
{
    public abstract class OutputExpectationTest
    {
        private readonly OutputExpectationBuilder builder = new OutputExpectationBuilder();
        private readonly TextWriter writer = new StringWriter();
        
        protected OutputExpectationTest()
        {
            Console.SetOut(writer);
        }
        
        public void AddExpectedLine(string expected)
        {
            builder.AddExpectedLine(expected);
        }
        
        public void AssertMatch()
        {
            writer.Flush();
            builder.AssertAll(writer.ToString());
        }
        
        
    }
}

