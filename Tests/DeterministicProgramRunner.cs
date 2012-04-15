using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.IO;
using System.Collections.Generic;

namespace mbasic
{
    // Used to run a program where all the output from the 
    // program is already known based on the input provided.
    public class DeterministicProgramRunner
    {
        IList<string> outputMessages = new List<string>();
        StringBuilder inputBuilder = new StringBuilder();
        
        public DeterministicProgramRunner()
        {
        }
        
        public void ExpectOutput(string output)
        {
            outputMessages.Add(output);
        }
        
        public void AddInputLine(string input)
        {
            inputBuilder.AppendFormat("{0}\n", input);
        }
        
        public string Run(string fileName, string arguments)
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
            
            return output;

        }
  
        private string output;
        private void ReadProcessOutput(StreamReader reader)
        {
            for(int i = 0; i < outputMessages.Count; i++)
            {
                var output = outputMessages[i];
                var buffer = new char[output.Length];
                reader.ReadBlock(buffer, 0, buffer.Length);
                var actual = new String(buffer);
                if (output != actual)
                {
                    throw new Exception(string.Format("Expected string #{0} to be '{1}' but was '{2}'", i, output, actual));
                }
            }

        }
        
        private void WriteProcessInput(StreamWriter writer)
        {
            writer.Write(inputBuilder.ToString());
            writer.Flush();
        }

    }
}

