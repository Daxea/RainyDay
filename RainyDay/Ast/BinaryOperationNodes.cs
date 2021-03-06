﻿namespace RainyDay.Ast
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

	public class IncrementByNode : BinaryOperationNode
	{
		public IncrementByNode(AstNode left, AstNode right) : base(left, right) { }
	}

	public class DecrementByNode : BinaryOperationNode
	{
		public DecrementByNode(AstNode left, AstNode right) : base(left, right) { }
	}

	public class MultiplyByNode : BinaryOperationNode
	{
		public MultiplyByNode(AstNode left, AstNode right) : base(left, right) { }
	}

	public class DivideByNode : BinaryOperationNode
	{
		public DivideByNode(AstNode left, AstNode right) : base(left, right) { }
	}

    public class GreaterThanNode : BinaryOperationNode
    {
        public GreaterThanNode(AstNode left, AstNode right) : base(left, right) { }
    }

    public class GreaterThanOrEqualToNode : BinaryOperationNode
    {
        public GreaterThanOrEqualToNode(AstNode left, AstNode right) : base(left, right) { }
    }

    public class LessThanNode : BinaryOperationNode
    {
        public LessThanNode(AstNode left, AstNode right) : base(left, right) { }
    }

    public class LessThanOrEqualToNode : BinaryOperationNode
    {
        public LessThanOrEqualToNode(AstNode left, AstNode right) : base(left, right) { }
    }

    public class EqualToNode : BinaryOperationNode
    {
        public EqualToNode(AstNode left, AstNode right) : base(left, right) { }
    }

    public class NotEqualToNode : BinaryOperationNode
    {
        public NotEqualToNode(AstNode left, AstNode right) : base(left, right) { }
    }
}