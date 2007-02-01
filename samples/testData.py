#!/usr/bin/env python
from testCore import assertEquals
lineNumber = 0
def assertEquals(x):
	global lineNumber
	lineNumber = lineNumber + 1
	input = raw_input()
	if (x == input): return
	print "Line %d: '%s' != '%s'" % (lineNumber,x,input)
# Page II-63
assertEquals(" 2  4 ")
assertEquals(" 6  7 ")
assertEquals(" 8  1 ")
assertEquals(" 2  3 ")
assertEquals(" 4  5 ")
assertEquals("HELLO")
assertEquals("JONES, MARY")
assertEquals(" 28 ")
assertEquals(" 3.1416 ")
assertEquals("A$ IS HI")
assertEquals("B$ IS ")
assertEquals("C IS  2 ")
assertEquals("A$ IS ")
assertEquals("this is the first string.")
assertEquals("B$ IS .")
assertEquals("C$ IS .")
assertEquals("D IS  5 .")
