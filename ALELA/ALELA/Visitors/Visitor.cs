using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALELA.AST;

namespace ALELA.Visitors
{
	public abstract class Visitor
	{
		public void visit(Tree n)
		{
			n.accept(this);
		}
		public abstract void visit(BinExpr n);
		public abstract void visit(Expr n);
		public abstract void visit(Id n);
		public abstract void visit(IntValue n);
		public abstract void visit(StringValue n);
		public abstract void visit(Printing n);
		public abstract void visit(Prog n);
		public abstract void visit(SymDeclaring n);
		public abstract void visit(FloatDcl n);
		public abstract void visit(IntDcl n);
		public abstract void visit(SymReferencing n);
	}
}
