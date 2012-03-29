using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;

namespace Tests
{
    public class OutputExpectationBuilder
    {
        IList<string> strings = new List<string>();
        
        public void AddExpectedLine(string message)
        {
            strings.Add(message);
        }
        
        public void AssertAll(string output)
        {
            var builder = new StringBuilder();
            var reader = new StringReader(output);
            var lineIndex = 0;
            string actual;
            while((actual = reader.ReadLine()) != null) 
            {
                var expected = strings[lineIndex];
                lineIndex++;
                if (actual != expected)
                {
                    builder.AppendFormat("Line {0}: expected '{1}', actual '{2}'\n", lineIndex, expected, actual);
                }
            }
            var result = builder.ToString();
            if (result != string.Empty) Assert.Fail(result);
        }
    }
}

