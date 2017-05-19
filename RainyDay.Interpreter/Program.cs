using System;
using System.Collections.Generic;
using System.IO;

namespace RainyDay.Interpreter
{
	class Program
	{
		static void Main(string[] args)
		{
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = "RainDay Interpreter";
            Console.SetWindowSize((int)(Console.LargestWindowWidth * 0.75f), (int)(Console.LargestWindowHeight * 0.75f));

            var interpreter = new Interpreter();

			while (true)
			{
                interpreter.ClearErrors();
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
                else if ("restart".Equals(input.ToLowerInvariant()))
                {
                    interpreter = new Interpreter();
                    continue;
                }
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

                interpreter.Interpret(tree);
                if (interpreter.Errors.Length > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    foreach (var error in interpreter.Errors)
                        Console.WriteLine($"\t{error.Message}");
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.WriteLine($"\n{interpreter}");
			}
		}
	}
}