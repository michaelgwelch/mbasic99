/*******************************************************************************
    Copyright 2006 Michael Welch
    
    This file is part of MBasic99.

    MBasic99 is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    MBasic99 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MBasic99; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*******************************************************************************/


internal enum Token 
{
    Colon       = ':',
    Comma       = ',',
    Exponent    = '^',
    LessThan    = '<',
    GreaterThan = '>',
    Concatenate = '&',
    Equals      = '=',
    LeftParen   = '(',
    RightParen  = ')',
    Plus        = '+',
    Minus       = '-',
    Times       = '*',
    Divides     = '/',
    Semicolon   = ';',
    And         = 256,
    Base,
    Call,
    Data,
    Dim,
    Else,
    End, // used for keywords END and STOP
    EndOfLine,
    EOF, // end of file
    Error,
    Float,
    For,
    Function,
    Go, // As part of GO SUB 
    Goto,
    Gosub,
    GreaterThanEqual,
    If,
    Input,
    LessThanEqual,
    Let,
    Next,
    Not,
    NotEquals,
    Number,
    Or,
    Option,
    Print,
    Randomize,
    Read,
    Remark, // technically not a token, and should never be returned. Used internally by lexer only.
    Restore,
    Return,
    String,
    Sub,        // The key word "SUB"
    Subroutine, // A built in subroutine like clear, or print.
    Tab,
    Then,
    To,
    Variable

}