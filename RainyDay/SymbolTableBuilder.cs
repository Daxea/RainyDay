using System;
using System.Collections.Generic;
using System.Linq;

using RainyDay.Ast;

namespace RainyDay
{
	public class SymbolTableBuilder : AstNodeVisitor
	{
		private SymbolTable _symbolTable;

		public SymbolTableBuilder()
		{
			_symbolTable = new SymbolTable();

			CreatePrimitiveTypeSymbols();
		}

		private void CreatePrimitiveTypeSymbols()
		{
			_symbolTable.Insert(new TypeSymbol(Tokens.Void));
			_symbolTable.Insert(new TypeSymbol(Tokens.Int32));
			_symbolTable.Insert(new TypeSymbol(Tokens.Single));
			_symbolTable.Insert(new TypeSymbol(Tokens.String));
			_symbolTable.Insert(new TypeSymbol(Tokens.Character));
		}

		private void Visit(ScriptNode node)
		{
			Visit(node.Imports);
			Visit(node.Module);
			foreach (var typeDefinition in node.Types)
				Visit(typeDefinition);
		}

		private void Visit(ImportNode node)
		{
			foreach (var module in node.Modules)
			Console.WriteLine($"Import {module}");
		}

		private void Visit(ModuleNode node)
		{
			Console.WriteLine($"Module {node.Name}");
		}

		private void Visit(TypeDefinitionNode node)
		{
			Console.WriteLine($"Define Type: {node.Name}");
			Visit(node.TypeBody);
		}

		private void Visit(TypeBodyNode node)
		{
			foreach (var member in node.Members)
				Visit(member);
		}

		private void Visit(FunctionNode node)
		{
			Console.WriteLine($"Function: {node.Name} returns {node.Type.Name}");
			foreach (var parameter in node.Parameters)
				Visit(parameter);
			Visit(node.Body);
		}

		private void Visit(ParameterNode node)
		{
			Console.WriteLine($"\tParamater: {node.Name} : {node.Type.Name}");
		}

		private void Visit(PropertyNode node)
		{
			Console.WriteLine($"Property: {node.Name} : {node.Type.Name}");
			if (node.Getter != null)
				Visit(node.Getter);
			if (node.Setter != null)
				Visit(node.Setter);
		}

		private void Visit(PropertyBodyNode node)
		{
			Visit(node.Body);
		}

		private void Visit(FieldNode node)
		{
			Console.WriteLine($"Field {node.Name} : {node.Type.Name} = {node.Value}");
		}

		private void Visit(BlockNode node)
		{
			foreach (var child in node.Children)
				Visit(child);
		}

		private void Visit(IntegerNode node) { }

		private void Visit(SingleNode node) { }

		private void Visit(CharacterNode node) { }

		private void Visit(StringNode node) { }

		private void Visit(UnaryOperationNode node) => Visit(node.Expression);

		private void Visit(PreIncrementNode node) => Visit(node.Expression);

		private void Visit(PreDecrementNode node) => Visit(node.Expression);

		private void Visit(PostIncrementNode node) => Visit(node.Expression);

		private void Visit(PostDecrementNode node) => Visit(node.Expression);

		private void Visit(BinaryOperationNode node)
		{
			Visit(node.Left);
			Visit(node.Right);
		}

		private void Visit(VariableDeclarationNode node)
		{
			var typeName = node.Type.Name;
			var typeSymbol = _symbolTable.Lookup<TypeSymbol>(typeName);
			foreach (var variable in node.Variables)
			{
				//var varSymbol = _symbolTable.Lookup<VariableSymbol>(variable.Name);
				var varSymbol = new VariableSymbol(variable.Name, typeSymbol);
				_symbolTable.Insert(varSymbol);
			}
		}

		private void Visit(VariableNode node)
		{
			var name = node.Name;
			var symbol = _symbolTable.Lookup<VariableSymbol>(name);
			if (symbol == null)
				ReportError(new Exception($"Variable {name} has not been declared!"));
		}

		public override string ToString() => _symbolTable.ToString();
	}
}