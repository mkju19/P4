using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler.Visitors {
    class InitiationChecker : Visitor {
        private int scopeLevel = 1;
        private List<int> scopekey = new List<int>() { 1 };
        private Dictionary<Tuple<string, string>, int> InitiationTable = new Dictionary<Tuple<string, string>, int>();

        public InitiationChecker() {
            foreach (KeyValuePair<Tuple<string, string>, int> keyValues in AST.SymbolTable) {
                InitiationTable.Add(keyValues.Key, 0);
            }
        }

        public override void visit(Prog n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            UnusedVariables();
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
        }

        public override void visit(IntDcl n) {
        }

        public override void visit(FloatDcl n) {
        }

        public override void visit(StringDcl n) {
        }

        public override void visit(BooleanDcl n) {
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
            InitiationTable[FindKey(n.id)] = 1;
            foreach (AST ast in n.param_list) {
                ast.accept(this);
            }
        }

        public override void visit(Assigning n) {
            InitiationTable[GetKey(n.id)] = 1;
            n.child.accept(this);
        }

        public override void visit(SymReferencing n) {
            if (!KeyValInit(n.id)) error($"{n.id} at {GetKey(n.id).Item1} is not initiated with a value");
        }

        public override void visit(IntConst n) {
        }

        public override void visit(FloatConst n) {
        }

        public override void visit(StringConst n) {
        }

        public override void visit(BooleanConst n) {
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

        private void plusScope() {
            if (scopekey.Count <= scopeLevel++) scopekey.Add(1);
        }

        private void minusScope() {
            if (scopekey.Count >= (--scopeLevel)) scopekey[scopeLevel]++;
            if (scopekey.Count >= (scopeLevel + 2)) scopekey.RemoveAt(scopekey.Count - 1);
        }

        private Tuple<string, string> GetKey(string id) {
            string val = "";
            foreach (int key in scopekey) {
                if (val.Length >= scopeLevel) break;
                val += key.ToString();
            }
            Tuple<string, string> tuple = new Tuple<string, string>(val, id);
            return tuple;
        }

        private Tuple<string, string> FindKey(string id) {
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

        private bool KeyValInit(string id) {
            string val = "";
            Tuple<string, string> tuple;
            foreach (int key in scopekey) {
                if (val.Length >= scopeLevel) break;
                val += key.ToString();
                tuple = new Tuple<string, string>(val, id);
                if (InitiationTable.ContainsKey(tuple) && InitiationTable[tuple] == 1) return true;
            }
            return false;
        }

        public void UnusedVariables() {
            foreach (KeyValuePair<Tuple<string, string>, int> keyValues in InitiationTable) {
                if (keyValues.Value != 0) continue;
                Console.WriteLine($"Variable {keyValues.Key.Item2} at {keyValues.Key.Item1} is not used");
            }
        }

        private void error(string message) {
            throw new Exception(message);
        }
    }
}
