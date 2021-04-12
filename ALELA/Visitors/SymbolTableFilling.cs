using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler {
    class SymbolTableFilling : Visitor {
        private int scopeLevel = 1;
        private List<int> scopekey = new List<int>() { 1 };
        public override void visit(Prog n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
        }

        public override void visit(ProgSetup n) {
            plusScope();
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void visit(ProgLoop n) {
            plusScope();
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void visit(SymDeclaring n) {
            //throw new NotImplementedException();
        }

        public override void visit(VoidDcl n) {
            if (!KeyValExists(n.id)) AST.SymbolTable.Add(GetKeyVal(n.id), AST.VOID);
            else error("variable " + n.id + " is already declared");
        }

        public override void visit(IntDcl n) {
            if (!KeyValExists(n.id)) AST.SymbolTable.Add(GetKeyVal(n.id), AST.INTTYPE);
            else error("variable " + n.id + " is already declared");
        }

        public override void visit(FloatDcl n) {
            if (!KeyValExists(n.id)) AST.SymbolTable.Add(GetKeyVal(n.id), AST.FLTTYPE);
            else error("variable " + n.id + " is already declared");
        }

        public override void visit(StringDcl n) {
            if (!KeyValExists(n.id)) AST.SymbolTable.Add(GetKeyVal(n.id), AST.STRING);
            else error("variable " + n.id + " is already declared");
        }

        public override void visit(BooleanDcl n) {
            if (!KeyValExists(n.id)) AST.SymbolTable.Add(GetKeyVal(n.id), AST.BOOLEAN);
            else error("variable " + n.id + " is already declared");
        }

        public override void visit(Decl n) {
            n.declaring.accept(this);
            n.assigning?.accept(this);
        }

        public override void visit(FuncDecl n) {
            n.declaring.accept(this);
            plusScope();
            foreach (AST ast in n.declarings) {
                ast.accept(this);
            }
            foreach (AST ast in n.statments) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void visit(SymStatments n) {
            //throw new NotImplementedException();
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
            plusScope();
            n.stm1.accept(this);
            n.stm2.accept(this);
            n.stm3.accept(this);
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
        }

        public override void visit(SymReferencing n) {
            //throw new NotImplementedException();
        }

        public override void visit(IntConst n) {
            //throw new NotImplementedException();
        }

        public override void visit(FloatConst n) {
            //throw new NotImplementedException();
        }

        public override void visit(StringConst n) {
            //throw new NotImplementedException();
        }

        public override void visit(BooleanConst n) {
            //throw new NotImplementedException();
        }

        public override void visit(Expression n) {
            n.childe1.accept(this);
            n.childe2.accept(this);
        }

        public override void visit(NotExpression n) {
            n.childe.accept(this);
        }

        public override void visit(ConvertingToFloat n) {
            n.child.accept(this);
        }

        public override void visit(ConvertingToBool n) {
            n.child.accept(this);
        }

        private void error(string message) {
            throw new Exception(message);
        }

        private void plusScope() {
            if (scopekey.Count <= scopeLevel++) scopekey.Add(1);
        }

        private void minusScope() {
            if (scopekey.Count >= --scopeLevel) scopekey[scopeLevel]++;
            if (scopekey.Count >= (scopeLevel + 2)) scopekey.RemoveAt(scopekey.Count - 1);
        }

        private Tuple<string, string> GetKeyVal(string id) {
            string val = "";
            foreach (int key in scopekey) {
                if (val.Length >= scopeLevel) break;
                val += key.ToString();
            }
            Tuple<string, string> tuple = new Tuple<string, string>(val, id);
            return tuple;
        }

        private bool KeyValExists(string id) {
            string val = "";
            foreach (int key in scopekey) {
                if (val.Length >= scopeLevel) break;
                val += key.ToString();
                if (AST.SymbolTable.ContainsKey(new Tuple<string, string>(val, id))) return true;
            }
            return false;
        }
    }
}
