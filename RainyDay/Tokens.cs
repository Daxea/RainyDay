using System;
using System.Collections.Generic;
using System.Linq;

namespace RainyDay
{
    public static class Tokens
    {
        #region Language Basics

        public const string Semi = "Semicolon";
        public const string Colon = "Colon";
        public const string Comma = "Comma";

        public const string ScriptEnd = "ScriptEnd";
        public const string Using = "Using";
        public const string Module = "Module";
        public const string Variable = "Variable";
        public const string Identifier = "Identifier";
        public const string Branch = "Branch";
        public const string LoopWhile = "While";
        public const string LoopDo = "Do";
        public const string LoopFor = "For";
        public const string LoopForEach = "ForEach";

        #endregion
        #region Comparisons

        public const string Equal = "Compare.Equal";
        public const string NotEqual = "Compare.NotEqual";
        public const string GreaterThan = "Compare.GreaterThan";
        public const string GreaterThanOrEqualTo = "Compare.GreaterThanOrEqualTo";
        public const string LessThan = "Compare.LessThan";
        public const string LessThanOrEqualTo = "Compare.LessThanOrEqualTo";

        #endregion
        #region Operators

        public const string Assign = "Assign";
        public const string Add = "Add";
        public const string Subtract = "Subtract";
        public const string Multiply = "Multiply";
        public const string Divide = "Divide";
        public const string Not = "Not";

        public const string IncrementBy = "IncrementBy";
        public const string DecrementBy = "DecrementBy";
        public const string MultiplyBy = "MultiplyBy";
        public const string DivideBy = "DivideBy";

        public const string IncrementByOne = "IncrementByOne";
        public const string DecrementByOne = "DecrementByOne";

        #endregion
        #region Primitive Types

        public const string Int32 = "Int32";
        public const string Single = "Single";
        public const string String = "String";
        public const string Character = "Character";

        #endregion
        #region Blocks/Groups

        public const string ArrayStart = "ArrayStart";
        public const string ArrayEnd = "ArrayEnd";

        public const string BlockStart = "BlockStart";
        public const string BlockEnd = "BlockEnd";

        public const string ParamStart = "ParamStart";
        public const string ParamEnd = "ParamEnd";

        #endregion

        private static Dictionary<string, Token> _reserved = new Dictionary<string, Token> {
            { "var", new Token(Variable) },
            { "int", new Token(Int32) },
            { "float", new Token(Single) },
            { "string", new Token(String) },
            { "char", new Token(Character) },
            { "module", new Token(Module) },
            { "use", new Token(Using) },
            { "if", new Token(Branch) },
            { "while", new Token(LoopWhile) },
            { "do", new Token(LoopDo) },
            { "for", new Token(LoopFor) },
            { "foreach", new Token(LoopForEach) }
        };
        public static Dictionary<string, Token> Reserved => _reserved;
    }
}