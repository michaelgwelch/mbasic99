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

namespace mbasic
{
    using VariableList = System.Collections.Generic.List<Variable>;
    using KeyWordList = System.Collections.Generic.SortedList<string, Token>;
    using mbasic.SyntaxTree;
    internal class SymbolTable 
    {
        VariableList variables = new VariableList();
        KeyWordList keyWords = new KeyWordList();
        

        /// <summary>
        /// Inserts a variable into symbol table.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public int Insert(string name)
        {
            variables.Add(new Variable(name));
            return variables.Count - 1;
        }

        public int Lookup(string s)
        {
            return variables.FindIndex((new FindByVarNamePredicate(s)).Match);
        }

        public int Count { get { return variables.Count; } }

        public Variable this[int index]
        {
            get
            {
                return variables[index];
            }
        }

        public IEnumerable<Variable> Variables
        {
            get
            {
                foreach (Variable v in variables) yield return v;
            }
        }
        #region Key Words
        public void ReserveWord(string keyWord, Token t)
        {
            keyWords.Add(keyWord, t);
        }

        public bool ContainsKeyWord(string keyWord)
        {
            return keyWords.ContainsKey(keyWord);
        }

        public Token GetKeyWordToken(string keyWord)
        {
            return keyWords[keyWord];
        }

        #endregion
        private class FindByVarNamePredicate
        {
            string var;
            public FindByVarNamePredicate(string varName)
            {
                this.var = varName;
            }

            public bool Match(Variable v)
            {
                if (v.Value == var) return true;
                else return false;
            }
        }
        
    }
}
