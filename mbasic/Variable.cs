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
using System.Reflection.Emit;
using TiBasicRuntime;

namespace mbasic
{
    class Variable
    {
        string name;
        int size; // only used for arrays
        BasicType dataType; // The TI BASIC data type

        public Variable(string name) 
        {
            this.name = name;
            this.size = -1;
            dataType = BasicType.Unknown;
        }

        public BasicType BasicType { get { return dataType; } }

        public void SetBasicType(bool isArray)
        {
            if (dataType != BasicType.Unknown) throw new InvalidOperationException("Data type already set");
            if (size != -1 && !isArray) throw new InvalidOperationException("Array variable being used as a scalar");

            if (isArray)
            {
                dataType = (name.Contains("$") ? BasicType.StringArray : BasicType.NumberArray);
                if (size == -1) size = 10;
            }
            else dataType = (name.Contains("$") ? BasicType.String : BasicType.Number);
                
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

        public string Value { get { return name; } }

        public void Dimension(int size)
        {
            if (this.size != -1) throw new Exception(String.Format("Array variable {0} used before it was DIMensioned", name));
            this.size = size;
        }

        public LocalBuilder EmitDeclare(ILGenerator gen)
        {
            LocalBuilder local;
            switch (dataType)
            {
                case BasicType.Number:
                    local = gen.DeclareLocal(typeof(double));
                    break;
                case BasicType.NumberArray:
                    local = gen.DeclareLocal(typeof(double[]));
                    break;
                case BasicType.String:
                    local = gen.DeclareLocal(typeof(string));
                    break;
                case BasicType.StringArray:
                    local = gen.DeclareLocal(typeof(string[]));
                    break;
                default:
                    throw new InvalidOperationException("type not defined for variable");
            }
            return local;
        }

        public void EmitDefaultValue(ILGenerator gen, LocalBuilder local)
        {
            switch (dataType)
            {
                case BasicType.Number:
                    gen.Emit(OpCodes.Ldc_R8, 0.0);
                    break;
                case BasicType.NumberArray:
                    gen.Emit(OpCodes.Ldc_I4, size+1);
                    gen.Emit(OpCodes.Newarr, typeof(double));
                    break;
                case BasicType.String:
                    gen.Emit(OpCodes.Ldstr, "");
                    break;
                case BasicType.StringArray:
                    gen.Emit(OpCodes.Ldc_I4, size);
                    BuiltInsMethodCall.CreateStringArray().Emit(gen);
                    break;
            }
            gen.Emit(OpCodes.Stloc, local);
        }

    }
}
