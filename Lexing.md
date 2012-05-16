This document will contain dozens of examples pulled from the User's Reference Guide of legal program statements and then how they are broken up into lexemes. This will then aid in the design of the lexer and the parser.

The DATA Statement makes it hard to differentiate at the lexer level between identifiers and strings and so we won't differentiate. This will be left up to the parser to figure out. All non-keyword alpha-numeric strings will be returned as a String lexeme. The parser if it needs an identifier will need to verify the returned string matches the restrictions for an identifier.

The line numbers must be Integers but then it makes the parsing more complicated as numeric expressions must deal with Ints and Floats even though the language has no special support for integers. So both ints and floats will return as Number lexeme. The parser will need to do range checking on labels to make sure they are legal.

    100 PRINT "HELLO"
    Number Keyword QuotedString EOL

    120 LET A=100
    Number Keyword String = Number EOL
    
    130 Let COST=24.95
    Number Keyword String = Number EOL
    
    110 A=27.9
    Number String = Number EOL
    
    120 PRINT A;B
    Number Keyword String ; String EOL
    
    130 END
    Number Keyword EOL
    
    100 PRINT "HI!"
    Number Keyword QuotedString EOL
    
    100 PRINT "TO PRINT ""QUOTE MARKS"" YOU MUST USE DOUBLE QUOTES."
    Number Keyword QuotedString EOL
    
    140 PRINT A*B/2
    Number Keyword String * String / Number EOL
    
    130 PRINT (1E60*1E76)/1E50
    Number Keyword ( Number * Number ) / Number EOL
    
    110 PRINT 24+1E-139
    Number Keyword Number + Number
    
    100 A=2<5
    Number String = Number < Number
    
    110 B=3<=2
    Number String = Number <= Number
    
    100 A$="HI"
    Number String = QuotedString