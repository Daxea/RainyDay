using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using RainyDay.Ast;

namespace RainyDay
{
	public class AstNodeVisitor
	{
		private readonly Dictionary<Type, MethodInfo> _methods;

		private List<Exception> _errors = new List<Exception>();
		public Exception[] Errors => _errors.ToArray();

		public bool IsSuccess => _errors.Count == 0;

		public AstNodeVisitor()
		{
			var type = GetType().GetTypeInfo();
			_methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
				.Where(m => m.Name.StartsWith("Visit") && m.GetParameters().Length == 1)
				.ToDictionary(m => m.GetParameters().First().ParameterType);
		}

		public object Visit(AstNode node)
		{
			if (!_methods.ContainsKey(node.GetType()))
				return null;
			return _methods[node.GetType()].Invoke(this, new[] { node });
		}

		protected void ReportError(Exception error) => _errors.Add(error);

        public void ClearErrors() => _errors.Clear();
	}
}