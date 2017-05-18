using System;
using System.Collections.Generic;
using System.Linq;

using RainyDay.Ast;

namespace RainyDay
{
	public class Parser
	{
		private Lexer _lexer;
		private Token _currentToken;

		public Parser(Lexer lexer)
		{
			_lexer = lexer;
			_currentToken = _lexer.GetNextToken();
		}

		public AstNode ParseStatement() => Statement();

		public AstNode ParseScript() => Script();

		private void Eat(string tokenType)
		{
			if (_currentToken.Type == tokenType)
				_currentToken = _lexer.GetNextToken();
			else
				throw new Exception("Parsing Error!");
		}

		private ScriptNode Script()
		{
			// SCRIPT = (IMPORTS)(MODULE)(TYPE-DEF)*

			var imports = Imports();
			var module = Module();
			var types = TypeDefinitions();
			return new ScriptNode(imports, module, types);
		}

		private ImportNode Imports()
		{
			// IMPORTS = (use IDENTIFIER SEMI)*

			var imports = new List<string>();
			while (_currentToken.Type == Tokens.Using)
			{
				Eat(Tokens.Using);
				imports.Add(_currentToken.Value.ToString());
				Eat(Tokens.Identifier);
				Eat(Tokens.Semi);
			}
			return new ImportNode(imports.ToArray());
		}

		private ModuleNode Module()
		{
			// MODULE = module IDENTIFIER(DOT IDENTIFIER)* SEMI

			if (_currentToken.Type != Tokens.Module)
				return new ModuleNode(string.Empty);
			Eat(Tokens.Module);
			if (_currentToken.Type != Tokens.Identifier)
				return new ModuleNode(string.Empty);
			var result = _currentToken.Value.ToString();
			Eat(Tokens.Identifier);
			while (_currentToken.Type == Tokens.Dot)
			{
				Eat(Tokens.Dot);
				result += _currentToken.Value.ToString();
				Eat(Tokens.Identifier);
			}
			Eat(Tokens.Semi);
			return new ModuleNode(result);
		}

		private TypeDefinitionNode[] TypeDefinitions()
		{
			var types = new List<TypeDefinitionNode>();
			while (_currentToken.Type == Tokens.Type)
			{
				Eat(Tokens.Type);
				types.Add(TypeDefinition());
			}
			return types.ToArray();
		}

		private TypeDefinitionNode TypeDefinition()
		{
			// TYPE-DEF = type IDENTIFIER BLOCK-START TYPE-BODY BLOCK-END

			var typeName = _currentToken.Value.ToString();
			Eat(Tokens.Identifier);
			if (_currentToken.Type == Tokens.Colon)
			{
				// todo: add inheritence chaining
			}
			TypeBodyNode typeBody = null;
			if (_currentToken.Type == Tokens.BlockStart)
			{
				Eat(Tokens.BlockStart);
				typeBody = TypeBody();
				Eat(Tokens.BlockEnd);
			}
			return new TypeDefinitionNode(typeName, typeBody);
		}

		private TypeBodyNode TypeBody()
		{
			// TYPE-BODY = (MEMBER)*

			var members = new List<MemberNode>();
			while (_currentToken.Type != Tokens.BlockEnd)
				members.Add(Member());
			return new TypeBodyNode(members.ToArray());
		}

		private MemberNode Member()
		{
			// MEMBER = TYPE-SPEC IDENTIFIER FUNCTION |
			//          TYPE-SPEC IDENTIFIER PROPERTY |
			//          TYPE-SPEC IDENTIFIER FIELD SEMI

			var memberType = TypeSpec();
			var memberName = _currentToken.Value.ToString();
			Eat(Tokens.Identifier);

			if (_currentToken.Type == Tokens.ParamStart)
				return Function(memberName, memberType);
			else if (_currentToken.Type == Tokens.BlockStart)
				return Property(memberName, memberType);
			else
			{
				var member = Field(memberName, memberType);
				Eat(Tokens.Semi);
				return member;
			}
		}

		private FunctionNode Function(string name, TypeNode type)
		{
			// FUNCTION = PARAMETERS BLOCK

			var parameters = Parameters();
			var body = Block();

			return new FunctionNode(name, type, parameters, body);
		}

		private ParameterNode[] Parameters()
		{
			// PARAMETERS = PARAM-START (TYPE-SPEC IDENTIFIER (COMMA TYPE-SPEC IDENTIFIER)*) PARAM-END

			var parameters = new List<ParameterNode>();

			Eat(Tokens.ParamStart);
			if (_currentToken.Type == Tokens.ParamEnd)
			{
				Eat(Tokens.ParamEnd);
				return parameters.ToArray();
			}

			var type = TypeSpec();
			parameters.Add(new ParameterNode(_currentToken.Value.ToString(), type));
			Eat(Tokens.Identifier);

			while (_currentToken.Type == Tokens.Comma)
			{
				Eat(Tokens.Comma);
				type = TypeSpec();
				parameters.Add(new ParameterNode(_currentToken.Value.ToString(), type));
				Eat(Tokens.Identifier);
			}
			Eat(Tokens.ParamEnd);
			return parameters.ToArray();
		}

		private PropertyNode Property(string name, TypeNode type)
		{
			// PROPERTY = BLOCK-START (get PROPERTY-BODY) (set PROPERTY-BODY) BLOCK-END |
			//            BLOCK-START (set PROPERTY-BODY) (get PROPERTY-BODY) BLOCK-END

			Eat(Tokens.BlockStart);
			PropertyBodyNode getter = null, setter = null;
			while (_currentToken.Type != Tokens.BlockEnd)
			{
				if (_currentToken.Type == Tokens.PropertyGetter)
				{
					Eat(Tokens.PropertyGetter);
					getter = PropertyBody();
				}
				if (_currentToken.Type == Tokens.PropertySetter)
				{
					Eat(Tokens.PropertySetter);
					setter = PropertyBody();
				}
			}

			return new PropertyNode(name, type, getter, setter);

			PropertyBodyNode PropertyBody()
			{
				// PROPERTY-BODY = LAMBDA | BLOCK

				if (_currentToken.Type == Tokens.Lambda)
					return new PropertyBodyNode(Lambda());
				if (_currentToken.Type == Tokens.BlockStart)
					return new PropertyBodyNode(Block());
				return null;
			}
		}

		private FieldNode Field(string name, TypeNode type)
		{
			// FIELD = (ASSIGN EXPRESSION)

			if (_currentToken.Type == Tokens.Assign)
			{
				Eat(Tokens.Assign);
				return new FieldNode(name, type, Expression());
			}
			return new FieldNode(name, type);
		}

		private BlockNode Block()
		{
			// BLOCK = BLOCK-START STATEMENTS BLOCK-END

			Eat(Tokens.BlockStart);
			var nodes = Statements();
			Eat(Tokens.BlockEnd);
			return new BlockNode(nodes);
		}

		private LambdaNode Lambda()
		{
			// LAMBDA = LAMBDA-OP BLOCK |
			//          LAMBDA-OP STATEMENT SEMI

			Eat(Tokens.Lambda);
			if (_currentToken.Type == Tokens.BlockStart)
				return new LambdaNode(Block());
			var statement = Statement();
			Eat(Tokens.Semi);
			return new LambdaNode(statement);
		}

		private AstNode[] Statements()
		{
			// STATEMENTS = STATEMENT SEMI (STATEMENT SEMI)*

			var node = Statement();

			var results = new List<AstNode> { node };

			while (_currentToken.Type == Tokens.Semi)
			{
				Eat(Tokens.Semi);
				results.Add(Statement());
			}

			if (_currentToken.Type == Tokens.Identifier)
				throw new Exception("wat");

			return results.ToArray();
		}

		private AstNode Statement()
		{
			// STATEMENT = BLOCK |
			//             var VARIABLE-DECL | // implicit variable declaration
			//             IDENITIFER VARIABLE-DECL | // explicit variable declaration with user-defined type
			//             TYPE-SPEC VARAIBLE-DECL | // explicit variable declaration with primitive type
			//             IDENTIFIER ASSIGNMENT |
			//			   IDENTIFIER INCREMENT-BY EXPRESSION |
			//			   IDENTIFIER DECREMENT-BY EXPRESSION |
			//			   IDENTIFIER MULTIPLY-BY EXPRESSION |
			//			   IDENTIFIER DIVIDE-BY EXPRESSION |
			//             RETURN EXPRESSION |
			//             BRANCH |
			//             NO-OP

			if (_currentToken.Type == Tokens.BlockStart)
				return Block();
			else if (_currentToken.Type == Tokens.Variable)
			{
				Eat(_currentToken.Type);
				return VariableDeclaration(TypeNode.Inferred);
			}
			else if (_currentToken.Type == Tokens.Identifier)
			{
				var id = _currentToken.Value.ToString();
				Eat(Tokens.Identifier);
				if (_currentToken.Type == Tokens.Assign)
					return Assignment(new VariableNode(id));
				else if (_currentToken.Type == Tokens.Identifier)
					return VariableDeclaration(new TypeNode(id));
				else if (_currentToken.Type == Tokens.IncrementBy)
				{
					Eat(Tokens.IncrementBy);
					return new IncrementByNode(new VariableNode(id), Expression());
				}
				else if (_currentToken.Type == Tokens.DecrementBy)
				{
					Eat(Tokens.DecrementBy);
					return new DecrementByNode(new VariableNode(id), Expression());
				}
				else if (_currentToken.Type == Tokens.MultiplyBy)
				{
					Eat(Tokens.MultiplyBy);
					return new MultiplyByNode(new VariableNode(id), Expression());
				}
				else if (_currentToken.Type == Tokens.DivideBy)
				{
					Eat(Tokens.DivideBy);
					return new DivideByNode(new VariableNode(id), Expression());
				}
			}
			else if (Tokens.IsPrimitiveType(_currentToken.Type))
				return VariableDeclaration(TypeSpec());
			else if (_currentToken.Type == Tokens.Return)
			{
				Eat(Tokens.Return);
				return new ReturnNode(Expression());
			}
			else if (_currentToken.Type == Tokens.Branch)
			{
				Eat(Tokens.Branch);
				return Branch();
			}
			else
				return new NoOpNode();
			throw new Exception("bad statement");
		}

		private AssignNode Assignment(VariableNode variable)
		{
			Eat(Tokens.Assign);
			var right = Expression();
			return new AssignNode(variable, right);
		}

		private AstNode Expression()
		{
			// EXPRESSION = TERM (ADD | SUBTRACT TERM)*

			var node = Term();

			var ops = new[] { Tokens.Add, Tokens.Subtract };

			while (ops.Contains(_currentToken.Type))
			{
				var token = _currentToken;
				Eat(token.Type);
				if (token.Type == Tokens.Add)
					node = new AddNode(node, Term());
				else if (token.Type == Tokens.Subtract)
					node = new SubtractNode(node, Term());
			}

			return node;
		}

		private BranchNode Branch()
		{
			// BRANCH = PARAM-START EXPRESSION PARAM-END BLOCK (else BRANCH)* (else BLOCK | STATEMENT SEMI) |
			//          PARAM-START EXPRESSION PARAM-END STATEMENT SEMI (else BRANCH)* (else BLOCK | STATEMENT SEMI)

			Eat(Tokens.ParamStart);
			var condition = Expression();
			Eat(Tokens.ParamEnd);
			AstNode ifTrue = null;
			if (_currentToken.Type == Tokens.BlockStart)
				ifTrue = Block();
			AstNode ifFalse = null;
			if (_currentToken.Type == Tokens.Else)
			{
				Eat(Tokens.Else);
				if (_currentToken.Type == Tokens.Branch)
					ifFalse = Branch();
				else if (_currentToken.Type == Tokens.BlockStart)
					ifFalse = Block();
				else
					ifFalse = Statement();
			}
			return new BranchNode(condition, ifTrue, ifFalse);
		}

		private AstNode Term()
		{
			// TERM = FACTOR (MULTIPLY | DIVIDE FACTOR)*

			var node = Factor();

			var ops = new[] { Tokens.Multiply, Tokens.Divide };

			while (ops.Contains(_currentToken.Type))
			{
				var token = _currentToken;
				Eat(token.Type);
				if (token.Type == Tokens.Multiply)
					node = new MultiplyNode(node, Factor());
				else if (token.Type == Tokens.Divide)
					node = new DivideNode(node, Factor());
			}

			return node;
		}

		private AstNode Factor()
		{
			// FACTOR = UNARY-OP FACTOR |
			//          INCREMENT-BY-ONE VARIABLE |
			//          DECREMENT-BY-ONE VARIABLE |
			//          NUMBER (INCREMENT-BY-ONE) |
			//          NUMBER (DECREMENT-BY-ONE) |
			//          STRING | CHAR | BOOLEAN |
			//          PARAM-START EXPRESSION PARAM-END |
			//          VARIABLE

			var token = _currentToken;
			if (Tokens.IsUnaryOperator(token.Type)) { Eat(token.Type); return new UnaryOperationNode(token, Factor()); }
			else if (token.Type == Tokens.IncrementByOne) { Eat(token.Type); return new PreIncrementNode(Variable()); }
			else if (token.Type == Tokens.DecrementByOne) { Eat(token.Type); return new PreDecrementNode(Variable()); }
			else if (Tokens.IsNumberType(token.Type)) return Number();
			else if (token.Type == Tokens.String) { Eat(token.Type); return new StringNode((string)token.Value); }
			else if (token.Type == Tokens.Character) { Eat(token.Type); return new CharacterNode((char)token.Value); }
			else if (token.Type == Tokens.True) { Eat(token.Type); return new BooleanNode(true); }
			else if (token.Type == Tokens.False) { Eat(token.Type); return new BooleanNode(false); }
			else if (token.Type == Tokens.ParamStart)
			{
				Eat(Tokens.ParamStart);
				var expression = Expression();
				Eat(Tokens.ParamEnd);
				return expression;
			}
			else
			{
				var variable = Variable();
				if (token.Type == Tokens.IncrementByOne) { Eat(token.Type); return new PostIncrementNode(variable); }
				else if (token.Type == Tokens.DecrementByOne) { Eat(token.Type); return new PostDecrementNode(variable); }
				return variable;

			}
			throw new Exception("Factor bad. Very bad. No like it. Stop.");
		}

		private AstNode Number()
		{
			// NUMBER = Int32 | Single

			var token = _currentToken;
			if (token.Type == Tokens.Int32) { Eat(token.Type); return new IntegerNode((int)token.Value); }
			else if (token.Type == Tokens.Single) { Eat(token.Type); return new SingleNode((float)token.Value); }
			throw new Exception($"Parse Error: Failed to parse number from token {token}");
		}

		private VariableDeclarationNode VariableDeclaration(TypeNode type)
		{
			// VARIABLE-DECL = VARIABLES (ASSIGN EXPRESSION)

			var variables = Variables();
			if (type.IsInferred && _currentToken.Type != Tokens.Assign)
				throw new Exception("Implicit types must be assigned to when declared!");
			if (_currentToken.Type == Tokens.Assign)
			{
				Eat(Tokens.Assign);
				return new VariableDeclarationNode(variables.ToArray(), type, Expression());
			}
			return new VariableDeclarationNode(variables.ToArray(), type);
		}

		private List<VariableNode> Variables()
		{
			// VARIABLES = VARIABLE (COMMA VARIABLE)*

			var variables = new List<VariableNode> { Variable() };
			while (_currentToken.Type == Tokens.Comma)
			{
				Eat(Tokens.Comma);
				variables.Add(Variable());
			}
			return variables;
		}

		private VariableNode Variable()
		{
			// VARIABLE = IDENTIFIER

			var node = new VariableNode(_currentToken.Value.ToString());
			Eat(Tokens.Identifier);
			return node;
		}

		private TypeNode TypeSpec()
		{
			// TYPE-SPEC

			var token = _currentToken;
			var node = new TypeNode(token.Type);
			Eat(token.Type);
			return node;
		}
	}
}