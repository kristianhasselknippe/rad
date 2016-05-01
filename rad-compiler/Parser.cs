using System;
using System.Collections.Generic;
using System.Linq;

namespace radcompiler
{
	abstract class Statement
	{
		public readonly Token Source;

		protected Statement(Token source)
		{
			Source = source;
		}
	}

	abstract class Expression: Statement
	{
		protected Expression(Token source) : base(source) { }
	}

	/*class FunctionCall : Expression(Token source) : base(source)
	{

	}*/

	sealed class BinaryExpression : Expression
	{
		public readonly Expression LeftHand;
		public readonly Expression RightHand;
		public readonly Operator Operator;
		public BinaryExpression(Token source, Expression lh, Operator op, Expression rh) : base(source)
		{
			LeftHand = lh;
			RightHand = rh;
			Operator = op;
		}
	}

	sealed class Identifier : Expression
	{
		public Identifier(IdentifierToken source) : base(source) { }
	}

	abstract class Constant : Expression
	{
		protected Constant(Token source) : base(source) { }
	}

	sealed class IntConstant : Constant
	{
		public IntConstant(Int source) : base(source) { }
	}

	sealed class DoubleConstant : Constant
	{
		public DoubleConstant(Double source) : base(source) { }
	}

	sealed class StringConstant : Constant
	{
		public StringConstant(String source) : base(source) { }
	}

	class Assignment: Statement
	{
		public readonly Identifier Variable;
		public readonly Expression Value;
		public Assignment(Token source, Identifier variable, Expression value) : base(source)
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
		public readonly List<Statement> Statements = new List<Statement>();
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



		Expression ParsePrimaryExpression(Precedence precedence)
		{
			var c = Peek ();
			var op = c as Operator;

			if (op != null && op.Value == "(")
			{
				Consume();
				var e = ParseExpression(Precedence.Parens);
				if (!Consume(")"))
				{
					Error("Expected )");
				}
				return e;
			}
			else if (c is Int)
				return new IntConstant((Int)c);
			else if (c is Double)
				return new DoubleConstant((Double)c);
			else if (c is String)
				return new StringConstant((String)c);
			else if (c is IdentifierToken)
				return new Identifier((IdentifierToken)c);
			else
				Error("Expected primary expression");
			return null;
		}

		Expression ParseExpression(Precedence precedence)
		{
			var leftHand = ParsePrimaryExpression(precedence);
			if (leftHand != null)
			{
				Consume ();
				var t = Peek() as Operator;
				if (t != null && t.Precedence > precedence)
				{
					Consume();
					var rightHand = ParseExpression(t.Precedence);
					return new BinaryExpression(t, leftHand, t, rightHand);
				}
				else
				{
					return leftHand;
				}
			}
			Error("Expected expression");
			return null;
		}


		Assignment ParseAssignment()
		{
			var t = Peek();
			if (t is IdentifierToken)
			{
				var identifier = new Identifier((IdentifierToken)t);
				if (Peek(1,"="))
				{
					Consume(2);
					var e = ParseExpression(Precedence.Parens);
					if (e != null)
					{
						return new Assignment(t, identifier, e);
					}
					else
					{
						return null;
					}
				}
				else
				{
					return null;
				}
			}
			return null;
		}

		Statement ParseStatement()
		{
			return (Statement)ParseAssignment() ??
				(Statement)ParseExpression(Precedence.Parens);
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
					var s = ParseStatement();
					if (s != null)
					{
						f.Statements.Add(s);
					}
					else
					{
						Error("Expected statement, function declaration or }");
					}
				}
			}

			return f;
		}

		Function ParseFunctionDeclaration()
		{
			var start = _pos;

			var funcName = Peek(0) as IdentifierToken;
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
					if (!(paramName is IdentifierToken)) return null;
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
