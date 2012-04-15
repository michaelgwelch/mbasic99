using System;
using NUnit.Framework;

namespace mbasic
{
    [TestFixture()]
	public class SecretTest
    {
        [Test()]
        public void TestCase()
        {
            InteractiveProgramRunner runner = new InteractiveProgramRunner("../../../bin/mbasic.exe",
                                                                           "../../../samples/secret.mbas");
            runner.ExpectOuput("ENTER LIMIT: ");
            runner.AddInputLine("100");
            runner.ExpectOutput("GUESS: ");
            runner.AddInputLine("50");
            
            int guess = 50;
            int high = 100;
            int low = 0;
            runner.Acion("SECRET NUMBER IS LESS THAN\nYOUR NUMBER\nGUESS: ", 
                        () => 
                        { 
                            int range = guess - low;
                            if (range == 0) throw new Exception("low equals high. What?");
                            if (range == 1) throw new Exception("no numbers between low and high. What?");
                            int newGuess = low + (range / 2);
                            high = guess;
                            guess = newGuess;
                            runner.AddInputLine(guess.ToString());
                        });
            
            runner.Acion("SECRET NUMBER IS LARGER THAN\nYOUR NUMBER\nGUESS: ", 
                        () => 
                        { 
                            int range = high - guess;
                            if (range == 0) throw new Exception("low equals high. What?");
                            if (range == 1) throw new Exception("no numbers between low and high. What?");
                            int newGuess = guess + (range / 2);
                            low = guess;
                            guess = newGuess;
                            runner.AddInputLine(guess.ToString());
                        });
            
            runner.Action("YOU GUESSED THE SECRET NUMBER IN 
            
        }
    }
}

