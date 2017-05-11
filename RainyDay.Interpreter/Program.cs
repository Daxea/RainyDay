using System;
using System.Collections.Generic;
using System.Linq;

namespace RainyDay.Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.Write("@> ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    continue;
                if (input.Equals("quit"))
                    return;
                var lexer = new Lexer(input);
                Token token;
                do
                {
                    token = lexer.GetNextToken();
                    if (token == null || token.Type == Tokens.ScriptEnd)
                    {
                        foreach (var error in lexer.Errors)
                            Console.WriteLine(error.Message);
                        break;
                    }
                    if (token.Value != null)
                        Console.WriteLine($"{token.Type}: {token.Value}");
                    else
                        Console.WriteLine(token.Type);
                }
                while (true);
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }
}