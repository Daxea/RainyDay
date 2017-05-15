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
		public const string Dot = "Dot";
		public const string Lambda = "Lambda";
		public const string Return = "Return";

		public const string ScriptEnd = "ScriptEnd";
		public const string Using = "Using";
		public const string Module = "Module";
		public const string Type = "Type";
		public const string Function = "Function";
		public const string Property = "Property";
		public const string PropertyGetter = "PropertyGetter";
		public const string PropertySetter = "PropertySetter";
		public const string Variable = "Variable";
		public const string Identifier = "Identifier";
		public const string Branch = "Branch";
        public const string Else = "Else";
		public const string LoopWhile = "While";
		public const string LoopDo = "Do";
		public const string LoopFor = "For";
		public const string LoopForEach = "ForEach";
        public const string True = "True";
        public const string False = "False";

		#endregion
		#region Comparisons

		public const string Equal = "Compare.Equal";
		public const string NotEqual = "Compare.NotEqual";
		public const string GreaterThan = "Compare.GreaterThan";
		public const string GreaterThanOrEqualTo = "Compare.GreaterThanOrEqualTo";
		public const string LessThan = "Compare.LessThan";
		public const string LessThanOrEqualTo = "Compare.LessThanOrEqualTo";

		private static string[] _Comparisons = new[] { Equal, NotEqual, GreaterThan, GreaterThanOrEqualTo, LessThan, LessThanOrEqualTo };

		public static bool IsComparison(string tokenType) => _Comparisons.Contains(tokenType);

		#endregion
		#region Operators

		public const string Assign = "Assign";
		public const string Add = "Add";
		public const string Subtract = "Subtract";
		public const string Multiply = "Multiply";
		public const string Divide = "Divide";

		private static string[] _BinaryOperators = new[] { Assign, Add, Subtract, Multiply, Divide };

		public static bool IsBinaryOperator(string tokenType) => _BinaryOperators.Contains(tokenType);

        public const string IncrementBy = "IncrementBy";
        public const string DecrementBy = "DecrementBy";
        public const string MultiplyBy = "MultiplyBy";
        public const string DivideBy = "DivideBy";

		private static string[] _OperateByOperators = new[] { IncrementBy, DecrementBy, MultiplyBy, DivideBy };

		public static bool IsOperateBy(string tokenType) => _OperateByOperators.Contains(tokenType);

        public const string IncrementByOne = "IncrementByOne";
        public const string DecrementByOne = "DecrementByOne";

		private static string[] _IncrementOrDecrementOperators = new[] { IncrementByOne, DecrementByOne };

		public static bool IsIncrementOrDecrementOperator(string tokenType) => _IncrementOrDecrementOperators.Contains(tokenType);

		public const string UnaryIncrement = "UnaryIncrement";
		public const string UnaryDecrement = "UnaryDecrement";
		public const string Not = "Not";

		private static string[] _UnaryOperators = new[] { UnaryIncrement, UnaryDecrement, Not };

		public static bool IsUnaryOperator(string tokenType) => _UnaryOperators.Contains(tokenType);

		#endregion
		#region Primitive Types

		public const string Void = "Void";

        public const string Int32 = "Int32";
        public const string Single = "Single";

		private static string[] _NumberTypes = new [] { Int32, Single };

		public static bool IsNumberType(string tokenType) => _NumberTypes.Contains(tokenType);

        public const string String = "String";
        public const string Character = "Character";
        public const string Boolean = "Boolean";

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
            { "type", new Token(Type) },
            { "func", new Token(Function) },
            { "prop", new Token(Property) },
            { "get", new Token(PropertyGetter) },
            { "set", new Token(PropertySetter) },
            { "var", new Token(Variable) },
            { "void", new Token(Void) },
            { "int", new Token(Int32) },
            { "float", new Token(Single) },
            { "string", new Token(String) },
            { "char", new Token(Character) },
            { "bool", new Token(Boolean) },
            { "true", new Token(True) },
            { "false", new Token(False) },
            { "module", new Token(Module) },
            { "use", new Token(Using) },
            { "if", new Token(Branch) },
            { "else", new Token(Else) },
            { "while", new Token(LoopWhile) },
            { "do", new Token(LoopDo) },
            { "for", new Token(LoopFor) },
            { "foreach", new Token(LoopForEach) },
			{ "return", new Token(Return) }
        };
        public static Dictionary<string, Token> Reserved => _reserved;
    }
}