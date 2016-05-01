using System;
using System.Linq;
using System.Collections.Generic;

namespace radcompiler
{
	enum Precedence
	{
		Parens,

		Assign,
		AddAssign = Assign,
		MinusAssign = Assign,
		MulAssign = Assign,
		DivAssign = Assign,
		ModAssign = Assign,
		BitwiseAndAssign = Assign,
		BitwiseXorAssign = Assign,
		BitwiseOrAssign = Assign,
		ShlAssign = Assign,
		ShrAssign = Assign,

		QuestionMark,

		DoubleQuestionMark,

		LogOr,

		LogAnd,

		BitwiseOr,

		BitwiseXor,

		BitwiseAnd,

		Equal,
		NotEqual = Equal,


		LessThan,
		GreaterThan = LessThan,
		LessOrEqual = LessThan,
		GreaterOrEqual = LessThan,
		Is = LessThan,
		As = LessThan,

		Shl,
		Shr = Shl,

		Plus,
		Minus = Plus,

		Mul,
		Div = Mul,
		Mod = Mul,

		Period,

		UnrecognizedOp = -1
	}




	sealed class Operator: Token
	{
		public readonly string Value;
		public readonly Precedence Precedence;
		public Operator(int pos, int length, string value):base(pos, length)
		{
			Value = value;
			Precedence = GetPrecedenceFromString(value);
		}

		public override string ToString ()
		{
			return Value;
		}

		static Precedence GetPrecedenceFromString(string opString)
		{
			if (opString == "(" || opString == ")") return Precedence.Parens;
			else if (opString == "=") return Precedence.Assign;
			else if (opString == "+=") return Precedence.AddAssign;
			else if (opString == "-=") return Precedence.MinusAssign;
			else if (opString == "*=") return Precedence.MulAssign;
			else if (opString == "/=") return Precedence.DivAssign;
			else if (opString == "%=") return Precedence.ModAssign;
			else if (opString == "&=") return Precedence.BitwiseAndAssign;
			else if (opString == "^=") return Precedence.BitwiseXorAssign;
			else if (opString == "|=") return Precedence.BitwiseOrAssign;
			//else if (opString == "") return Precedence.ShlAssign;
			//else if (opString == "") return Precedence.ShrAssign;
			else if (opString == "?") return Precedence.QuestionMark;
			else if (opString == "??") return Precedence.DoubleQuestionMark;
			//else if (opString == "") return Precedence.LogOr;
			//else if (opString == "") return Precedence.LogAnd;
			else if (opString == "|") return Precedence.BitwiseOr;
			else if (opString == "^") return Precedence.BitwiseXor;
			else if (opString == "&") return Precedence.BitwiseAnd;
			else if (opString == "==") return Precedence.Equal;
			else if (opString == "!=") return Precedence.NotEqual;
			else if (opString == "<") return Precedence.LessThan;
			else if (opString == ">") return Precedence.GreaterThan;
			else if (opString == "<=") return Precedence.LessOrEqual;
			else if (opString == ">=") return Precedence.GreaterOrEqual;
			//else if (opString == "is") return Precedence.Is;
			//else if (opString == "as") return Precedence.As;
			else if (opString == "<<") return Precedence.Shl;
			else if (opString == ">>") return Precedence.Shr;
			else if (opString == "+") return Precedence.Plus;
			else if (opString == "-") return Precedence.Minus;
			else if (opString == "*") return Precedence.Mul;
			else if (opString == "/") return Precedence.Div;
			else if (opString == "%") return Precedence.Mod;
			else if (opString == ".") return Precedence.Period;

			else return Precedence.UnrecognizedOp;
		}
	}
}
