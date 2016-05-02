using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace radcompiler
{
    abstract class Expression : Statement
    {
        protected Expression(Token source) : base(source) { }
        public override void Serialize(StringBuilder sb)
        {
            sb.Append(Source.ToString());
        }
    }

    sealed class FunctionCall : Expression
    {
		public readonly Identifier FunctionIdentifier;
		public readonly List<Expression> ArgumentList = new List<Expression>();

        public FunctionCall(Token source, Identifier functionIdentifier) : base(source)
        {
			FunctionIdentifier = functionIdentifier;
        }

        public override void Serialize(StringBuilder sb)
        {
			FunctionIdentifier.Serialize(sb);
			sb.Append("(");
			foreach (var a in ArgumentList)
			{
				a.Serialize(sb);
			}
			sb.Append(")\n");
        }
    }

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

        public override void Serialize(StringBuilder sb)
        {
            sb.Append("(");
            LeftHand.Serialize(sb);
            sb.Append(" " + Operator + " ");
            RightHand.Serialize(sb);
            sb.Append(")");
        }
    }

    sealed class Identifier : Expression
    {
        public readonly string Name;
        public Identifier(IdentifierToken source) : base(source)
        {
            Name = source.Value;
        }
        public override void Serialize(StringBuilder sb)
        {
            sb.Append(Name);
        }
    }

    class Assignment : Statement
    {
        public readonly Identifier Variable;
        public readonly Expression Value;
        public Assignment(Token source, Identifier variable, Expression value) : base(source)
        {
            Variable = variable;
            Value = value;
        }

        public override void Serialize(StringBuilder sb)
        {
            Variable.Serialize(sb);
            sb.Append(" = ");
            Value.Serialize(sb);
            sb.Append("\n");
        }
    }

}
