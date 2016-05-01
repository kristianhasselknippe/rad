using System;

namespace radcompiler
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var code = System.IO.File.ReadAllText(args[0]);

			var lexer = new Lexer(code);
			if (lexer.ErrorCount > 0) return;

			var parser = new Parser(lexer);

		}
	}
}
