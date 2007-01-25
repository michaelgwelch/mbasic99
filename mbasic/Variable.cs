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
        int[] dimensions; // only used for arrays
        BasicType dataType; // The TI BASIC data type
        Type clrArrayType;

        public Variable(string name) 
        {
            this.name = name;
            this.dimensions = null;
            dataType = BasicType.Unknown;
        }

        public BasicType BasicType { get { return dataType; } }

        public void SetBasicType(bool isArray, int numDimensions)
        {
            if (dataType != BasicType.Unknown) throw new InvalidOperationException("Data type already set");
            if (dimensions != null && !isArray) throw new InvalidOperationException("Array variable being used as a scalar");

            if (isArray)
            {
                dataType = (name.Contains("$") ? BasicType.StringArray : BasicType.NumberArray);
                if (dimensions == null)
                {
                    dimensions = new int[numDimensions];
                    for (int i = 0; i < numDimensions; i++) dimensions[i] = 10;
                }
            }
            else dataType = (name.Contains("$") ? BasicType.String : BasicType.Number);
                
        }

        public void SetBasicType()
        {
            SetBasicType(false, -1);
        }

        public void ConstrainType(bool isArray, int numDimensions)
        {
            if (dataType == BasicType.Unknown) SetBasicType(isArray, numDimensions);
        }

        public void ConstrainType()
        {
            ConstrainType(false, -1);
        }

        public string Value { get { return name; } }

        public void Dimension(params int[] dims)
        {
            if (this.dimensions != null) throw new Exception(String.Format("Array variable {0} used before it was DIMensioned", name));
            this.dimensions = dims;
            ConstrainType(true, dims.Length);
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
                    clrArrayType = Type.GetType("System.Double[" + new String(',', dimensions.Length - 1) + "]");
                    local = gen.DeclareLocal(clrArrayType);
                    break;
                case BasicType.String:
                    local = gen.DeclareLocal(typeof(string));
                    break;
                case BasicType.StringArray:
                    clrArrayType = Type.GetType("System.String[" + new String(',', dimensions.Length - 1) + "]");
                    local = gen.DeclareLocal(clrArrayType);
                    break;
                default:
                    throw new InvalidOperationException("type not defined for variable");
            }
            return local;
        }

        public void EmitDefaultValue(ILGenerator gen, LocalBuilder local)
        {
            if (dimensions != null)
            {
                gen.Emit(OpCodes.Ldc_I4, dimensions.Length);
                gen.Emit(OpCodes.Newarr, typeof(int));
                for (int i = 0; i < dimensions.Length; i++)
                {
                    gen.Emit(OpCodes.Dup);
                    gen.Emit(OpCodes.Ldc_I4, i);
                    gen.Emit(OpCodes.Ldc_I4, dimensions[i]);
                    gen.Emit(OpCodes.Stelem_I4);
                }
                if (dataType == BasicType.NumberArray) BuiltInsMethodCall.CreateNumberArray().Emit(gen);
                else BuiltInsMethodCall.CreateStringArray().Emit(gen);
                gen.Emit(OpCodes.Castclass, clrArrayType);
            }
            else
            {
                switch (dataType)
                {
                    case BasicType.String:
                        gen.Emit(OpCodes.Ldstr, "");
                        break;
                }
            }
            if (dataType != BasicType.Number) gen.Emit(OpCodes.Stloc, local);
        }

    }
}
