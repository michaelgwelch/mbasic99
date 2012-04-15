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
	public class CheckParensTest : DeterministicProgramTest
    {
        [Test]
        public void TestCase()
        {
            // Arrange
            DeterministicProgramRunner runner = new DeterministicProgramRunner();
            runner.ExpectOutput("String: ");
            runner.AddInputLine("{([])}");
            runner.ExpectOutput("Match\n");
            runner.ExpectOutput("Another String (Y or N): ");
            runner.AddInputLine("N");
            
            // Act
            runner.Run("../../../bin/mbasic.exe", "../../../samples/checkparens.mbas");
            
            // Assert
            // Nothing to do, runner throws an exception if output doesn't match.
            
        }
        

    }
    

}

