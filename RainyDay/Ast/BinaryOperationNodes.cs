namespace RainyDay.Ast
{
	public abstract class BinaryOperationNode : AstNode
	{
		public AstNode Left { get; }
		public AstNode Right { get; }

		public BinaryOperationNode(AstNode left, AstNode right)
		{
			Left = left;
			Right = right;
		}
	}

	public class AssignNode : BinaryOperationNode
	{
		public AssignNode(AstNode left, AstNode right) : base(left, right) { }
	}

	public class AddNode : BinaryOperationNode
	{
		public AddNode(AstNode left, AstNode right) : base(left, right) { }
	}

	public class SubtractNode : BinaryOperationNode
	{
		public SubtractNode(AstNode left, AstNode right) : base(left, right) { }
	}

	public class MultiplyNode : BinaryOperationNode
	{
		public MultiplyNode(AstNode left, AstNode right) : base(left, right) { }
	}

    public class DivideNode : BinaryOperationNode
    {
        public DivideNode(AstNode left, AstNode right) : base(left, right) { }
    }
}