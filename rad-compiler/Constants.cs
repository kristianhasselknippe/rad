using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace radcompiler
{
    abstract class Constant : Expression
    {
        protected Constant(Token source) : base(source) { }
    }

    sealed class IntConstant : Constant
    {
		public readonly int IntValue;
        public IntConstant(Int source) : base(source)
		{
			IntValue = source.Value;
		}

        public override string ToString()
        {
            return Source.ToString();
        }
    }

    sealed class DoubleConstant : Constant
    {
		public readonly double DoubleValue;

        public DoubleConstant(Double source) : base(source)
		{
			DoubleValue = source.Value;
		}

        public override string ToString()
        {
            return Source.ToString() + "d";
        }
    }

    sealed class StringConstant : Constant
    {
		public readonly string StringValue;

        public StringConstant(String source) : base(source)
		{
			StringValue = source.Value;
		}

        public override string ToString()
        {
            return "\"" + Source.ToString() + "\"";
        }
    }
}
