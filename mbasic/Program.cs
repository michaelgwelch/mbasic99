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

    class Program
    {
        static void Main(string[] args)
        {
            bool debug = true;
            bool runit = true;

            string fileName = args[0];
            string assemblyName = Path.GetFileNameWithoutExtension(fileName);
            string moduleName = assemblyName + ".exe";
            string exeName = assemblyName + ".exe";

            Stream stream = File.OpenRead(fileName);
            SymbolTable symbols = new SymbolTable();
            InitializeReservedWords(symbols);
            Parser parser = new Parser(stream, symbols);
            Node.lexer = parser.lexer;

            Statement n = parser.Parse();

            Node.symbols = symbols;
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

            TypeBuilder typeBuilder = mbldr.DefineType("TiBasic.Program", TypeAttributes.BeforeFieldInit 
                | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.Abstract);





            MethodBuilder mthdbldr = typeBuilder.DefineMethod("Main", MethodAttributes.Static | MethodAttributes.Public);

            ILGenerator gen = mthdbldr.GetILGenerator();
            // Create global variables

            List<LocalBuilder> locals = new List<LocalBuilder>();
               
            foreach(Variable v in symbols.Variables)
            {
                LocalBuilder local;
                if (v.BasicType == BasicType.String) locals.Add(local = gen.DeclareLocal(typeof(string)));
                else locals.Add(local = gen.DeclareLocal(typeof(double)));
                if (debug) local.SetLocalSymInfo(v.Value);
            }
            Node.locals = locals;
            Node.writer = mbldr.DefineDocument(fileName, Guid.Empty, Guid.Empty, Guid.Empty);
            Node.debug = debug;
            Node.labels = new LabelList();

            n.CheckTypes();
            n.RecordLabels(gen);
            // Emit try
            Label end = gen.BeginExceptionBlock();
            Node.endLabel = end;

            n.Emit(gen);

            // Emit catch
            gen.BeginCatchBlock(typeof(Exception));
            MethodInfo getMessageMethod = typeof(Exception).GetMethod("get_Message");
            gen.Emit(OpCodes.Call, getMessageMethod);
            MethodInfo writeLineMethod = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
            gen.Emit(OpCodes.Call, writeLineMethod);
            gen.EndExceptionBlock();
            

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
            symbols.ReserveWord("ELSE", Token.Else);
            symbols.ReserveWord("PRINT", Token.Print);
            symbols.ReserveWord("INPUT", Token.Input);
            symbols.ReserveWord("GOTO", Token.Goto);
            symbols.ReserveWord("IF", Token.If);
            symbols.ReserveWord("FOR", Token.For);
            symbols.ReserveWord("TO", Token.To);
            symbols.ReserveWord("NEXT", Token.Next);
            symbols.ReserveWord("THEN", Token.Then);
            symbols.ReserveWord("CALL", Token.Call);
            symbols.ReserveWord("CLEAR", Token.Subroutine);
            symbols.ReserveWord("DISPLAY", Token.Print);
            symbols.ReserveWord("REM", Token.Remark);
            symbols.ReserveWord("LET", Token.Let);
            symbols.ReserveWord("END", Token.End);
            symbols.ReserveWord("STOP", Token.End);
            symbols.ReserveWord("TAB", Token.Tab);

            // String Functionis
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
