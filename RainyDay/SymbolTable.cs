using System;
using System.Collections.Specialized;
using System.Linq;

namespace RainyDay
{
	public class SymbolTable
	{
		private OrderedDictionary _symbols;

		public SymbolTable()
		{
			_symbols = new OrderedDictionary();
		}

		public void Insert(Symbol symbol)
		{
			Console.WriteLine($"Define: {symbol.Name}");
			_symbols.Add(symbol.Name, symbol);
		}

		public virtual T Lookup<T>(string name)
			where T : Symbol
		{
			Console.WriteLine($"Lookup: {name}");
			if (_symbols.Contains(name))
				return _symbols[name] as T;
			return null;
		}

		public override string ToString()
		{
			var result = "Symbols:";
			for (int i = 0; i < _symbols.Count; i++)
				result += $"\n\t{_symbols[i]}";
			return result;
		}
	}
}