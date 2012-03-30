using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;

namespace mbasic
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
                if (lineIndex >= strings.Count) Assert.AreEqual(strings.Count, lineIndex+1, "Number of lines expected is less than what was found");
                var expected = strings[lineIndex];
                lineIndex++;
                if (actual != expected)
                {
                    builder.AppendFormat("Line {0}: expected '{1}', actual '{2}'\n", lineIndex, expected, actual);
                }
            }
            
            Assert.AreEqual(strings.Count, lineIndex, "Number of lines read doesn't equal number of expected lines");
            var result = builder.ToString();
            if (result != string.Empty) Assert.Fail(result);
        }
    }
}

