using System;
using System.Diagnostics;
using System.IO;

namespace mbasic
{
    public abstract class OutputExpectationTest : IDisposable
    {
        private readonly OutputExpectationBuilder builder = new OutputExpectationBuilder();
        private readonly Process process;
        private readonly string path;
        
        protected OutputExpectationTest(string path)
        {
            process = new Process();
            process.StartInfo.FileName = "../../../bin/mbasic.exe";
            this.path = path;
        }
        
        public void AddExpectedLine(string expected)
        {
            builder.AddExpectedLine(expected);
        }
        
        public void AssertMatch()
        {
            builder.AssertAll(process.StandardOutput.ReadToEnd());
        }
        
        public void Run(string path)
        {
            process.StartInfo.Arguments = path;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();  
        }
        
        public void Dispose()
        {
            process.Dispose();
        }
        
        public virtual void Invoke()
        {
            // Arrange
            
            // Act
            Run(path);
            
            // Assert
            AssertMatch();
        }
        
    }
}

