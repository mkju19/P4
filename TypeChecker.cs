using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler {
    class TypeChecker : Visitor {
        public override void visit(Prog n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
            };
        }

        public override void visit(ProgSetup n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
            };
        }

        public override void visit(ProgLoop n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
            };
        }

        public override void visit(SymDeclaring n) {
            //throw new NotImplementedException();
        }

        public override void visit(VoidDcl n) {
            //throw new NotImplementedException();
        }

        public override void visit(IntDcl n) {
            //throw new NotImplementedException();
        }

        public override void visit(FloatDcl n) {
            //throw new NotImplementedException();
        }

        public override void visit(StringDcl n) {
            //throw new NotImplementedException();
        }

        public override void visit(BooleanDcl n) {
            //throw new NotImplementedException();
        }

        public override void visit(Decl n) {
            n.declaring.accept(this);
            n.assigning.accept(this);
        }

        public override void visit(FuncDecl n) {
            n.declaring.accept(this);
            foreach (SymDeclaring ast in n.declarings) {
                ast.accept(this);
            }
            foreach (AST ast in n.statments) {
                ast.accept(this);
            }
        }

        public override void visit(SymStatments n) {
            throw new NotImplementedException();
        }

        public override void visit(IfStmt n) {
            n.logi_expr.accept(this);
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            n.elseIF_Eles.accept(this);
        }

        public override void visit(ElseStmt n) {
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
        }

        public override void visit(WhileStmt n) {
            n.logi_expr.accept(this);
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
        }

        public override void visit(ForStmt n) {
            n.num_from.accept(this);
            n.num_to.accept(this);
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
        }

        public override void visit(SwitchStmt n) {
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
        }

        public override void visit(SwitchCase n) {
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
        }

        public override void visit(SwitchDefault n) {
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
        }

        public override void visit(FunctionStmt n) {
            foreach (AST ast in n.param_list) {
                ast.accept(this);
            }
        }

        public override void visit(Assigning n) {
            n.child.accept(this);
            int m = AST.SymbolTable[n.id];
            int t = generalize(n.child.type, m);
            n.child = convert(n.child, m);
            n.type = t;
        }

        public override void visit(SymReferencing n) {
            n.type = AST.SymbolTable[n.id];
        }

        public override void visit(IntConst n) {
            n.type = AST.INTTYPE;
        }

        public override void visit(FloatConst n) {
            n.type = AST.FLTTYPE;
        }

        public override void visit(StringConst n) {
            n.type = AST.STRING;
        }

        public override void visit(BooleanConst n) {
            n.type = AST.BOOLEAN;
        }

        public override void visit(Expression n) {
            n.childe1.accept(this);
            n.childe2.accept(this);
            int m = generalize(n.childe1.type, n.childe2.type);
            n.childe1 = convert(n.childe1, m);
            n.childe2 = convert(n.childe2, m);
            n.type = m;
        }

        public override void visit(NotExpression n) {
            n.childe.accept(this);
            n.type = n.childe.type;
        }

        public override void visit(ConvertingToFloat n) {
            n.child.accept(this);
            n.type = AST.FLTTYPE;
        }

        public override void visit(ConvertingToBool n) {
            n.child.accept(this);
            n.type = AST.BOOLEAN;
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
