using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace radcompiler
{
    abstract class Statement
    {
        public readonly Token Source;

        protected Statement(Token source)
        {
            Source = source;
        }

        public virtual void Serialize(StringBuilder sb) { }
    }

	sealed class IfStatement : Statement
	{
		public readonly Expression Condition;
		public readonly FunctionBody Body;

		public IfStatement(Token source, Expression condition, FunctionBody body)
				: base(source)
		{
			Condition = condition;
			Body = body;
		}

		public override void Serialize(StringBuilder sb)
		{
			sb.Append("if ");
			Condition.Serialize(sb);
			sb.Append("\n{\n");
			Body.Serialize(sb);
			sb.Append("}");
		}
	}
}
