import clr
from System import *

lineNumber = 0
def assertEquals(x):
	global lineNumber
	lineNumber = lineNumber + 1
	input = Console.ReadLine()
	if x.Equals(input): return
	Console.WriteLine("Line {0}: '{1}' != '{2}'",lineNumber,x,input)
# Page II-61
assertEquals(" 22  15 ")
assertEquals(" 36  52 ")
assertEquals(" 48  96.5 ")

assertEquals(" 2  4  6  8  10  12  14  16 ")
assertEquals(" 12  14  16  18  20  22  24 ")
assertEquals(" 26 ")
Console.WriteLine("done")

