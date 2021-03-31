using System;
using System.Collections.Generic;
using System.Text;

namespace cocor_compiler {
    class TypeChecker : Visitor {
        public override void visit(Assigning n) {
            n.child1.accept(this);
            int m = AST.SymbolTable[n.id];
            int t = generalize(n.child1.type, m);
            n.child1 = convert(n.child1, m);
            n.type = t;
        }

        public override void visit(Expression n) {
            n.childe1.accept(this);
            n.childe2.accept(this);
            int m = generalize(n.childe1.type, n.childe2.type);
            n.childe1 = convert(n.childe1, m);
            n.childe2 = convert(n.childe2, m);
            n.type = m;
        }

        public override void visit(ConvertingToFloat n) {
            n.child.accept(this);
            n.type = AST.FLTTYPE;
        }

        public override void visit(FloatConsting n) {
            n.type = AST.FLTTYPE;
        }

        public override void visit(IntConsting n) {
            n.type = AST.INTTYPE;
        }

        public override void visit(Printing n) {
            n.type = AST.SymbolTable[n.id];
        }

        public override void visit(Prog n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
            };
        }

        public override void visit(SymDeclaring n) {
            //throw new NotImplementedException();
        }

        public override void visit(FloatDcl n) {
            //throw new NotImplementedException();
        }

        public override void visit(IntDcl n) {
            //throw new NotImplementedException();
        }

        public override void visit(SymReferencing n) {
            n.type = AST.SymbolTable[n.id];
        }
        private int generalize(int t1, int t2) {
            if (t1 == AST.FLTTYPE || t2 == AST.FLTTYPE) return AST.FLTTYPE; else return AST.INTTYPE;
        }

        private AST convert(AST n, int t) {
            if (n.type == AST.FLTTYPE && t == AST.INTTYPE) error("Illegal type conversion");
            else if (n.type == AST.INTTYPE && t == AST.FLTTYPE) return new ConvertingToFloat(n);
            return n;
        }
        private void error(string message) {
            throw new Exception(message);
        }
    }
}
