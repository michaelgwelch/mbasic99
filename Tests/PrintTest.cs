using System;
using mbasic;
using NUnit.Framework;

namespace Tests
{
    // Runs print.mbas program and checks its output.
    [TestFixture()]
	public class PrintTest : OutputExpectationTest
    {
        public PrintTest() : base()
        {
            // Scenarios from II-65
            AddExpectedLine(" 10  20 ");
            AddExpectedLine("TI COMPUTER");
            AddExpectedLine("HELLO, FRIEND");
            AddExpectedLine("HIJOAN");
            AddExpectedLine("HI JOAN");
            AddExpectedLine("HELLO JOAN");
            AddExpectedLine(" 10.2 -30.5  16.7 ");
            AddExpectedLine("-20.3 ");
            
            // Number printing scenarios from II-66 of User's Reference Guide
            AddExpectedLine("-10  7.1 ");
            AddExpectedLine(" 9.34277E+10 ");
            AddExpectedLine(" .0000000001 ");
            AddExpectedLine(" 1.2E-10 ");
            AddExpectedLine(" 2.46E-10 ");
            AddExpectedLine(" 15 -3 ");
            AddExpectedLine(" 3.35 -46.1 ");
            AddExpectedLine(" 791.1234568 ");
            AddExpectedLine("-.0127  .64 ");
            AddExpectedLine(" 1.97853E-10 ");
            AddExpectedLine("-9.877E+22 ");
            AddExpectedLine(" 7.364E+12 ");
            AddExpectedLine(" 1.23659E-14 ");
            AddExpectedLine(" 1.25E-09 -4.36E+13 ");
            AddExpectedLine(" 7.6E+**  8.1E-** ");
            
            // Scenarios from II-67
            AddExpectedLine("A");
            AddExpectedLine("");
            AddExpectedLine("B");
            AddExpectedLine("-26 -33 HELLOHOW ARE YOU?");
            AddExpectedLine("-26 ");
            AddExpectedLine("HELLO");
            AddExpectedLine("HOW ARE YOU?");
            AddExpectedLine("ZONE 1        ZONE 2");
            AddExpectedLine("ZONE 1");
            AddExpectedLine("              ZONE 2");
            AddExpectedLine("ZONE 1");
            
            // Scenarios from II-68
            AddExpectedLine("    HELLO");
            AddExpectedLine("    HELLO");
            AddExpectedLine(" 23.5     48.6 ");
            AddExpectedLine("   23.5 ");
            AddExpectedLine("   48.6 ");
            
            AddExpectedLine(" 326           79 ");
            AddExpectedLine(" 326           79 ");
            AddExpectedLine(" 326          ");
            AddExpectedLine(" 79 ");
            AddExpectedLine("     326 ");
            AddExpectedLine("      79 ");
            AddExpectedLine(" 326           79 ");
            
            
            // Other tests
            AddExpectedLine(" 75 ");
            AddExpectedLine("HELLO");
            AddExpectedLine("TO PRINT \"QUOTE MARKS\" YOU M");
            AddExpectedLine("UST USE DOUBLE QUOTES.");
        }
        
        [Test()]
        public void Invoke()
        {
            // Arrange
            var printProgram = "../../../samples/print.mbas";

            // Act
            Program.Main(new[] {printProgram});
            
            // Assert
            AssertMatch();
            
        }
    }
    

}

