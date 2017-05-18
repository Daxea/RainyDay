using System;
using System.Collections.Generic;

using RainyDay.Ast;

namespace RainyDay.Interpreter
{
    public class Interpreter : AstNodeVisitor
    {
        #region Memory

        private Dictionary<string, VariableInfo> GlobalVariables { get; } = new Dictionary<string, VariableInfo>();

        private class VariableInfo
        {
            public string Type { get; }
            public object Value { get; set; }

            public VariableInfo(string type)
            {
                Type = type;
                Value = null;
            }

            public VariableInfo(string type, object value)
            {
                Type = type;
                Value = value;
            }
        }

        #endregion

        public void Interpret(AstNode tree) => Visit(tree);

        private void Visit(BlockNode node)
        {
            foreach (var child in node.Children)
                Visit(child);
        }

        private void Visit(VariableDeclarationNode node)
        {
            foreach (var variable in node.Variables)
            {
                if (GlobalVariables.ContainsKey(variable.Name))
                {
                    ReportError(new Exception($"Variable \"{variable.Name}\" already declared in global scope!"));
                    continue;
                }
                var type = node.Type.Name;
                var expression = Visit(node.Expression);
                if (node.Type.IsInferred)
                {
                    if (expression == null)
                    {
                        ReportError(new Exception("Cannot infer variable type without a value!"));
                        return;
                    }
                    type = expression.GetType().Name;
                }
                GlobalVariables.Add(variable.Name, new VariableInfo(type, Visit(node.Expression)));
            }
        }

        private void Visit(AssignNode node)
        {
            if (!(node.Left is VariableNode))
            {
                ReportError(new InvalidOperationException("You can only assign values to variables!"));
                return;
            }

            var variableNode = (node.Left as VariableNode);
            if (!GlobalVariables.ContainsKey(variableNode.Name))
            {
                ReportError(new InvalidOperationException($"Variable \"{variableNode.Name}\" has not been declared!"));
                return;
            }

            var variable = GlobalVariables[variableNode.Name];
            var expression = Visit(node.Right);
            if (expression.GetType().Name != variable.Type)
            {
                ReportError(new Exception($"Type Mismatch! Data of type \"{expression.GetType().Name}\" cannot be assigned to " +
                    $"variable \"{variableNode.Name}\" of type \"{variable.Type}\"!"));
                return;
            }
            variable.Value = expression;
        }

        private object Visit(UnaryOperationNode node)
        {
            var value = Visit(node.Expression);
            if (node.Operation.Type == Tokens.Subtract)
            {
                if (value is int)
                    return -((int)value);
                if (value is float)
                    return -((float)value);
                ReportError(new Exception("You can only apply unary operations (+,-) to numeric types!"));
            }
            else if (node.Operation.Type == Tokens.Not)
            {
                if (value is bool)
                    return !((bool)value);
                ReportError(new Exception("You can only apply unary operations (!) to boolean types!"));
            }
            return null;
        }

        private object Visit(VariableNode node)
        {
            if (!GlobalVariables.ContainsKey(node.Name))
            {
                ReportError(new Exception($"Variable {node.Name} has not been declared!"));
                return null;
            }
            return GlobalVariables[node.Name].Value;
        }

        #region Increment/Decrement

        private object Visit(PreIncrementNode node)
        {
            if (node.Variable == null)
            {
                ReportError(new Exception("You can only apply inc/dec operations (++,--) on variables!"));
                return null;
            }
            if (!GlobalVariables.ContainsKey(node.Variable.Name))
            {
                ReportError(new Exception($"Variable {node.Variable.Name} has not been declared!"));
                return null;
            }
            dynamic variable = GlobalVariables[node.Variable.Name];
            if (!Tokens.IsNumberType(variable.Type))
            {
                ReportError(new Exception("You can only apply inc/dec operations (++,--) to numeric types!"));
                return null;
            }
            GlobalVariables[node.Variable.Name].Value = variable.Value + 1;
            return GlobalVariables[node.Variable.Name].Value;
        }

        private void Visit(PreDecrementNode node)
        {

        }

		private void Visit(IncrementByNode node)
		{
			var variable = (node.Left as VariableNode);
			dynamic value = Visit(node.Right);
			dynamic originalValue = GlobalVariables[variable.Name].Value;
			if (originalValue.GetType() == value.GetType())
				GlobalVariables[variable.Name].Value = originalValue + value;
		}

		private void Visit(DecrementByNode node)
		{
			var variable = (node.Left as VariableNode);
			dynamic value = Visit(node.Right);
			dynamic originalValue = GlobalVariables[variable.Name].Value;
			if (originalValue.GetType() == value.GetType())
				GlobalVariables[variable.Name].Value = originalValue - value;
		}

		private void Visit(MultiplyByNode node)
		{
			var variable = (node.Left as VariableNode);
			dynamic value = Visit(node.Right);
			dynamic originalValue = GlobalVariables[variable.Name].Value;
			if (originalValue.GetType() == value.GetType())
				GlobalVariables[variable.Name].Value = originalValue * value;
		}

		private void Visit(DivideByNode node)
		{
			var variable = (node.Left as VariableNode);
			dynamic value = Visit(node.Right);
			dynamic originalValue = GlobalVariables[variable.Name].Value;
			if (originalValue.GetType() == value.GetType())
				GlobalVariables[variable.Name].Value = originalValue / value;
		}

		#endregion
		#region Binary Operations

		private object Visit(AddNode node)
        {
            dynamic left = Visit(node.Left);
            dynamic right = Visit(node.Right);

            try
            {
                return left + right;
            }
            catch (Exception error)
            {
                ReportError(error);
                return null;
            }
        }

        private object Visit(SubtractNode node)
        {
            dynamic left = Visit(node.Left);
            dynamic right = Visit(node.Right);

            try
            {
                return left - right;
            }
            catch (Exception error)
            {
                ReportError(error);
                return null;
            }
        }

        private object Visit(MultiplyNode node)
        {
            dynamic left = Visit(node.Left);
            dynamic right = Visit(node.Right);

            try
            {
                return left * right;
            }
            catch (Exception error)
            {
                ReportError(error);
                return null;
            }
        }

        private object Visit(DivideNode node)
        {
            dynamic left = Visit(node.Left);
            dynamic right = Visit(node.Right);

            try
            {
                return left / right;
            }
            catch (Exception error)
            {
                ReportError(error);
                return null;
            }
        }

        #endregion
        #region Primitives

        private bool Visit(BooleanNode node) => node.Value;
        private int Visit(IntegerNode node) => node.Value;
        private float Visit(SingleNode node) => node.Value;
        private string Visit(StringNode node) => node.Value;
        private char Visit(CharacterNode node) => node.Value;

        #endregion
        #region NoOp

        private object Visit(NoOpNode node) => null;

        #endregion
        #region ToString

        private const int CellSize = -16;

        public override string ToString()
        {
            var output = $"\n\t{new string('_', Math.Abs((CellSize*3) - 3))}\n\t{"Type",CellSize}| {"Variable",CellSize}| Value";
            foreach (var variable in GlobalVariables)
                output += $"\n\t{variable.Value.Type,CellSize}| {variable.Key,CellSize}| {variable.Value.Value}";
            return output;
        }

        #endregion
    }
}