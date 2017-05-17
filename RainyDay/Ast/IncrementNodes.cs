namespace RainyDay.Ast
{
	public class PreIncrementNode : AstNode
	{
		public VariableNode Variable { get; }

		public PreIncrementNode(VariableNode variable)
		{
			Variable = variable;
		}
	}

	public class PostIncrementNode : AstNode
	{
		public VariableNode Variable { get; }

		public PostIncrementNode(VariableNode variable)
		{
			Variable = variable;
		}
	}
}