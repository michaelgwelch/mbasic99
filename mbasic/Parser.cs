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


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using mbasic.SyntaxTree;
namespace mbasic
{
    using mbasic.SyntaxTree;
    using StatementList = System.Collections.Generic.List<Statement>;


    internal class Parser
    {
        public Lexer lexer;
        Token lookahead;
        public Parser(Stream stream, SymbolTable symbols)
        {
            lexer = new Lexer(stream, symbols);
        }

        public Statement Parse()
        {
            lookahead = lexer.Next();
            return Block(Token.EOF);
        }

        public void Match(Token t)
        {
            // Special case
            // 1. If t = Token.NewLine but lookahead = EOF, then just return

            if (lookahead == Token.EOF && t == Token.EndOfLine) return;

            if (lookahead == t) lookahead = lexer.Next();
            else throw new Exception(String.Format("Parsing exception on label {0}", lexer.Label));
        }

        private Statement Statement()
        {
            Statement retVal = null;
            switch (lookahead)
            {
                case Token.Print:
                    retVal = Print();
                    break;
                case Token.For:
                    retVal = ForStatement();
                    break;
                case Token.Let:
                    Match(Token.Let);
                    goto case Token.Variable;
                case Token.Variable:
                    retVal = Assign();
                    break;
                case Token.Input:
                    retVal = Input();
                    break;
                case Token.If:
                    retVal = IfStatement();
                    break;
                case Token.Goto:
                    retVal = Goto();
                    break;
                case Token.Randomize:
                    retVal = Randomize();
                    break;
                case Token.Call:
                    retVal = CallSubroutine();
                    break;
                case Token.End:
                    retVal = EndStatement();
                    break;

            }
            Match(Token.EndOfLine);
            return retVal;
        }

        private Statement EndStatement()
        {
            int line = lexer.LineNumber;
            Match(Token.End);
            return new End(line);
        }

        private Statement CallSubroutine()
        {
            int line = lexer.LineNumber;
            Match(Token.Call);
            string name = lexer.Value;
            Match(Token.Subroutine);
            return new Subroutine(name, line);

        }

        private Statement Randomize()
        {
            int lineNumber = lexer.LineNumber;
            Match(Token.Randomize);
            if (lookahead == Token.Number)
            {
                double seedValue = lexer.NumericValue;
                Match(Token.Number);
                return new Randomize(Convert.ToInt32(Math.Floor(seedValue)), lexer.LineNumber);
            }
            return new Randomize(lineNumber);
        }

        private Statement IfStatement()
        {
            int line = lexer.LineNumber;
            Match(Token.If);
            Expression conditional = Expression();
            Match(Token.Then);
            string label = lexer.Value;
            Match(Token.Number);
            if (lookahead == Token.Else)
            {
                Match(Token.Else);
                string elseLabel = lexer.Value;
                Match(Token.Number);
                return new If(conditional, label, elseLabel, line);
            }
            return new If(conditional, label, line);
        }

        private Statement Goto()
        {
            int line = lexer.LineNumber;
            Match(Token.Goto);
            string label = lexer.Value;
            Match(Token.Number);
            return new Goto(label, line);
        }

        private Statement Input()
        {
            int line = lexer.LineNumber;
            Match(Token.Input);
            int index = lexer.SymbolIndex;
            Match(Token.Variable);
            return new Input(index, line);
        }

        private Statement Assign()
        {
            int line = lexer.LineNumber;
            int index = lexer.SymbolIndex;
            Match(Token.Variable);
            Match(Token.Equals);

            Expression expr = Expression();
            return new Assign(index, expr, line);
        }
        #region Expression Handling

        Expression Expression()
        {
            Expression s = StringExpr();
            return MoreStringExpr(s);
        }

        Expression MoreStringExpr(Expression s1)
        {
            if (lookahead == Token.Concatenate)
            {
                int line = lexer.LineNumber;
                Match(Token.Concatenate);
                Expression s2 = StringExpr();
                return MoreStringExpr(new Concatenate(s1, s2, line));
            }
            else return s1;
        }

        Expression StringExpr()
        {
            Expression r = RelationalExpr();
            return MoreRelationalExpr(r);
        }

        Expression MoreRelationalExpr(Expression r1)
        {
            Expression r2;
            Expression r3=null;
            int line = lexer.LineNumber;

            switch (lookahead)
            {
                case Token.Equals:
                case Token.LessThan:
                case Token.GreaterThan:
                case Token.NotEquals:
                case Token.LessThanEqual:
                case Token.GreaterThanEqual:
                    Token l = lookahead;
                    Match(lookahead);
                    r2 = RelationalExpr();
                    if (l == Token.Equals) r3 = new Equals(r1, r2, line);
                    else if (l == Token.LessThan) r3 = new LessThan(r1, r2, line);
                    else if (l == Token.LessThanEqual) r3 = new LessThanEqual(r1, r2, line);
                    else if (l == Token.GreaterThan) r3 = new Not(new LessThanEqual(r1, r2, line), line);
                    else if (l == Token.GreaterThanEqual) r3 = new Not(new LessThan(r1, r2, line), line);
                    else if (l == Token.NotEquals) r3 = new Not(new Equals(r1, r2, line), line);
                    return MoreRelationalExpr(r3);
                default:
                    break;
            }
            return r1;
        }

        Expression RelationalExpr()
        {
            Expression a = AdditionExpr();
            return MoreAdditionExpr(a);
        }

        Expression MoreAdditionExpr(Expression a1)
        {
            int line = lexer.LineNumber;
            switch (lookahead)
            {
                case Token.Plus:
                case Token.Minus:
                    Token l = lookahead;
                    Match(lookahead);
                    Expression a2 = AdditionExpr();
                    Expression a3=null;
                    if (l == Token.Plus) a3 = new Add(a1, a2, line);
                    if (l == Token.Minus) a3 = new Subtract(a1, a2, line);
                    return MoreAdditionExpr(a3);
                default:
                    break;
            }
            return a1;
        }

        Expression AdditionExpr()
        {
            Expression m = MultiplicationExpr();
            return MoreMultiplicationExpr(m);
        }

        Expression MoreMultiplicationExpr(Expression m1)
        {
            int line = lexer.LineNumber;
            switch (lookahead)
            {
                case Token.Times:
                case Token.Divides:
                    Token l = lookahead;
                    Match(lookahead);
                    Expression m2 = MultiplicationExpr();
                    Expression m3=null;
                    if (l == Token.Times) m3 = new Multiply(m1, m2, line);
                    if (l == Token.Divides) m3 = new Division(m1, m2, line);
                    return MoreMultiplicationExpr(m3);
                default:
                    break;
            }
            return m1;
        }

        Expression MultiplicationExpr()
        {
            Expression f = Factor();
            return MoreFactors(f);
        }

        Expression MoreFactors(Expression f1)
        {
            int line = lexer.LineNumber;
            if (lookahead == Token.Exponent)
            {
                Match(lookahead);
                Expression f2 = Factor();
                Expression f3 = new Power(f1, f2, line);
                return MoreFactors(f3);
            }
            return f1;
        }

        Expression Factor()
        {
            switch (lookahead)
            {
                case Token.LeftParen:
                    Match(Token.LeftParen);
                    Expression e = Expression();
                    Match(Token.RightParen);
                    return e;
                case Token.Function:
                    string name = lexer.Value;
                    Match(Token.Function);
                    Expression[] args = ArgList();
                    return new Function(name, lexer.LineNumber, args);

                case Token.Variable:
                    return VariableReference();
                case Token.Number:
                    return NumberLiteral();
                case Token.String:
                    return StringLiteral();
                case Token.Plus:
                    return UnaryPlus();
                case Token.Minus:
                    return UnaryMinus();
                default: 
                    throw new Exception("No Factor detected");
            }
        }

        Expression[] ArgList()
        {
            if (lookahead == Token.EndOfLine || lookahead == Token.EOF
                || lookahead == Token.RightParen || lookahead == Token.Comma) return new Expression[0];
            List<Expression> exprs = new List<Expression>();
            Match(Token.LeftParen);
            while (lookahead != Token.RightParen)
            {
                exprs.Add(Expression());
                if (lookahead == Token.Comma) Match(Token.Comma);
            }
            Match(Token.RightParen);
            return exprs.ToArray();
        }

        Expression UnaryPlus()
        {
            Match(Token.Plus);
            return Expression();
        }

        Expression UnaryMinus()
        {
            Match(Token.Minus);
            int line = lexer.LineNumber;
            Expression e = Expression();
            return new Negative(e, line);
        }

        Expression StringLiteral()
        {
            StringLiteral literal = new StringLiteral(lexer.Value, lexer.LineNumber);
            Match(Token.String);
            return literal;
        }

        Expression NumberLiteral()
        {
            NumberLiteral literal = new NumberLiteral(lexer.NumericValue, lexer.LineNumber);
            Match(Token.Number);
            return literal;
        }

        Expression VariableReference()
        {
            VariableReference var = new VariableReference(lexer.SymbolIndex, lexer.LineNumber);
            Match(Token.Variable);
            return var;
        }


        private Expression Negative()
        {
            Match(Token.Minus);
            return new Negative(Expression(), lexer.LineNumber);
        }

        #endregion Expression Handling

        private Statement Print()
        {
            int line = lexer.LineNumber;
            Match(Token.Print);
            return new Print(PrintList(), line);
        }

        private Expression PrintList()
        {
            return Expression();
        }


        private void Error() { throw new Exception("Error during parsing"); }

        private Statement ForStatement()
        {
            int line = lexer.LineNumber;
            Match(Token.For);

            int index = lexer.SymbolIndex;
            Match(Token.Variable);

            Match(Token.Equals);

            Expression startVal = Expression();

            Match(Token.To);

            Expression endVal = Expression();

            Match(Token.EndOfLine);

            Block block = Block(Token.Next);

            int endLine = lexer.LineNumber;
            Match(Token.Next);
            Match(Token.Variable);

            Assign init = new Assign(index, startVal, line);
            Assign update = new Assign(index, new Increment(index, line), endLine);
            Expression comparison = new LessThanEqual(new VariableReference(index, line), endVal, line);
            return new For(init, comparison, update, block);
        }

        public Block Block(Token endToken)
        {
            StatementList stmts = new StatementList();
            while (lookahead != endToken)
                stmts.Add(Statement());
            return new Block(stmts);
        }

    }
}
