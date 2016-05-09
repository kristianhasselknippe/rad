using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace radcompiler
{
    class FunctionBody
    {
        public readonly IList<FunctionDeclaration> Functions = new List<FunctionDeclaration>();
        public readonly IList<Statement> Statements = new List<Statement>();

        public FunctionBody() { }

        public void Serialize(StringBuilder sb)
        {
			sb.Append("{\n");
            foreach (var f in Functions)
                f.Serialize(sb);
            foreach (var s in Statements)
                s.Serialize(sb);
			sb.Append("\n}\n");
        }
    }

    class FunctionDeclaration
    {
        static int anonymousCounter = 0;

        public readonly string
			Name;
        public readonly IList<IdentifierToken> ParameterList;
        public readonly FunctionBody FunctionBody;

        public FunctionDeclaration(string name, IList<IdentifierToken> paramList, FunctionBody body)
        {
            Name = name ?? ("anonymous_" + anonymousCounter++);
            ParameterList = paramList;
            FunctionBody = body;
        }

        public void Serialize(StringBuilder sb)
        {

            sb.Append(Name + "(");
            for (var i = 0; i < ParameterList.Count; i++)
            {
                sb.Append(ParameterList[i].ToString());
                if (i < ParameterList.Count - 1)
                    sb.Append(",");
            }
            sb.Append(")");
            sb.Append("\n");
            FunctionBody.Serialize(sb);
        }
    }

}
