namespace RainyDay.Ast
{
	public class PreIncrementNode : AstNode
	{
		public AstNode Expression { get; }

		public PreIncrementNode(AstNode expression)
		{
			Expression = expression;
		}
	}

	public class PostIncrementNode : AstNode
	{
		public AstNode Expression { get; }

		public PostIncrementNode(AstNode expression)
		{
			Expression = expression;
		}
	}
}