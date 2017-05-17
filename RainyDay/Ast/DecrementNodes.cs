namespace RainyDay.Ast
{
	public class PreDecrementNode : AstNode
	{
		public VariableNode Variable { get; }

		public PreDecrementNode(VariableNode variable)
		{
			Variable = variable;
		}
	}

	public class PostDecrementNode : AstNode
	{
		public VariableNode Variable { get; }

		public PostDecrementNode(VariableNode variable)
		{
			Variable = variable;
		}
	}
}