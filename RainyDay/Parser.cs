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
			var imports = Imports();
			var module = Module();
			var types = TypeDefinitions();
			return new ScriptNode(imports, module, types);
		}

		private ImportNode Imports()
		{
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
			var members = new List<MemberNode>();
			while (_currentToken.Type != Tokens.BlockEnd)
			{
				var memberType = TypeSpec();
				var memberName = _currentToken.Value.ToString();
				Eat(Tokens.Identifier);

				if (_currentToken.Type == Tokens.ParamStart)
					members.Add(Function(memberName, memberType));
				else if (_currentToken.Type == Tokens.BlockStart)
					members.Add(Property(memberName, memberType));
				else
				{
					members.Add(Field(memberName, memberType));
					Eat(Tokens.Semi);
				}
			}
			return new TypeBodyNode(members.ToArray());
		}

		private FunctionNode Function(string name, TypeNode type)
		{
			var parameters = Parameters();
			var body = Block();

			return new FunctionNode(name, type, parameters, body);
		}

		private ParameterNode[] Parameters()
		{
			var parameters = new List<ParameterNode>();

			Eat(Tokens.ParamStart);
			while (_currentToken.Type != Tokens.ParamEnd)
			{
				var type = TypeSpec();
				var parameter = new ParameterNode(_currentToken.Value.ToString(), type);
				Eat(Tokens.Identifier);
			}
			Eat(Tokens.ParamEnd);
			return parameters.ToArray();
		}

		private PropertyNode Property(string name, TypeNode type)
		{
			Eat(Tokens.BlockStart);
			PropertyBodyNode getter = null, setter = null;
			if (_currentToken.Type == Tokens.PropertyGetter)
			{
				Eat(Tokens.PropertyGetter);
				getter = GetBody();
			}
			if (_currentToken.Type == Tokens.PropertySetter)
			{
				Eat(Tokens.PropertySetter);
				setter = GetBody();
			}

			return new PropertyNode(name, type, getter, setter);

			PropertyBodyNode GetBody()
			{
				if (_currentToken.Type == Tokens.Lambda)
					return new PropertyBodyNode(Lambda());
				if (_currentToken.Type == Tokens.BlockStart)
					return new PropertyBodyNode(Block());
				return null;
			}
		}

		private FieldNode Field(string name, TypeNode type)
		{
			if (_currentToken.Type == Tokens.Assign)
			{
				Eat(Tokens.Assign);
				return new FieldNode(name, type, Expression());
			}
			return new FieldNode(name, type);
		}

		private BlockNode Block()
		{
			Eat(Tokens.BlockStart);
			var nodes = Statements();
			Eat(Tokens.BlockEnd);
			return new BlockNode(nodes);
		}

		private LambdaNode Lambda()
		{
			return null;
		}

		private AstNode[] Statements()
		{
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
			if (_currentToken.Type == Tokens.BlockStart)
				return Block();
			else if (_currentToken.Type == Tokens.Variable)
			{
				Eat(_currentToken.Type);
				return VariableDeclaration();
			}
			else if (_currentToken.Type == Tokens.Identifier)
				return Assignment();
			else
				return new NoOpNode();
		}

		private AstNode Assignment()
		{
			var left = Variable();
			Eat(Tokens.Assign);
			var right = Expression();
			return new AssignNode(left, right);
		}

		private AstNode Expression()
		{
			var node = Term();

			var ops = new[] { Tokens.Add, Tokens.Subtract };

			while (ops.Contains(_currentToken.Type))
			{
				var token = _currentToken;
				Eat(token.Type);
				if (token.Type == Tokens.Add)
					node = new AddNode(node, Term());
			}

			return node;
		}

		private AstNode Term()
		{
			var node = Factor();

			var ops = new[] { Tokens.Multiply, Tokens.Divide };

			while (ops.Contains(_currentToken.Type))
			{
				var token = _currentToken;
				Eat(token.Type);
				if (token.Type == Tokens.Multiply)
					node = new MultiplyNode(node, Factor());
			}

			return node;
		}

		private AstNode Factor()
		{
			var token = _currentToken;
			if (Tokens.IsUnaryOperator(token.Type)) { Eat(token.Type); return new UnaryOperationNode(token, Factor()); }
			else if (token.Type == Tokens.IncrementByOne) { Eat(token.Type); return new PreIncrementNode(Factor()); }
			else if (token.Type == Tokens.DecrementByOne) { Eat(token.Type); return new PreDecrementNode(Factor()); }
			else if (Tokens.IsNumberType(token.Type))
			{
				var number = Number();
				token = _currentToken;
				if (token.Type == Tokens.IncrementByOne) { Eat(token.Type); return new PostIncrementNode(number); }
				else if (token.Type == Tokens.DecrementByOne) { Eat(token.Type); return new PostDecrementNode(number); }
				return number;
			}
			else if (token.Type == Tokens.String) { Eat(token.Type); return new StringNode((string)token.Value); }
			else if (token.Type == Tokens.Character) { Eat(token.Type); return new CharacterNode((char)token.Value); }
			else if (token.Type == Tokens.ParamStart)
			{
				Eat(Tokens.ParamStart);
				var expression = Expression();
				Eat(Tokens.ParamEnd);
				return expression;
			}
			else return Variable();
			throw new Exception("Factor bad. Very bad. No like it. Stop.");
		}

		private AstNode Number()
		{
			var token = _currentToken;
			if (token.Type == Tokens.Int32) { Eat(token.Type); return new IntegerNode((int)token.Value); }
			else if (token.Type == Tokens.Single) { Eat(token.Type); return new SingleNode((float)token.Value); }
			throw new Exception($"Parse Error: Failed to parse number from token {token}");
		}

		private VariableDeclarationNode VariableDeclaration()
		{
			var variables = new List<VariableNode> { new VariableNode(_currentToken.ToString()) };
			Eat(Tokens.Identifier);

			while (_currentToken.Type == Tokens.Comma)
			{
				Eat(Tokens.Comma);
				variables.Add(new VariableNode(_currentToken.ToString()));
				Eat(Tokens.Identifier);
			}

			Eat(Tokens.Colon);
			var typeNode = TypeSpec();

			if (_currentToken.Type == Tokens.Assign)
			{
				Eat(Tokens.Assign);
				return new VariableDeclarationNode(variables.ToArray(), typeNode, Expression());
			}
			return new VariableDeclarationNode(variables.ToArray(), typeNode);
		}

		private VariableNode Variable()
		{
			var node = new VariableNode(_currentToken.Value.ToString());
			Eat(Tokens.Identifier);
			return node;
		}

		private TypeNode TypeSpec()
		{
			var token = _currentToken;
			var node = new TypeNode(token.Type);
			Eat(token.Type);
			return node;
		}
	}
}