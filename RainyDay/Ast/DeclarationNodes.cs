namespace RainyDay.Ast
{
	public class DeclarationNode : AstNode { }

	public class VariableDeclarationNode : DeclarationNode
	{
		public VariableNode[] Variables { get; }
		public TypeNode Type { get; }
		public AstNode Expression { get; }

		public VariableDeclarationNode(VariableNode[] variables, TypeNode type)
		{
			Variables = variables;
			Type = type;
			Expression = new NoOpNode();
		}

		public VariableDeclarationNode(VariableNode[] variables, TypeNode type, AstNode value)
		{
			Variables = variables;
			Type = type;
			Expression = value;
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