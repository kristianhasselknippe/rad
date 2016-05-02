using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace radcompiler
{
	class Compiler
	{
		StringBuilder _code = new StringBuilder();

		readonly Parser _parser;

		public Compiler(Parser parser)
		{
			_parser = parser;
			Compile();
		}

		void Emit(string s)
		{
			_code.Append(s);
		}

		Dictionary<string,FunctionDeclaration> GenericFunctions =
			new Dictionary<string,FunctionDeclaration>();

		List<Identifier> variables = new List<Identifier>();

		void CompileAssignment(Assignment a)
		{
			if (GenericFunctions.ContainsKey(a.Variable.Name))
			{
				Error(a.Source, "An identifier with the name " + a.Variable.Name + " already exists");
			}
			CompileExpression(a.Value);
			variables.Add(a.Variable);
		}

		void CompileBinaryExpression(BinaryExpression be)
		{
		}

		void CompileIdentifier(Identifier i)
		{
			if (!variables.Contains(i))
			{
				Error(i.Source, "No variable named " + i.Name + " accessible.");
			}
		}

		void CompileIfStatement(IfStatement i)
		{

		}

		void CompileFunctionCall(FunctionCall fc)
		{

		}

		void CompileConstant(Constant c)
		{

		}

		void CompileExpression(Expression e)
		{
			if (e is BinaryExpression)
				CompileBinaryExpression((BinaryExpression)e);
			else if (e is Identifier)
				CompileIdentifier((Identifier)e);
			else if (e is Constant)
				CompileConstant((Constant)e);
		}

		void CompileStatement(Statement statement)
		{
			if (statement is IfStatement)
				CompileIfStatement((IfStatement)statement);
			else if (statement is FunctionCall)
				CompileFunctionCall((FunctionCall)statement);
			else if (statement is Assignment)
				CompileAssignment((Assignment)statement);
			else if (statement is Expression)
				CompileExpression((Expression)statement);
		}

		void CompileFunctionBody(FunctionBody function)
		{
			foreach (var f in function.Functions)
				GenericFunctions.Add(f.Name,f);
			foreach (var s in function.Statements)
				CompileStatement(s);
		}

		void Compile()
		{
			CompileFunctionBody(_parser.Root);
		}

		void Error(Token source, string message)
		{
            var textPos = _parser.Lexer.ToTextPosition(source.Position).ToString();
			Console.WriteLine(textPos + " compiler error: " + message);
		}
	}
}
