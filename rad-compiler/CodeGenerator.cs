using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace radcompiler
{
	class CodeGenerator
	{
		StringBuilder _code = new StringBuilder();
		Compiler _compiler;

		public CodeGenerator(Compiler compiler)
		{
			_compiler = compiler;
		}

		public void Generate()
		{

		}

		void Emit(string s)
		{
			_code.Append(s);
		}

		void Error(Token source, string message)
		{
            var textPos = _compiler.Parser.Lexer.ToTextPosition(source.Position).ToString();
			Console.WriteLine(textPos + " compiler error: " + message);
		}
	}
}
