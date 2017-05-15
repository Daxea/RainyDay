using System;

namespace RainyDay.Ast
{
	public abstract class AstNode { }

	public sealed class NoOpNode : AstNode { }

	public class TypeNode : AstNode
	{
		public string Name { get; }

		public TypeNode(string name)
		{
			Name = name;
		}
	}

	public class BlockNode : AstNode
	{
		public AstNode[] Children { get; }

		public BlockNode(params AstNode[] children)
		{
			Children = children;
		}
	}

	public class ScriptNode : AstNode
	{
		public ImportNode Imports { get; }
		public ModuleNode Module { get; }
		public TypeDefinitionNode[] Types { get; }

		public ScriptNode(ImportNode imports, ModuleNode module, TypeDefinitionNode[] types)
		{
			Imports = imports;
			Module = module;
			Types = types ?? new TypeDefinitionNode[0];
		}
	}

	public class TypeDefinitionNode : AstNode
	{
		public string Name { get; }
		public TypeBodyNode TypeBody { get; }

		public TypeDefinitionNode(string name, TypeBodyNode typeBody)
		{
			Name = name;
			TypeBody = typeBody;
		}
	}

	public class TypeBodyNode : AstNode
	{
		public MemberNode[] Members { get; }

		public TypeBodyNode(MemberNode[] members)
		{
			Members = members ?? new MemberNode[0];
		}
	}

	public class MemberNode : AstNode
	{
		public string Name { get; }
		public TypeNode Type { get; }

		public MemberNode(string name, TypeNode type)
		{
			Name = name;
			Type = type;
		}
	}

	public class FieldNode : MemberNode
	{
		public object Value { get; }

		public FieldNode(string name, TypeNode type) : base(name, type)
		{
			Value = null;
		}

		public FieldNode(string name, TypeNode type, object value) : base(name, type)
		{
			Value = value;
		}
	}

	public class FunctionNode : MemberNode
	{
		public ParameterNode[] Parameters { get; }
		public BlockNode Body { get; }

		public FunctionNode(string name, TypeNode type, ParameterNode[] parameters, BlockNode body) : base(name, type)
		{
			Parameters = parameters;
			Body = body;
		}
	}

	public class PropertyNode : MemberNode
	{
		public PropertyBodyNode Getter { get; }
		public PropertyBodyNode Setter { get; }

		public PropertyNode(string name, TypeNode type, PropertyBodyNode getter, PropertyBodyNode setter) : base(name, type)
		{
			Getter = getter;
			Setter = setter;
		}
	}

	public class PropertyBodyNode : AstNode
	{
		public BlockNode Body { get; }

		public PropertyBodyNode(BlockNode body)
		{
			Body = body;
		}
	}

	public class ParameterNode : AstNode
	{
		public string Name { get; }
		public TypeNode Type { get; }

		public ParameterNode(string name, TypeNode type)
		{
			Name = name;
			Type = type;
		}
	}

	public class LambdaNode : BlockNode { }

	public class ImportNode : AstNode
	{
		/// <summary>
		/// Gets the modules to import.
		/// </summary>
		public string[] Modules { get; }

		public ImportNode(string[] modules)
		{
			Modules = modules;
		}
	}

	public class ModuleNode : AstNode
	{
		public string Name { get; }

		public ModuleNode(string name)
		{
			Name = name;
		}
	}

	public class UnaryOperationNode : AstNode
	{
		public Token Operation { get; }
		public AstNode Expression { get; }

		public UnaryOperationNode(Token operation, AstNode expression)
		{
			Operation = operation;
			Expression = expression;
		}
	}

	public class VariableNode : AstNode
	{
		public string Name { get; }

		public VariableNode(string name)
		{
			Name = name;
		}
	}

	public abstract class DataNode<T> : AstNode
	{
		public T Value { get; }

		public DataNode(T value)
		{
			Value = value;
		}

		public override string ToString() => Value.ToString();
	}

	public class IntegerNode : DataNode<int>
	{
		public IntegerNode(int value) : base(value) { }
	}

	public class SingleNode : DataNode<float>
	{
		public SingleNode(float value) : base(value) { }
	}

	public class StringNode : DataNode<string>
	{
		public StringNode(string value) : base(value) { }
	}

	public class CharacterNode : DataNode<char>
	{
		public CharacterNode(char value) : base(value) { }
	}
}