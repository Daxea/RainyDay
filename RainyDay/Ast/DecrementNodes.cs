namespace RainyDay.Ast
{
	public class PreDecrementNode : AstNode
	{
		public AstNode Expression { get; }

		public PreDecrementNode(AstNode expression)
		{
			Expression = expression;
		}
	}

	public class PostDecrementNode : AstNode
	{
		public AstNode Expression { get; }

		public PostDecrementNode(AstNode expression)
		{
			Expression = expression;
		}
	}
}