using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler {
    public abstract class Visitor {
		public void Visit(AST n) {
			n.accept(this);
		}
		public abstract void Visit(Prog n);
		public abstract void Visit(ProgSetup n);
		public abstract void Visit(ProgLoop n);
		public abstract void Visit(SymDeclaring n);
		public abstract void Visit(VoidDcl n);
		public abstract void Visit(IntDcl n);
		public abstract void Visit(FloatDcl n);
		public abstract void Visit(StringDcl n);
		public abstract void Visit(BooleanDcl n);
		public abstract void Visit(StructDcl n);
		public abstract void Visit(ListDcl n);
		public abstract void Visit(Decl n);
		public abstract void Visit(FuncDecl n);
		public abstract void Visit(StructDcel n);
		public abstract void Visit(StructDef n);
		public abstract void Visit(SymStatments n);
		public abstract void Visit(IfStmt n);
		public abstract void Visit(ElseStmt n);
		public abstract void Visit(WhileStmt n);
		public abstract void Visit(ForStmt n);
		public abstract void Visit(SwitchStmt n);
		public abstract void Visit(SwitchCase n);
		public abstract void Visit(SwitchDefault n);
		public abstract void Visit(FunctionStmt n);
		public abstract void Visit(Assigning n);
		public abstract void Visit(SymReferencing n);
		public abstract void Visit(DotReferencing n);
		public abstract void Visit(ListReferencing n);
		public abstract void Visit(BooleanConst n);
		public abstract void Visit(IntConst n);
		public abstract void Visit(FloatConst n);
		public abstract void Visit(StringConst n);
		public abstract void Visit(ListConst n);
		public abstract void Visit(Expression n);
		public abstract void Visit(LogiExpression n);
		public abstract void Visit(NotExpression n);
		public abstract void Visit(ConvertingToString n);
		public abstract void Visit(ConvertingToFloat n);
		public abstract void Visit(ConvertingToBool n);
	}
}
