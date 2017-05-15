using System;
using System.Collections.Generic;
using System.IO;

namespace RainyDay.Interpreter
{
	class Program
	{
		static void Main(string[] args)
		{
			while (true)
			{
				Ast.AstNode tree;

				Console.Write("@> ");
				var input = Console.ReadLine();
				if (string.IsNullOrEmpty(input))
					continue;
				var isSymbolMode = input.Contains("-s");
				if (input.ToLowerInvariant().StartsWith("run"))
				{
					var lineArgs = input.Split('-');
					var path = lineArgs[lineArgs.Length - 1];
					if (!path.Contains(":"))
						path = $@"C:\RainyDay\{path}";

                    input = File.ReadAllText(path);

					var lexer = new Lexer(input);
					var parser = new Parser(lexer);
					tree = parser.ParseScript();
				}
				else if ("cls".Equals(input.ToLowerInvariant()))
				{
					Console.Clear();
					continue;
				}
				else if ("quit".Contains(input.ToLowerInvariant()))
					break;
				else
				{
					var lexer = new Lexer(input);
					var parser = new Parser(lexer);
					tree = parser.ParseStatement();
				}

				if (isSymbolMode)
				{
					var tableBuilder = new SymbolTableBuilder();
					tableBuilder.Visit(tree);
					Console.WriteLine($"{tableBuilder}\n");
				}

				// TODO: Run the script interpretor once it is implemented...

				Pause();
			}
		}

		static void Pause()
		{
			Console.WriteLine("\nPress any key to continue...");
			Console.ReadKey();
		}
	}
}