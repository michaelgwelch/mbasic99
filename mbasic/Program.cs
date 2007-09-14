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
using System.Reflection.Emit;
using System.Reflection;
using System.Diagnostics.SymbolStore;
using System.Diagnostics;


namespace mbasic
{
    using LabelList = System.Collections.Generic.SortedList<string, Label>;
    using TIBasicRuntime;
    using File = System.IO.File;
    class Program
    {
        static readonly MethodInfo popMethod =
            typeof(BuiltIns).GetMethod("PopReturnAddress");

        static void Main(string[] args)
        {
            bool debug = true;
            bool runit = false;

            string fileName = args[0];
            string assemblyName = Path.GetFileNameWithoutExtension(fileName);
            string moduleName = assemblyName + ".exe";
            string exeName = assemblyName + ".exe";

            fileName = Path.GetFullPath(fileName);
            Stream stream = File.OpenRead(fileName);
            SymbolTable symbols = new SymbolTable();
            InitializeReservedWords(symbols);
            SortedList<string, object[]> data = new SortedList<string, object[]>(); // a list used to contain all the statid DATA data.
            Parser parser = new Parser(stream, symbols, data);
            Node.lexer = parser.lexer;
            Node.symbols = symbols;

            Statement n = parser.Parse();

            AppDomain domain = AppDomain.CurrentDomain;
            AssemblyName name = new AssemblyName(assemblyName);
            AssemblyBuilder bldr = domain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);

            if (debug)
            {
                Type daType = typeof(DebuggableAttribute);
                ConstructorInfo daCtor = daType.GetConstructor(new Type[] { typeof(DebuggableAttribute.DebuggingModes) });
                CustomAttributeBuilder daBuilder = new CustomAttributeBuilder(daCtor, new object[] {
                    DebuggableAttribute.DebuggingModes.DisableOptimizations |
                    DebuggableAttribute.DebuggingModes.Default });
                bldr.SetCustomAttribute(daBuilder);
            }
            ModuleBuilder mbldr = bldr.DefineDynamicModule(moduleName, exeName, debug);

            TypeBuilder typeBuilder = mbldr.DefineType(assemblyName + ".Program", TypeAttributes.BeforeFieldInit 
                | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.Abstract);





            MethodBuilder mthdbldr = typeBuilder.DefineMethod("Main", MethodAttributes.Static | MethodAttributes.Public);

            ILGenerator gen = mthdbldr.GetILGenerator();
            Node.writer = mbldr.DefineDocument(fileName, Guid.Empty, Guid.Empty, Guid.Empty);
            Node.debug = debug;
            Node.labels = new LabelList();
            n.RecordLabels(gen);

            n.CheckTypes();
            // Create local variables

            List<LocalBuilder> locals = new List<LocalBuilder>();

            foreach (Variable v in symbols.Variables)
            {
                
                LocalBuilder local = v.EmitDeclare(gen);
                if (debug) local.SetLocalSymInfo(v.Value);
                locals.Add(local);
            }
            Node.locals = locals;



            #region Initialize locals
            // Emit a call to BuiltIns.OptionBase to set
            // the option base at run-time of BASIC program
            // this will be used for initializing all arrays
            MethodInfo setOptionBaseMethod =
                typeof(BuiltIns).GetMethod("OptionBase");
            gen.Emit(OpCodes.Ldc_I4, Statement.OptionBase);
            gen.Emit(OpCodes.Call, setOptionBaseMethod);
            for (int i = 0; i < symbols.Count; i++)
            {
                symbols[i].EmitDefaultValue(gen, locals[i]);
            }

            #endregion Intialize strings

            #region Create Static DATA data
            if (data.Count > 0)
            {
                MethodInfo addDataMethod = typeof(BuiltIns).GetMethod("AddData");

                for (int labelIndex = 0; labelIndex < data.Count; labelIndex++)
                {
                    object[] dataList = data.Values[labelIndex];
                    string label = data.Keys[labelIndex];

                    gen.Emit(OpCodes.Ldstr, label);
                    gen.Emit(OpCodes.Ldc_I4, dataList.Length);
                    gen.Emit(OpCodes.Newarr, typeof(object));

                    for (int i = 0; i < dataList.Length; i++)
                    {
                        gen.Emit(OpCodes.Dup); // duplicate array reference, it will be consumed on store element
                        gen.Emit(OpCodes.Ldc_I4, i);
                        object o = dataList[i];
                        if (o is string) gen.Emit(OpCodes.Ldstr, (string)o);
                        else
                        {
                            double d = (double)o;
                            gen.Emit(OpCodes.Ldc_R8, d);
                            gen.Emit(OpCodes.Box, typeof(double));
                        }
                        gen.Emit(OpCodes.Stelem_Ref);
                    }

                    gen.Emit(OpCodes.Call, addDataMethod);
                }
            }
            #endregion

            Node.returnSwitch = gen.DefineLabel();
            // Emit try
            Label end = gen.BeginExceptionBlock();
            Node.endLabel = end;

            n.Emit(gen);

            #region Emit Return Switch for GOSUB/RETURN
            gen.MarkSequencePoint(Node.writer, Int32.MaxValue, -1, Int32.MaxValue, -1);

            Label exitLabel = gen.DefineLabel();
            gen.Emit(OpCodes.Br, exitLabel);

            gen.MarkLabel(Node.returnSwitch);

            gen.Emit(OpCodes.Call, popMethod);
            gen.Emit(OpCodes.Switch, Node.returnLabels.ToArray());

            gen.MarkLabel(exitLabel);
            #endregion

            // Emit catch
            gen.BeginCatchBlock(typeof(Exception));
            MethodInfo getMessageMethod = typeof(Exception).GetMethod("get_Message");
            gen.Emit(OpCodes.Call, getMessageMethod);
            MethodInfo writeLineMethod = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
            gen.Emit(OpCodes.Call, writeLineMethod);
            gen.EndExceptionBlock();

            gen.MarkSequencePoint(Node.writer, Int32.MaxValue, -1, Int32.MaxValue, -1);
            gen.Emit(OpCodes.Ret);




            Type program = typeBuilder.CreateType();
            bldr.SetEntryPoint(mthdbldr);
            bldr.Save(exeName);

            if (runit)
            {
                program.GetMethod("Main").Invoke(null, new object[0]); 
            }
        }

        static private void InitializeReservedWords(SymbolTable symbols)
        {
            // Keywords for statements
            symbols.ReserveWord("BASE", Token.Base);
            symbols.ReserveWord("CALL", Token.Call);
            symbols.ReserveWord("CLEAR", Token.Subroutine);
            symbols.ReserveWord("DATA", Token.Data);
            symbols.ReserveWord("DIM", Token.Dim);
            symbols.ReserveWord("DISPLAY", Token.Print);
            symbols.ReserveWord("ELSE", Token.Else);
            symbols.ReserveWord("END", Token.End);
            symbols.ReserveWord("FOR", Token.For);
            symbols.ReserveWord("GO", Token.Go);
            symbols.ReserveWord("GOSUB", Token.Gosub);
            symbols.ReserveWord("GOTO", Token.Goto);
            symbols.ReserveWord("IF", Token.If);
            symbols.ReserveWord("INPUT", Token.Input);
            symbols.ReserveWord("LET", Token.Let);
            symbols.ReserveWord("NEXT", Token.Next);
            symbols.ReserveWord("ON", Token.On);
            symbols.ReserveWord("OPTION", Token.Option);
            symbols.ReserveWord("PRINT", Token.Print);
            symbols.ReserveWord("READ", Token.Read);
            symbols.ReserveWord("REM", Token.Remark);
            symbols.ReserveWord("RESTORE", Token.Restore);
            symbols.ReserveWord("RETURN", Token.Return);
            symbols.ReserveWord("STOP", Token.End);
            symbols.ReserveWord("SUB", Token.Sub);
            symbols.ReserveWord("TAB", Token.Tab);
            symbols.ReserveWord("THEN", Token.Then);
            symbols.ReserveWord("TO", Token.To);

            // String Functions
            symbols.ReserveWord("ASC", Token.Function);
            symbols.ReserveWord("CHR$", Token.Function);
            symbols.ReserveWord("LEN", Token.Function);
            symbols.ReserveWord("POS", Token.Function);
            symbols.ReserveWord("SEG$", Token.Function);
            symbols.ReserveWord("STR$", Token.Function);
            symbols.ReserveWord("VAL", Token.Function);

            // Number Functions
            symbols.ReserveWord("ABS", Token.Function);
            symbols.ReserveWord("ATN", Token.Function);
            symbols.ReserveWord("COS", Token.Function);
            symbols.ReserveWord("EXP", Token.Function);
            symbols.ReserveWord("INT", Token.Function);
            symbols.ReserveWord("LOG", Token.Function);
            symbols.ReserveWord("RANDOMIZE", Token.Randomize); // actually a statement
            symbols.ReserveWord("RND", Token.Function);
            symbols.ReserveWord("SGN", Token.Function);
            symbols.ReserveWord("SIN", Token.Function);
            symbols.ReserveWord("SQR", Token.Function);
            symbols.ReserveWord("TAN", Token.Function);
        }

    }
}
