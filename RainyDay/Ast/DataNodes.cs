namespace RainyDay.Ast
{
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

    public class BooleanNode : DataNode<bool>
    {
        public BooleanNode(bool value) : base(value) { }
    }
}