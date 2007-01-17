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
using mbasic.SyntaxTree;

namespace mbasic
{
    class Variable
    {
        string value;
        BasicType dataType; // The TI BASIC data type
        public Variable(string val)
        {
            this.value = val;
            this.dataType = BasicType.Unknown;
        }

        public BasicType BasicType { get { return dataType; } }
        public void SetBasicType(bool isArray)
        {
            if (isArray) throw new ArgumentException("arrays not yet supported");
            if (dataType == BasicType.Unknown)
                dataType = (value.Contains("$") ? BasicType.String : BasicType.Number);
            else
                throw new InvalidOperationException("Data type already set");
        }

        public void SetBasicType()
        {
            SetBasicType(false);
        }

        public void ConstrainType(bool isArray)
        {
            if (dataType == BasicType.Unknown) SetBasicType(isArray);
        }

        public void ConstrainType()
        {
            ConstrainType(false);
        }

        public string Value { get { return value; } }
    }
}
