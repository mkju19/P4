using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler {
    class TypeChecker : Visitor {
        private int scopeLevel = 1;
        private List<int> scopekey = new List<int>() { 1 };

        public override void visit(Prog n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
            };
        }

        public override void visit(ProgSetup n) {
            plusScope();
            foreach (AST ast in n.prog) {
                ast.accept(this);
            };
            minusScope();
        }

        public override void visit(ProgLoop n) {
            plusScope();
            foreach (AST ast in n.prog) {
                ast.accept(this);
            };
            minusScope();
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
            n.assigning?.accept(this);
        }

        public override void visit(FuncDecl n) {
            n.declaring.accept(this);
            foreach (SymDeclaring ast in n.declarings) {
                ast.accept(this);
            }
            plusScope();
            foreach (AST ast in n.statments) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void visit(SymStatments n) {
            throw new NotImplementedException();
        }

        public override void visit(IfStmt n) {
            n.logi_expr.accept(this);
            plusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            minusScope();
            n.elseIF_Eles?.accept(this);
        }

        public override void visit(ElseStmt n) {
            plusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void visit(WhileStmt n) {
            n.logi_expr.accept(this);
            plusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void visit(ForStmt n) {
            n.num_from.accept(this);
            n.num_to.accept(this);
            plusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void visit(SwitchStmt n) {
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
        }

        public override void visit(SwitchCase n) {
            plusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void visit(SwitchDefault n) {
            plusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void visit(FunctionStmt n) {
            foreach (AST ast in n.param_list) {
                ast.accept(this);
            }
        }

        public override void visit(Assigning n) {
            n.child.accept(this);
            int m = AST.SymbolTable[GetKeyVal(n.id)];
            int t = generalize(n.child.type, m);
            n.child = convert(n.child, m);
            n.type = t;
        }

        public override void visit(SymReferencing n) {
            n.type = AST.SymbolTable[GetKeyVal(n.id)];
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
            if (t1 == AST.VOID || t2 == AST.VOID) return AST.VOID;
            else if (t1 == AST.BOOLEAN || t2 == AST.BOOLEAN) return AST.BOOLEAN;
            else if (t1 == AST.STRING || t2 == AST.STRING) return AST.STRING;
            else if (t1 == AST.FLTTYPE || t2 == AST.FLTTYPE) return AST.FLTTYPE;
            else return AST.INTTYPE;
        }

        private AST convert(AST n, int t) {
            /*n = note to convert t = final type*/
            if (n.type == AST.VOID || t == AST.VOID) error("Illegal VOID conversion");
            if (n.type != AST.STRING && t == AST.STRING) error("Illegal STRING conversion");
            if (n.type == AST.STRING && t != AST.STRING) error("Illegal STRING conversion");
            if (n.type == AST.BOOLEAN && t != AST.BOOLEAN) error("Illegal BOOLEAN conversion");
            else if (n.type != AST.BOOLEAN && t == AST.BOOLEAN) return new ConvertingToBool(n);
            if (n.type == AST.FLTTYPE && t == AST.INTTYPE) error("Illegal FLTTYPE conversion");
            else if (n.type == AST.INTTYPE && t == AST.FLTTYPE) return new ConvertingToFloat(n);
            return n;
        }

        private void error(string message) {
            throw new Exception(message);
        }

        private void plusScope() {
            scopeLevel++;
            if (scopekey.Count <= scopeLevel - 1) scopekey.Add(1);
        }

        private void minusScope() {
            scopeLevel--;
            if (scopekey.Count >= (scopeLevel)) scopekey[scopeLevel]++;
            if (scopekey.Count >= (scopeLevel + 2)) scopekey.RemoveAt(scopekey.Count - 1);
        }

        private Tuple<string, string> GetKeyVal(string id) {
            string val = "";
            foreach (int key in scopekey) {
                if (val.Length >= scopeLevel) break;
                val += key.ToString();
            }
            Tuple<string, string> tuple = new Tuple<string, string>(val, id);
            while (!AST.SymbolTable.ContainsKey(tuple)) {
                if (val.Length == 0) error("value dose not existe!");
                val = val.Substring(0, val.Length - 1);
                tuple = new Tuple<string, string>(val, id);
            }
            return tuple;
        }
    }
}
