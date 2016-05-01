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
        public IntConstant(Int source) : base(source) { }
        public override string ToString()
        {
            return Source.ToString();
        }
    }

    sealed class DoubleConstant : Constant
    {
        public DoubleConstant(Double source) : base(source) { }
        public override string ToString()
        {
            return Source.ToString() + "d";
        }
    }

    sealed class StringConstant : Constant
    {
        public StringConstant(String source) : base(source) { }
        public override string ToString()
        {
            return "\"" + Source.ToString() + "\"";
        }
    }
}
