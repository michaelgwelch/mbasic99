using System;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace mbasic
{
    [TestFixture()]
	public class CheckParensTest
    {
        string output;
        
        [Test]
        public void TestCase()
        {
            // Setup Process
            Process process = new Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.UseShellExecute = false;
            

            
            process.StartInfo.FileName = "../../../bin/mbasic.exe";
            process.StartInfo.Arguments = "../../../samples/checkparens.mbas";
            
            process.Start();
            

            Thread reader = new Thread(obj => ReadProcessOutput((StreamReader)obj));
            Thread writer = new Thread(obj => WriteProcessInput((StreamWriter)obj));
 
            writer.Start(process.StandardInput);            
            reader.Start(process.StandardOutput);

            writer.Join();            
            reader.Join();

                                     
            process.WaitForExit();
            process.Dispose();
            
            StringReader stringReader = new StringReader(output);
   
            Console.WriteLine(output);
            Assert.AreEqual("String: ", stringReader.ReadLine());
            Assert.AreEqual("Another String (Y or N): ", stringReader.ReadLine());
            Assert.AreEqual("", stringReader.ReadToEnd());
            
            
        }
        
        private string ReadPrompt(StreamReader reader)
        {
            char ch;
            StringBuilder builder = new StringBuilder();
            
            while((ch = (char)reader.Read()) != 0)
            {
                builder.Append(ch);
            }
            return builder.ToString();
            
        }
        
        IList<string> strings = new List<string>();
        private void ReadProcessOutput(StreamReader reader)
        {
            output = reader.ReadToEnd();
        }
        
        private void WriteProcessInput(StreamWriter writer)
        {
            writer.WriteLine("{([])}");
            writer.WriteLine("N");
            writer.Flush();
        }
    }
}

