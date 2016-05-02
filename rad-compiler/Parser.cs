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
			return null;
		}

        bool PeekOperator(Precedence precedence)
        {
            var t = Peek() as Operator;
            return t != null && t.Precedence >= precedence;
        }

		Expression ParseExpression(Precedence precedence)
		{
			var fc = ParseFunctionCall();
			if (fc != null)
				return fc;

			var leftHand = ParsePrimaryExpression(precedence);
			if (leftHand != null)
			{
                while (PeekOperator(precedence))
                {
					var t = Peek() as BinaryOperator;
					if (t == null)
						return leftHand;
					Consume ();
                    var rightHand = ParseExpression(t.Precedence);
                    leftHand = new BinaryExpression(t, leftHand, t, rightHand);
				}
                return leftHand;
			}
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


		IfStatement ParseIf()
		{
			var t = Peek();
			if (t is Keyword && ((Keyword)t).Value == "if")
			{
				Consume();
				var condition = ParseExpression(Precedence.Parens);
				if (condition != null)
				{
					if (Consume("{")) {
						var fb = ParseFunctionBody ();
						if (fb != null) {
							return new IfStatement (t, condition, fb);
						}
					}
				}
			}
			return null;
		}

		Statement ParseStatement()
		{
			Statement ret = null;
			ret = (Statement)ParseAssignment();
			if (ret != null) return ret;
			ret = (Statement)ParseIf();
			if (ret != null) return ret;
			ret = (Statement)ParseExpression(Precedence.Parens);
			if (ret != null) return ret;
			return ret;
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

		FunctionCall ParseFunctionCall()
		{
			var t = Peek() as IdentifierToken;
			if (t != null)
			{
				var identifier = new Identifier(t);
				var functionCall = new FunctionCall(t, identifier);
				var p = 1;
				if (PeekGroupingOperator(p,"("))
				{
					Consume(p+1);
					if (PeekGroupingOperator(p, ")"))
					{
						return functionCall;
					}
					else while (true)
					{
						var arg = ParseExpression(Precedence.Parens);
						if (arg != null)
						{
							functionCall.ArgumentList.Add(arg);
						}
						if (Consume(",")) continue;
						else if (Consume(")")) return functionCall;
						else
						{
							Error("Expected token \")\" in function call");
							return null;
						};
					}

				}
			}
			return null;
		}

		FunctionDeclaration ParseFunctionDeclaration()
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

			var f = new FunctionDeclaration(funcName.Value,
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

		bool PeekGroupingOperator(int offset, string op)
		{
			var paren = Peek(offset) as GroupingOperator;
			if (paren == null) return false;
			return paren.Value == op;
		}

		bool PeekSeparatorOperator(int offset, string op)
		{
			var paren = Peek(offset) as SeparatorOperator;
			if (paren == null) return false;
			return paren.Value == op;
		}

		Token Peek(int offset=0)
		{
			if (_pos+offset < _tokens.Length)
				return _tokens[_pos+offset];
			else return null;
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
