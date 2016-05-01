using System;
using System.Collections.Generic;
using System.Linq;

namespace radcompiler
{
	abstract class Statement
	{
		
	}

	abstract class Expression: Statement
	{
		
	}

	class Assignment: Statement
	{
		public readonly Identifier Variable;
		public readonly Expression Value;
		public Assignment(Identifier variable, Expression value)
		{
			Variable = variable;
			Value = value;
		}
	}

	class Function
	{
		static int anonymousCounter = 0;

		public readonly string Name;
		public Function(string name)
		{
			Name = name ?? ("anonymous_" + anonymousCounter++);
		}

		public readonly List<Function> Functions = new List<Function>();
	}

	class Parser
	{
		readonly Lexer _lexer;
		readonly Token[] _tokens;

		int _pos = 0;

		readonly Function _root;
		public Function Root { get { return _root; } }

		public Parser (Lexer lexer)
		{
			_lexer = lexer;
			_tokens = lexer.Tokens.ToArray();


			_root = ParseFunctionBody();
		}

		Function ParseFunctionBody(string name = null)
		{
			var f = new Function(name);

			while (!EOF)
			{
				if (Peek(0, "}")) break;

				var fd = ParseFunctionDeclaration();
				if (fd != null)
				{
					f.Functions.Add(fd);
				}
				else
				{
					Error("Unexpected syntax");
					return f;
				}
			}

			return f;
		}

		Function ParseFunctionDeclaration()
		{
			var start = _pos;

			var funcName = Peek(0) as Identifier;
			if (funcName == null) return null;

			var paramList = new List<Token>();
			var p = 1;

			// Optional argument list
			if (Peek(1, "(")) 
			{
				p++;
				if (Peek(p, ")")) 
				{
					p++;
				}
				else while (true)
				{
					var paramName = Peek(p);
					if (!(paramName is Identifier)) return null;
					if (Peek(p+1, ")")) { p+=2; break; }
					if (!Peek(p+1, ",")) return null;
					paramList.Add(paramName);
					p += 2;
				}
			}

			// The curly brace is the hallmark of a function
			if (!Peek(p, "{")) return null;
			p++;

			// Now we know for sure it is a function declaration
			Consume(p);
			var f = ParseFunctionBody(funcName.Value);

			if (!Consume("}"))
			{
				Error("Expected '}'");
			}
			
			return f;
		}




		bool Peek(int offset, string op)
		{
			var paren = Peek(offset) as Operator;
			if (paren == null) return false;
			return paren.Value == op;
		}

		void Consume(int tokenCount=1)
		{
			_pos += tokenCount;
		}

		bool Consume(string op)
		{
			if (Peek(0, op)) 
			{
				Consume();
				return true;
			}
			return false;
		}


		Token Peek(int offset=0)
		{
			if (_pos+offset < _tokens.Length)
				return _tokens[_pos+offset];
			else return null;
		}

		bool EOF
		{
			get 
			{
				return _pos >= _tokens.Length;
			}
		}

		void Error(string message)
		{
			var textPos = _lexer.ToTextPosition(_tokens[_pos].Position).ToString();
			Console.WriteLine(textPos + " syntax error: " + message);
		}


	}
}

