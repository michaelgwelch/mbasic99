import clr
from System import *

lineNumber = 0
def assertEquals(x):
	global lineNumber
	lineNumber = lineNumber + 1
	input = Console.ReadLine()
	if x.Equals(input): return
	Console.WriteLine("Line {0}: '{1}' != '{2}'",lineNumber,x,input)

assertEquals(" 10  20 ")

assertEquals("TI COMPUTER")

assertEquals("HELLO, FRIEND")

assertEquals("HIJOAN")

assertEquals("HI JOAN")

assertEquals("HELLO JOAN")

assertEquals(" 10.2 -30.5  16.7 ")
assertEquals("-20.3 ")

# Number printing scenarios from II-66 of User's Reference Guide
assertEquals("-10  7.1 ")
assertEquals(" 9.34277E+10 ")
assertEquals(" .0000000001 ")
assertEquals(" 1.2E-10 ")
assertEquals(" 2.46E-10 ")
assertEquals(" 15 -3 ")
assertEquals(" 3.35 -46.1 ")
assertEquals(" 791.1234568 ")
assertEquals("-.0127  .64 ")
assertEquals(" 1.97853E-10 ")
assertEquals("-9.877E+22 ")
assertEquals(" 7.364E+12 ")
assertEquals(" 1.23659E-14 ")
assertEquals(" 1.25E-09 -4.36E+13 ")
assertEquals(" 7.6E+**  8.1E-** ")

# Scenarios from II-67
assertEquals("A")
assertEquals("")
assertEquals("B")
assertEquals("-26 -33 HELLOHOW ARE YOU?")
assertEquals("-26 ")
assertEquals("HELLO")
assertEquals("HOW ARE YOU?")
assertEquals("ZONE 1        ZONE 2")
assertEquals("ZONE 1")
assertEquals("              ZONE 2")
assertEquals("ZONE 1")

# Scenarios from II-68
assertEquals("    HELLO")
assertEquals("    HELLO")
assertEquals(" 23.5     48.6 ")
assertEquals("   23.5 ")
assertEquals("   48.6 ")

assertEquals(" 326           79 ")
assertEquals(" 326           79 ")
assertEquals(" 326          ")
assertEquals(" 79 ")
assertEquals("     326 ")
assertEquals("      79 ")
assertEquals(" 326           79 ")


# Other tests
assertEquals(" 75 ");
assertEquals("HELLO");
Console.WriteLine("done")
