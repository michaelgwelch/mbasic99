This project aims to create a complete compiler for the TI BASIC programming
language. TI BASIC was the dialect of BASIC created for the TI-99/4A.

Current Status
==============
The project is currently in pre-alpha. (Updated 1/20/2007)

Simple programs that do not use any of the graphic or sound subroutines can be
parsed and compiled.

Completed
=========
* LET (assignment) statements
* REM comments
* END/STOP
* GOTO and ON-GOTO
* GOSUB/RETURN
* IF-THEN-ELSE
* FOR-TO (STEP is not implemented. Counting is by 1)
* NEXT
* INPUT (from Console)
* READ
* DATA
* RESTORE
* PRINT/DISPLAY (to Console)
* CALL CLEAR
* Multi-dimensional arrays
* Numeric Functions (ABS, ATN, COS, EXP, INT, LOG, RANDOMIZE, RND, SGN, SIN,
 SQR, TAN)
* String Functions (ASC, CHR$, LEN, POS, SEG$, STR$, VAL)
* DIM
* Arithmetic Operators: ^, +, -, *, /
* Relational Operators: <, <=, >, >=, =, <>
* String Operator: &
* Numeric Constants (including Scientific Notation)
* String Constants

# Not Implemented
* File I/O (OPEN, INPUT, PRINT, CLOSE, EOF, RESTORE)
* Option Base
* CALL COLOR
* CALL SCREEN
* CALL CHAR
* CALL HCHAR
* CALL VCHAR
* CALL SOUND
* CALL GCHAR
* CALL KEY
* CALL JOYST
* DEF (user defined functions)
* Interpreter commands (NEW, LIST, RUN, BYE, NUMBER, RESEQUENCE, etc.) These
 will not be implemented, as they only make sense in an interactive mode.
