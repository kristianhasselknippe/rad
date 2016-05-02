using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace radcompiler
{
	class Compiler
	{
		StringBuilder _code = new StringBuilder();

		readonly Parser _parser;

		public Compiler(Parser parser)
		{
			_parser = parser;
			Compile();
		}

		void Emit(string s)
		{
			_code.Append(s);
		}

		void Compile()
		{

		}
	}
}
