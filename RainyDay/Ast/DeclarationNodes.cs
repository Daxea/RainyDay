namespace RainyDay.Ast
{
	public class DeclarationNode : AstNode { }

	public class VariableDeclarationNode : DeclarationNode
	{
		public VariableNode[] Variables { get; }
		public TypeNode Type { get; }
		public object Value { get; }

		public VariableDeclarationNode(VariableNode[] variables, TypeNode type)
		{
			Variables = variables;
			Type = type;
			Value = null;
		}

		public VariableDeclarationNode(VariableNode[] variables, TypeNode type, object value)
		{
			Variables = variables;
			Type = type;
			Value = value;
		}
	}

	public class VariableDeclarationStatementNode : AstNode
	{
		public VariableDeclarationNode[] Declarations { get; }

		public VariableDeclarationStatementNode(VariableDeclarationNode[] declarations)
		{

		}
	}
}