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
        SortedList<string, object[]> data;
        Token lookahead;
        public Parser(Stream stream, SymbolTable symbols, SortedList<string, object[]> data)
        {
            lexer = new Lexer(stream, symbols);
            this.data = data;
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
            else throw new Exception(String.Format("Parsing exception on label {0}, column {1}"
                , lexer.LineId.Label, lexer.Column));
        }

        private Statement Statement()
        {
            Statement retVal = null;
            switch (lookahead)
            {
                case Token.Remark:
                    retVal = RemarkStatement();
                    break;
                case Token.Print:
                    retVal = PrintStatement();
                    break;
                case Token.For:
                    retVal = ForStatement();
                    break;
                case Token.Let:
                    Match(Token.Let);
                    goto case Token.Variable;
                case Token.Variable:
                    retVal = AssignStatement();
                    break;
                case Token.Input:
                    retVal = InputStatement();
                    break;
                case Token.If:
                    retVal = IfStatement();
                    break;
                case Token.Goto:
                    retVal = GotoStatement();
                    break;
                case Token.Randomize:
                    retVal = RandomizeStatement();
                    break;
                case Token.Call:
                    retVal = CallSubroutine();
                    break;
                case Token.End:
                    retVal = EndStatement();
                    break;
                case Token.Data:
                    retVal = DataStatement();
                    break;
                case Token.Read:
                    retVal = ReadStatement();
                    break;
                case Token.Restore:
                    retVal = RestoreStatement();
                    break;
                case Token.Go:
                    retVal = GoStatement();
                    break;
                case Token.Gosub:
                    retVal = GosubStatement();
                    break;
                case Token.Return:
                    retVal = ReturnStatement();
                    break;
                case Token.Dim:
                    retVal = DimStatement();
                    break;
                case Token.Option:
                    retVal = OptionBaseStatement();
                    break;
                case Token.On:
                    retVal = OnGotoStatement();
                    break;

            }
            Match(Token.EndOfLine);
            return retVal;
        }

        private Statement OnGotoStatement()
        {
            LineId line = lexer.LineId;
            Match(Token.On);
            Expression expr = Expression();
            Match(Token.Goto);

            List<string> labelNumbers = new List<string>();
            labelNumbers.Add(lexer.Value);
            Match(Token.Number);
            while (lookahead == Token.Comma)
            {
                Match(Token.Comma);
                labelNumbers.Add(lexer.Value);
                Match(Token.Number);
            }
            return new OnGoto(expr, labelNumbers, line);
        }

        private Statement OptionBaseStatement()
        {
            Match(Token.Option);
            Match(Token.Base);
            int optionBase = (int)lexer.NumericValue;
            if (optionBase != 0 && optionBase != 1) throw new Exception("Invalid Option Base");
            Match(Token.Number);
            return new OptionBaseStatement(optionBase);
        }

        private Statement DimStatement()
        {
            List<Statement> decls = new List<Statement>();

            Match(Token.Dim);

            decls.Add(ArrayDeclaration());
            while (lookahead == Token.Comma)
            {
                Match(Token.Comma);
                decls.Add(ArrayDeclaration());
            }

            return new Block(decls);
        }

        private ArrayDeclaration ArrayDeclaration()
        {
            List<int> dimensions = new List<int>();
            int index = lexer.SymbolIndex;
            Match(Token.Variable);
            Match(Token.LeftParen);
            dimensions.Add((int)lexer.NumericValue);
            Match(Token.Number);
            while (lookahead == Token.Comma)
            {
                Match(Token.Comma);
                dimensions.Add((int)lexer.NumericValue);
                Match(Token.Number);
            }
            Match(Token.RightParen);
            

            return new ArrayDeclaration(index, dimensions.ToArray());
        }

        private Statement RemarkStatement()
        {
            LineId line = lexer.LineId;
            Statement remark = new Remark(line);
            Match(Token.Remark);
            return remark;
        }

        private Statement ReturnStatement()
        {
            LineId line = lexer.LineId;
            Statement returnStatement = new Return(line);
            Match(Token.Return);
            return returnStatement;
        }

        private Statement GoStatement()
        {
            Match(Token.Go);
            Match(Token.Sub);
            return GosubStatementInternal();
        }

        private Statement GosubStatement()
        {
            Match(Token.Gosub);
            return GosubStatementInternal();
        }

        private Statement GosubStatementInternal()
        {
            string destLabel = lexer.Value;
            LineId line = lexer.LineId;
            Statement gosub = new Gosub(destLabel, line);
            Match(Token.Number);
            return gosub;
        }

        private Statement RestoreStatement()
        {
            LineId line = lexer.LineId;
            Restore restore;
            Match(Token.Restore);
            if (lookahead == Token.Number)
            {
                restore = new Restore(lexer.Value, line);
                Match(Token.Number);
            }
            else
            {
                restore = new Restore(line);
            }
            return restore;
        }

        private Statement ReadStatement()
        {
            Match(Token.Read);
            LineId line = lexer.LineId;
            List<Statement> reads = new List<Statement>();
            while (lookahead != Token.EOF && lookahead != Token.EndOfLine)
            {
                Location loc = Location();
                reads.Add(Assign.ReadData(loc, line));
                if (lookahead == Token.Comma) Match(Token.Comma);
            }
            return new Read(new Block(reads), line);
        }

        private Statement DataStatement()
        {
            Match(Token.Data);
            KeyValuePair<string, object[]> list = DataList();
            data.Add(list.Key, list.Value);
            return Data.Instance;
        }

        private Statement EndStatement()
        {
            LineId line = lexer.LineId;
            Match(Token.End);
            return new End(line);
        }

        private Statement CallSubroutine()
        {
            LineId line = lexer.LineId;
            Match(Token.Call);
            string name = lexer.Value;
            Match(Token.Subroutine);
            return new Subroutine(name, line);

        }

        private Statement RandomizeStatement()
        {
            LineId line = lexer.LineId;
            Match(Token.Randomize);

            if (lookahead == Token.EndOfLine || lookahead == Token.EOF) return new Randomize(line);

            else return new Randomize(Expression(), line);
        }

        private Statement IfStatement()
        {
            LineId line = lexer.LineId;
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

        private Statement GotoStatement()
        {
            LineId line = lexer.LineId;
            Match(Token.Goto);
            string label = lexer.Value;
            Match(Token.Number);
            return new Goto(label, line);
        }

        private Statement InputStatement()
        {
            // Requires more than one token of lookahead. If we encounter a string variable
            // after input, it could be an input prompt or the variable to store the input in.
            // It's not until we look past that and check for a colon do we know.

            LineId line = lexer.LineId;
            Match(Token.Input);
            List<Statement> inputs = new List<Statement>();
            Expression inputPrompt;

            // Assume that an input prompt is given and parse it as an expression
            // If it later turns out that it is not the input prompt, then it must
            // be convert to an assignment.

            Expression expr = Expression();

            if (lookahead == Token.Colon)
            {
                inputPrompt = expr;
                Match(Token.Colon);
            }
            else // if there is no colon then what we read shouldn't be
                // treated as an Expression, but as an Assignment.
            {
                inputPrompt = new StringLiteral("? ", LineId.None);
                LocationReference vr = (LocationReference)expr; // The expr must have been a Location Reference
                Location location = vr.Location;
                inputs.Add(Assign.ReadConsole(location, line));
                if (lookahead == Token.Comma) Match(Token.Comma);
            }


            while (lookahead != Token.EOF && lookahead != Token.EndOfLine)
            {
                Location location = Location();
                inputs.Add(Assign.ReadConsole(location, line));
                if (lookahead == Token.Comma) Match(Token.Comma);
            }
            return new Input(inputPrompt, new Block(inputs), line);

        }

        private Statement AssignStatement()
        {
            LineId line = lexer.LineId;
            Location loc = Location();

            Match(Token.Equals);

            Expression expr = Expression();
            return new Assign(loc, expr, line);
        }

        private Location Location()
        {
            int index = lexer.SymbolIndex;
            VariableLocation location = new VariableLocation(index);
            Match(Token.Variable);
            if (lookahead == Token.LeftParen)
            {
                List<Expression> exprs = new List<Expression>();
                Match(Token.LeftParen);
                exprs.Add(Expression());
                while (lookahead == Token.Comma)
                {
                    Match(Token.Comma);
                    exprs.Add(Expression());
                }
                Match(Token.RightParen);
                return new ArrayElement(location, exprs.ToArray());
            }
            else return location;
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
                LineId line = lexer.LineId;
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
            LineId line = lexer.LineId;

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
                    if (l == Token.Equals) r3 = RelationalExpression.CompareEquals(r1, r2, line);
                    else if (l == Token.LessThan) r3 = RelationalExpression.CompareLessThan(r1, r2, line);
                    else if (l == Token.LessThanEqual) r3 = RelationalExpression.CompareLessThanEquals(r1, r2, line);
                    else if (l == Token.GreaterThan) r3 = RelationalExpression.CompareGreaterThan(r1, r2, line);
                    else if (l == Token.GreaterThanEqual) r3 = RelationalExpression.CompareGreaterThanEquals(r1, r2, line);
                    else if (l == Token.NotEquals) r3 = RelationalExpression.CompareNotEquals(r1, r2, line);
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
            LineId line = lexer.LineId;
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
            LineId line = lexer.LineId;
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
            LineId line = lexer.LineId;
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
                    return new Function(name, lexer.LineId, args);

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
                    throw new Exception(
                        String.Format("No Factor detected label {0}, column {1}",
                        lexer.LineId.Label, lexer.Column));
            }
        }

        
        KeyValuePair<string, object[]> DataList()
        {
            string label = lexer.LineId.Label;

            if (lookahead == Token.EndOfLine || lookahead == Token.EOF)
                new KeyValuePair<string, object[]>(label, new object[0]);
            List<object> items = new List<object>();
            while (lookahead != Token.EndOfLine && lookahead != Token.EOF)
            {
                switch(lookahead)
                {
                    case Token.Number:
                        items.Add(lexer.NumericValue);
                        Match(Token.Number);
                        break;
                    case Token.String:
                        items.Add(lexer.Value);
                        Match(Token.String);
                        break;
                    case Token.Comma:
                        items.Add(String.Empty);
                        break;
                    default:
                        throw new Exception(String.Format(
                            "DATA ERROR ON {0}", lexer.LineId.Label));
                }
                if (lookahead == Token.Comma) Match(Token.Comma);
            }
            return new KeyValuePair<string,object[]>(label, items.ToArray());
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
            LineId line = lexer.LineId;
            Expression e = Expression();
            return new Negative(e, line);
        }

        Expression StringLiteral()
        {
            StringLiteral literal = new StringLiteral(lexer.Value, lexer.LineId);
            Match(Token.String);
            return literal;
        }

        Expression NumberLiteral()
        {
            NumberLiteral literal = new NumberLiteral(lexer.NumericValue, lexer.LineId);
            Match(Token.Number);
            return literal;
        }

        Expression VariableReference()
        {
            LineId line = lexer.LineId;
            Location location = Location();
            LocationReference var = new LocationReference(location, line);
            return var;
        }

        #endregion Expression Handling

        private Statement PrintStatement()
        {
            LineId line = lexer.LineId;
            Match(Token.Print);
            return new Print(PrintList(), line);
        }

        private Expression[] PrintList()
        {
            List<Expression> list = new List<Expression>();

            switch (lookahead)
            {
                case Token.Plus:
                case Token.Minus:
                case Token.Function:
                case Token.LeftParen:
                case Token.Number:
                case Token.String:
                case Token.Variable:
                    list.Add(Expression());
                    list.AddRange(MorePrintList());
                    break;
                case Token.Comma:
                case Token.Semicolon:
                case Token.Colon:
                    list.AddRange(MorePrintList());
                    break;
                case Token.Tab:
                    Match(Token.Tab);
                    Match(Token.LeftParen);
                    list.Add(new Tab(Expression(), lexer.LineId));
                    Match(Token.RightParen);
                    list.AddRange(MorePrintList());
                    break;
                case Token.EndOfLine:
                case Token.EOF:
                    break;
                default:
                    throw new Exception("Error parsing print list on " + lexer.LineId.Label);
            }
            
            return list.ToArray();
        }

        private Expression[] MorePrintList()
        {
            List<Expression> list = new List<Expression>();
            switch (lookahead)
            {
                case Token.Semicolon:
                    list.Add(new StringLiteral("\0", lexer.LineId));
                    Match(Token.Semicolon);
                    break;
                case Token.Colon:
                    list.Add(new StringLiteral("\n", lexer.LineId));
                    Match(Token.Colon);
                    break;
                case Token.Comma:
                    list.Add(new StringLiteral("\t", lexer.LineId));
                    Match(Token.Comma);
                    break;
                case Token.EndOfLine:
                case Token.EOF:
                    return new Expression[0];
                default:
                    throw new Exception(String.Format(
                        "Missing print seperator on {0}", lexer.LineId.Label));
            }
            list.AddRange(PrintList());
            return list.ToArray();

        }




        private Statement ForStatement()
        {
            LineId line = lexer.LineId; // This is the line that the FOR key word is used on
            Match(Token.For);
			
			//TODO: Must make sure that we only use variable (not array expression) in For Loop
            Location location = Location(); // this is the counting variable

            Match(Token.Equals);

            Expression startVal = Expression(); // This is the starting value

            Match(Token.To);

            Expression endVal = Expression(); // this is the ending value

            Expression stepVal = new NumberLiteral(1.0, lexer.LineId);
            if (lookahead == Token.Step)
            {
                Match(Token.Step);
                stepVal = Expression();
            }
            

            Match(Token.EndOfLine);

            Block block = Block(Token.Next);

            LineId endLine = lexer.LineId;  // This is the line that the NEXT keyword is used on
            Match(Token.Next);
            Match(Token.Variable);

            Assign init = new Assign(location, startVal, line);
            Assign update = new Assign(location, new Add(new LocationReference(location, endLine),
                stepVal, endLine), endLine);
            Expression comparison = RelationalExpression.CompareLessThanEquals(new LocationReference(location, line), endVal, line);
            return new For(init, comparison, update, block, line);
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
