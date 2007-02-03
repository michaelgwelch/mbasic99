#!/usr/bin/env python
lineNumber = 0
def assertEquals(x):
	global lineNumber
	lineNumber = lineNumber + 1
	input = raw_input()
	if (x == input): return
	print "Line %d: expected:'%s' != '%s'" % (lineNumber,x,input)

