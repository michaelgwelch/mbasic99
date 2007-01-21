using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using TiBasicRuntime;
namespace mbasic.SyntaxTree
{
    class BuiltInsMethodCall : Expression
    {
        private static readonly Type builtInsType = typeof(BuiltIns);
        private static readonly MethodInfo readStringFromData =
            builtInsType.GetMethod("ReadStringFromData");
        private static readonly MethodInfo readNumberFromData =
            builtInsType.GetMethod("ReadNumberFromData");
        private static readonly MethodInfo readStringFromConsole =
            builtInsType.GetMethod("ReadStringFromConsole");
        private static readonly MethodInfo readNumberFromConsole =
            builtInsType.GetMethod("ReadNumberFromConsole");
        private static readonly MethodInfo createStringArray =
            builtInsType.GetMethod("CreateStringArray");
        private static readonly MethodInfo createNumberArray =
            builtInsType.GetMethod("CreateNumberArray");

        private MethodInfo method;
        private BasicType type;
        private BuiltInsMethodCall(MethodInfo method)
            : base(LineId.None)
        {
            this.method = method;
            if (this.method.ReturnType == typeof(string)) type = BasicType.String;
            else type = BasicType.Number;
        }

        public override void Emit(ILGenerator gen)
        {
            gen.Emit(OpCodes.Call, method);
        }

        public override BasicType GetBasicType()
        {
            return type;
        }

        public static BuiltInsMethodCall ReadStringFromData()
        {
            return new BuiltInsMethodCall(readStringFromData);
        }

        public static BuiltInsMethodCall ReadNumberFromData()
        {
            return new BuiltInsMethodCall(readNumberFromData);
        }

        public static BuiltInsMethodCall ReadStringFromConsole()
        {
            return new BuiltInsMethodCall(readStringFromConsole);
        }

        public static BuiltInsMethodCall ReadNumberFromConsole()
        {
            return new BuiltInsMethodCall(readNumberFromConsole);
        }

        public static BuiltInsMethodCall CreateStringArray()
        {
            return new BuiltInsMethodCall(createStringArray);
        }

        public static BuiltInsMethodCall CreateNumberArray()
        {
            return new BuiltInsMethodCall(createNumberArray);
        }

    }
}
