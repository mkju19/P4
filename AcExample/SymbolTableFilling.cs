using System;
using System.Collections.Generic;
using System.Text;

namespace cocor_compiler {
    class SymbolTableFilling : Visitor {
        public override void visit(Assigning n) {
            n.child1.accept(this);
        }

        public override void visit(Expression n) {
            n.childe1.accept(this);
            n.childe2.accept(this);
        }

        public override void visit(ConvertingToFloat n) {
            n.child.accept(this);
        }

        public override void visit(FloatConsting n) {
            //throw new NotImplementedException();
        }

        public override void visit(IntConsting n) {
            //throw new NotImplementedException();
        }

        public override void visit(Printing n) {
            //throw new NotImplementedException();
        }

        public override void visit(Prog n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
        }

        public override void visit(SymDeclaring n) {
            //throw new NotImplementedException();
        }

        public override void visit(FloatDcl n) {
            if (!AST.SymbolTable.ContainsKey(n.id)) AST.SymbolTable.Add(n.id, AST.FLTTYPE);
            else error("variable " + n.id + " is already declared");
        }

        public override void visit(IntDcl n) {
            if (!AST.SymbolTable.ContainsKey(n.id)) AST.SymbolTable.Add(n.id, AST.INTTYPE);
            else error("variable " + n.id + " is already declared");
        }

        public override void visit(SymReferencing n) {
            //throw new NotImplementedException();
        }

        private void error(string message) {
            throw new Exception(message);
        }
    }
}
