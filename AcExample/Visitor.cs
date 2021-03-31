using System;
using System.Collections.Generic;
using System.Text;

namespace cocor_compiler {
    public abstract class Visitor {
		public void visit(AST n) {
			n.accept(this);
		}
		public abstract void visit(Assigning n);
		public abstract void visit(Expression n);
		public abstract void visit(ConvertingToFloat n);
		public abstract void visit(FloatConsting n);
		public abstract void visit(IntConsting n);
		public abstract void visit(Printing n);
		public abstract void visit(Prog n);
		public abstract void visit(SymDeclaring n);
		public abstract void visit(FloatDcl n);
		public abstract void visit(IntDcl n);
		public abstract void visit(SymReferencing n);
	}
}
