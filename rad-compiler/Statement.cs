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
}
