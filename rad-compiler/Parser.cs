using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace radcompiler
{
	
	class Parser
	{
		readonly Lexer _lexer;
		readonly Token[] _tokens;

		int _pos = 0;

		readonly FunctionBody _root;
		public FunctionBody Root { get { return _root; } }

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
			{
				Consume();
				return new IntConstant((Int)c);
			}
			else if (c is Double)
			{
				Consume();
				return new DoubleConstant((Double)c);
			}
			else if (c is String)
			{
				Consume();
				return new StringConstant((String)c);
			}
			else if (c is IdentifierToken)
			{
				Consume();
				return new Identifier((IdentifierToken)c);
			}
			else
				Error("Expected primary expression");
			return null;
		}

        bool PeekOperator(Precedence precedence)
        {
            var t = Peek() as Operator;
            return t != null && t.Precedence >= precedence;
        }

		Expression ParseExpression(Precedence precedence)
		{
			var leftHand = ParsePrimaryExpression(precedence);
			if (leftHand != null)
			{
                while (PeekOperator(precedence))
                {
                    var t = Consume() as Operator;
                    var rightHand = ParseExpression(t.Precedence);
                    leftHand = new BinaryExpression(t, leftHand, t, rightHand);
				}
                return leftHand;
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

		FunctionBody ParseFunctionBody()
		{
			var fb = new FunctionBody();
			while (!EOF)
			{

				if (Peek(0, "}")) break;

				var fd = ParseFunctionDeclaration();
				if (fd != null)
				{
					fb.Functions.Add(fd);
				}
				else
				{
					var s = ParseStatement();
					if (s != null)
					{
						fb.Statements.Add(s);
					}
					else
					{

						Error("Expected statement, function declaration or }");
					}
				}
			}

			return fb;
		}

		Function ParseFunctionDeclaration()
		{
			var start = _pos;

			var funcName = Peek(0) as IdentifierToken;
			if (funcName == null) return null;

			var paramList = new List<IdentifierToken>();
			var p = 1;

			// Optional argument list
			if (Peek(p, "("))
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
					if (Peek(p+1, ")"))
					{
						paramList.Add((IdentifierToken)paramName);
						p+=2;
						break;
					}
					if (!Peek(p+1, ",")) return null;
					paramList.Add((IdentifierToken)paramName);
					p += 2;
				}
			}


			// The curly brace is the hallmark of a function
			if (!Peek(p, "{")) return null;
			p++;

			// Now we know for sure it is a function declaration
			Consume(p);

			var f = new Function(funcName.Value,
								 paramList,
								 ParseFunctionBody());

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

		Token Consume(int tokenCount=1)
		{
            var t = _tokens[_pos];
			_pos += tokenCount;
            return t;
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

		public override string ToString()
	   	{
			var sb = new StringBuilder();
			Root.Serialize(sb);
			return sb.ToString();
		}
	}
}
