using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler {
    public abstract class Visitor {
		public void visit(AST n) {
			n.accept(this);
		}
		public abstract void visit(Prog n);
		public abstract void visit(ProgSetup n);
		public abstract void visit(ProgLoop n);
		public abstract void visit(SymDeclaring n);
		public abstract void visit(VoidDcl n);
		public abstract void visit(IntDcl n);
		public abstract void visit(FloatDcl n);
		public abstract void visit(StringDcl n);
		public abstract void visit(BooleanDcl n);
		public abstract void visit(Decl n);
		public abstract void visit(FuncDecl n);
		public abstract void visit(SymStatments n);
		public abstract void visit(IfStmt n);
		public abstract void visit(ElseStmt n);
		public abstract void visit(WhileStmt n);
		public abstract void visit(ForStmt n);
		public abstract void visit(SwitchStmt n);
		public abstract void visit(SwitchCase n);
		public abstract void visit(SwitchDefault n);
		public abstract void visit(FunctionStmt n);
		public abstract void visit(Assigning n);
		public abstract void visit(SymReferencing n);
		public abstract void visit(IntConst n);
		public abstract void visit(FloatConst n);
		public abstract void visit(StringConst n);
		public abstract void visit(BooleanConst n);
		public abstract void visit(Expression n);
		public abstract void visit(NotExpression n);
		public abstract void visit(ConvertingToFloat n);
		public abstract void visit(ConvertingToBool n);
	}
}
