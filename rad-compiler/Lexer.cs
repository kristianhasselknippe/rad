using System;
using System.Linq;
using System.Collections.Generic;

namespace radcompiler
{
	abstract class Token
	{
		public readonly int Position;
		public readonly int Length;

		public Token(int pos, int length)
		{
			Position = pos;
			Length = length;
		}
	}

	sealed class Int: Token
	{
		public readonly int Value;
		public Int(int pos, int len, int value): base(pos, len)
		{
			Value = value;
		}
		public override string ToString ()
		{
			return "(Int) " + Value;
		}
	}

	sealed class Double: Token
	{
		public readonly double Value;
		public Double(int pos, int len, double value): base(pos, len)
		{
			Value = value;
		}
		public override string ToString ()
		{
			return "(Double) " + Value;
		}
	}

	sealed class IdentifierToken: Token
	{
		public readonly string Value;
		public IdentifierToken(int pos, int len, string value): base(pos, len)
		{
			Value = value;
		}
		public override string ToString ()
		{
			return "(Identifier) " + Value;
		}
	}

	sealed class String: Token
	{
		public readonly string Value;
		public String(int pos, int len, string value): base(pos, len)
		{
			Value = value;
		}
		public override string ToString ()
		{
			return "(String) " + Value;
		}
	}

	struct TextPosition
	{
		public readonly int Offset;
		public readonly int Line;
		public readonly int Column;
		public TextPosition(int offset, int line, int column)
		{
			Offset = offset;
			Line = line;
			Column = column;
		}

		public override string ToString ()
		{
			return Line +":" +Column;
		}
	}

	class Lexer
	{
		public readonly string Code;

		public IEnumerable<Token> Tokens
		{
			get { return _tokens; }
		}

		int _pos = 0;
		int _line = 1;
		int _col = 0;
		List<Token> _tokens = new List<Token>();


		public Lexer (string code)
		{
			Code = code;
			Lex();
		}

		void Lex()
		{
			while (!EOF)
			{
				var c = Peek();
				if (char.IsWhiteSpace(c)) Consume();
				else if (char.IsNumber(c)) LexNumber();
				else if (char.IsLetter(c) || c == '_') LexIdentifier();
				else if (c == '"') LexString();
				else if (LexOperator()) continue;
				else
				{
					Error("Unrecognized token " + c);
				}
			}
		}

		void LexNumber()
		{
			int start = _pos;
			while (!EOF)
			{
				var c = Peek();
				if ( char.IsNumber(c) || (c == '.'))
				{
					Consume();
					continue;
				}

				break;
			}
			var len = _pos-start;
			var v = Code.Substring(start, len);
			if (v.Contains ('.'))
			{
				var d = double.Parse (v, System.Globalization.CultureInfo.InvariantCulture);
				Emit (new Double (start, len, d));
			}
			else
			{
				var i = int.Parse (v, System.Globalization.CultureInfo.InvariantCulture);
				Emit (new Int (start, len, i));
			}
		}

		void LexString()
		{
			int start = _pos;
			bool escape = false;

			var sb = new System.Text.StringBuilder();

			Consume(); // initial quote

			while (!EOF)
			{
				var c = Peek();
				if (escape)
				{
					escape = false;
					switch(c)
					{
					case 'n': sb.Append("\n"); break;
					case 't': sb.Append("\t"); break;
					default: sb.Append(c.ToString()); break;
					}

					Consume();
					continue;
				}
				else if (c == '"')
				{
					Consume();
					break;
				}
				else if (c == '\\')
				{
					escape = true;
					Consume();
					continue;
				}
				else
				{
					sb.Append(c.ToString());
					Consume();
					continue;
				}
			}

			Emit(new String(start, _pos-start, sb.ToString()));
		}

		void LexIdentifier()
		{
			int start = _pos;
			while (!EOF)
			{
				var c = Peek();
				if ( char.IsLetter(c) ||
				    (_pos != start && char.IsNumber(c)) ||
					(c == '_'))
				{
					Consume();
					continue;
				}

				break;
			}
			var len = _pos-start;
			Emit(new IdentifierToken(start, len, Code.Substring(start, len)));
		}

		static string[] tripleOps = new[] {
			"&&=", "||="
		};

		static string[] doubleOps = new[] {
			"++", "--", "==", "&&", "||", "+=", "-=", "*=", "/=", "&=", "|=", "^=", "%="
		};

		static string[] singleOps = new[] {
			"+", "-", "*", "/", "&", "|", "^", "=", "%", "(", ")", "{", "}", "[", "]", "?", ":", ",", ".", "!"
		};

		bool LexOperator()
		{
			var start = _pos;
			var c = Consume();

			if (_pos < Code.Length-1)
			{
				var c2 = Peek();
				var c3 = Peek(1);
				var op = c.ToString() + c2.ToString() + c3.ToString();

				if (tripleOps.Contains(op))
				{
					Consume();
					Consume();
					Emit(new Operator(start, 3, op));
					return true;
				}
			}

			if (!EOF)
			{
				var c2 = Peek();
				var op = c.ToString() + c2.ToString();

				if (doubleOps.Contains(op))
				{
					Consume();
					Emit(new Operator(start, 2, op));
					return true;
				}
			}

			if (singleOps.Contains(c.ToString()))
			{
				Emit(new Operator(start, 1, c.ToString()));
				return true;
			}

			return false;
		}

		void Emit(Token t)
		{
			_tokens.Add(t);
		}

		bool EOF { get { return _pos == Code.Length; } }

		char Consume()
		{
			return Code[_pos++];
		}

		public TextPosition ToTextPosition(int pos)
		{
			int line = 1;
			int col = 1;
			for (int i = 0; i < pos; i++)
			{
				if (Code[i] == '\n')
				{
					line++;
					col = 1;
				}
				else
				{
					col++;
				}
			}
			return new TextPosition(pos, line, col);
		}

		char Peek(int offset=0)
		{
			return Code[_pos+offset];
		}

		public int ErrorCount { get; private set; }

		void Error(string message)
		{
			ErrorCount = ErrorCount +1;
			Console.WriteLine(ToTextPosition(_pos) + " Lexer error: " + message);
		}
	}
}
