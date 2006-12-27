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
using System.Reflection.Emit;
using System.Reflection;
using TiBasicRuntime;
namespace mbasic.SyntaxTree
{
    class Function : Expression
    {
        private static readonly Type stringType = typeof(string);
        private static readonly Type numberType = typeof(double);
        private static readonly Type builtinsType = typeof(BuiltIns);
        private static readonly MethodInfo rndMethod =
            builtinsType.GetMethod("Rnd");
        private static readonly MethodInfo ascMethod =
            builtinsType.GetMethod("Asc");
        private static readonly MethodInfo intMethod =
            builtinsType.GetMethod("Int");
        private static readonly MethodInfo chrMethod =
            builtinsType.GetMethod("Chr");
        private static readonly MethodInfo lenMethod =
            builtinsType.GetMethod("Len");
        private static readonly MethodInfo posMethod =
            builtinsType.GetMethod("Pos");
        private static readonly MethodInfo segMethod =
            builtinsType.GetMethod("Seg");
        private static readonly MethodInfo strMethod =
            builtinsType.GetMethod("Str");
        private static readonly MethodInfo valMethod =
            builtinsType.GetMethod("Val");

        // Numeric Functions (resort to BuiltIns only when a suitable
        // 1 line IL instructions doesn't already exist in framework)
        private static readonly Type mathType = typeof(Math);
        private static readonly Type[] oneNumber = new Type[] { numberType };
        private static readonly MethodInfo absMethod =
            mathType.GetMethod("Abs", oneNumber);
        private static readonly MethodInfo atnMethod =
            mathType.GetMethod("Atan");
        private static readonly MethodInfo cosMethod =
            mathType.GetMethod("Cos");
        private static readonly MethodInfo expMethod =
            mathType.GetMethod("Exp");
        private static readonly MethodInfo logMethod =
            builtinsType.GetMethod("Log");
        private static readonly MethodInfo sgnMethod =
            mathType.GetMethod("Sign", oneNumber);
        private static readonly MethodInfo sinMethod =
            mathType.GetMethod("Sin");
        private static readonly MethodInfo sqrMethod =
            builtinsType.GetMethod("Sqr");
        private static readonly MethodInfo tanMethod =
            mathType.GetMethod("Tan");


            

        string functionName;
        Expression[] exprs;
        BasicType[] argsTypes;
        public Function(string name, LineId line, params Expression[] expressions)
            : base(line)
        {
            this.functionName = name;
            this.exprs = expressions; 
            argsTypes = new BasicType[exprs.Length];
        }

        public override void Emit(ILGenerator gen)
        {
            switch (functionName)
            {
                case "RND":
                    EmitFunctionCall(gen, rndMethod);
                    break;
                case "INT":
                    EmitFunctionCall(gen, intMethod, exprs[0]);
                    break;
                case "ASC":
                    EmitFunctionCall(gen, ascMethod, line.Label, exprs[0]);
                    break;
                case "CHR$":
                    EmitFunctionCall(gen, chrMethod, line.Label, exprs[0]);
                    break;
                case "LEN":
                    EmitFunctionCall(gen, lenMethod, exprs[0]);
                    break;
                case "POS":
                    EmitFunctionCall(gen, posMethod, line.Label, exprs[0], exprs[1], exprs[2]);
                    break;
                case "SEG$":
                    EmitFunctionCall(gen, segMethod, line.Label, exprs[0], exprs[1], exprs[2]);
                    break;
                case "STR$":
                    EmitFunctionCall(gen, strMethod, exprs[0]);
                    break;
                case "VAL":
                    EmitFunctionCall(gen, valMethod, line.Label, exprs[0]);
                    break;

                    // Numeric Functions
                case "ABS":
                    EmitFunctionCall(gen, absMethod, exprs[0]);
                    break;
                case "ATN":
                    EmitFunctionCall(gen, atnMethod, exprs[0]);
                    break;
                case "COS":
                    EmitFunctionCall(gen, cosMethod, exprs[0]);
                    break;
                case "EXP":
                    EmitFunctionCall(gen, expMethod, exprs[0]);
                    break;
                case "LOG":
                    EmitFunctionCall(gen, logMethod, line.Label, exprs[0]);
                    break;
                case "SGN":
                    EmitFunctionCall(gen, sgnMethod, exprs[0]);
                    gen.Emit(OpCodes.Conv_R8); // Sign returns an int
                    break;
                case "SIN":
                    EmitFunctionCall(gen, sinMethod, exprs[0]);
                    break;
                case "SQR":
                    EmitFunctionCall(gen, sqrMethod, line.Label, exprs[0]);
                    break;
                case "TAN":
                    EmitFunctionCall(gen, tanMethod, exprs[0]);
                    break;

            }
        }

        public override BasicType GetBasicType()
        {
            for (int i = 0; i < exprs.Length; i++)
            {
                argsTypes[i] = exprs[i].GetBasicType();
            }
            switch (functionName)
            {
                case "RND":
                    return BasicType.Number;
                case "ASC":
                    if (exprs.Length != 1) return BasicType.Error;
                    if (argsTypes[0] != BasicType.String) return BasicType.Error;
                    return BasicType.Number;
                case "CHR$":
                    if (exprs.Length != 1) return BasicType.Error;
                    if (argsTypes[0] != BasicType.Number) return BasicType.Error;
                    return BasicType.String;
                case "LEN":
                    if (exprs.Length != 1) return BasicType.Error;
                    if (argsTypes[0] != BasicType.String) return BasicType.Error;
                    return BasicType.Number;
                case "POS":
                    if (exprs.Length != 3) return BasicType.Error;
                    if (argsTypes[0] != BasicType.String) return BasicType.Error;
                    if (argsTypes[1] != BasicType.String) return BasicType.Error;
                    if (argsTypes[2] != BasicType.Number) return BasicType.Error;
                    return BasicType.Number;
                case "SEG$":
                    if (exprs.Length != 3) return BasicType.Error;
                    if (argsTypes[0] != BasicType.String) return BasicType.Error;
                    if (argsTypes[1] != BasicType.Number) return BasicType.Error;
                    if (argsTypes[2] != BasicType.Number) return BasicType.Error;
                    return BasicType.String;
                case "STR$":
                    if (exprs.Length != 1) return BasicType.Error;
                    if (argsTypes[0] != BasicType.Number) return BasicType.Error;
                    return BasicType.String;
                case "VAL":
                    if (exprs.Length != 1) return BasicType.Error;
                    if (argsTypes[0] != BasicType.String) return BasicType.Error;
                    return BasicType.Number;
                  
                    // Numeric Functions
                case "ABS":
                case "ATN":
                case "COS":
                case "EXP":
                case "INT":
                case "LOG":
                case "SGN":
                case "SIN":
                case "SQR":
                case "TAN":
                    if (exprs.Length != 1) return BasicType.Error;
                    if (argsTypes[0] != BasicType.Number) return BasicType.Error;
                    return BasicType.Number;
                default:
                    return BasicType.Error;
            }
        }

        private void EmitFunctionCall(ILGenerator gen,
            MethodInfo method, params Expression[] exprs)
        {
            foreach (Expression expr in exprs)
                expr.Emit(gen);

            gen.Emit(OpCodes.Call, method);
        }

        private void EmitFunctionCall(ILGenerator gen,
            MethodInfo method, string label, params Expression[] exprs)
        {
            gen.Emit(OpCodes.Ldstr, label);
            EmitFunctionCall(gen, method, exprs);
        }


    }


}
