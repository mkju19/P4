using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler.Visitors {
    class InitiationChecker : Visitor {
        private int scopeLevel = 1;
        private List<int> scopekey = new List<int>() { 1 };
        private Dictionary<Tuple<string, string>, int> InitiationTable = new Dictionary<Tuple<string, string>, int>();
        private Dictionary<string, List<Tuple<string, int>>> StructDic = new Dictionary<string, List<Tuple<string, int>>>();
        private string currentStructType = "";

        public InitiationChecker(Dictionary<string, List<Tuple<string, int>>> Dic) {
            foreach (KeyValuePair<Tuple<string, string>, int> keyValues in AST.SymbolTable) {
                InitiationTable.Add(keyValues.Key, 0);
            }
            foreach (KeyValuePair<string, List<Tuple<string, int>>> structvaluePair in Dic) {
                List<Tuple<string, int>> structdecls = new List<Tuple<string, int>>(); 
                foreach (Tuple<string, int> item in structvaluePair.Value) {
                    structdecls.Add(new Tuple<string, int>(item.Item1, 0));
                }
                StructDic.Add(structvaluePair.Key, structdecls);
            }
        }

        public override void Visit(Prog n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            UnusedVariables();
        }

        public override void Visit(ProgSetup n) {
            plusScope();
            foreach (AST ast in n.prog) {
                ast.accept(this);
            };
            minusScope();
        }

        public override void Visit(ProgLoop n) {
            plusScope();
            foreach (AST ast in n.prog) {
                ast.accept(this);
            };
            minusScope();
        }

        public override void Visit(SymDeclaring n) {
            //throw new NotImplementedException();
        }

        public override void Visit(VoidDcl n) {
        }

        public override void Visit(IntDcl n) {
        }

        public override void Visit(FloatDcl n) {
        }

        public override void Visit(StringDcl n) {
        }

        public override void Visit(BooleanDcl n) {
        }

        public override void Visit(StructDcl n) {
        }

        public override void Visit(ListDcl n) {
            n.listType.accept(this);
        }

        public override void Visit(Decl n) {
            n.declaring.accept(this);
            n.assigning?.accept(this);
        }

        public override void Visit(FuncDecl n) {
            n.declaring.accept(this);
            InitiationTable[GetKey(n.declaring.id)] = 1;
            plusScope();
            foreach (SymDeclaring ast in n.declarings) {
                ast.accept(this);
                InitiationTable[GetKey(ast.id)] = 1;
            }
            foreach (AST ast in n.statments) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void Visit(StructDcel n) {
            string pastStructType = "";
            if (currentStructType != "") pastStructType = currentStructType;
            SymReferencing current = n.structId as SymReferencing;
            currentStructType = current.id;
            foreach (AST ast in n.declarings) {
                ast.accept(this);
            }
            currentStructType = pastStructType;
        }

        public override void Visit(StructDef n) {
            plusScope();
            foreach (AST ast in n.declarings) {
                if (ast is SymDeclaring) {
                    SymDeclaring sym = ast as SymDeclaring;
                    InitiationTable[GetKey(sym.id)] = 1;
                } else { 
                    ast.accept(this);
                }
            }
            SymReferencing current = n.structType as SymReferencing;
            StructDic.Remove(current.id);
            minusScope();
        }

        public override void Visit(SymStatments n) {
            throw new NotImplementedException();
        }

        public override void Visit(IfStmt n) {
            n.logi_expr.accept(this);
            plusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            minusScope();
            n.elseIF_Eles?.accept(this);
        }

        public override void Visit(ElseStmt n) {
            plusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void Visit(WhileStmt n) {
            n.logi_expr.accept(this);
            plusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void Visit(ForStmt n) {
            plusScope();
            n.stm1.accept(this);
            n.stm2.accept(this);
            n.stm3.accept(this);
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void Visit(SwitchStmt n) {
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
        }

        public override void Visit(SwitchCase n) {
            plusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void Visit(SwitchDefault n) {
            plusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void Visit(FunctionStmt n) {
            SymReferencing current = n.id as SymReferencing;
            InitiationTable[FindKey(current.id)] = 1;
            foreach (AST ast in n.param_list) {
                ast.accept(this);
            }
        }

        public override void Visit(Assigning n) {
            SymReferencing current = n.id as SymReferencing;
            if (currentStructType != "")
                if (StructDic[currentStructType].Exists(x => x.Item1 == current.id)) {
                    Tuple<string, int> tuple = StructDic[currentStructType].Find(x => x.Item1 == current.id);
                    StructDic[currentStructType].Remove(tuple);
                    StructDic[currentStructType].Add(new Tuple<string, int>(tuple.Item1, 1));
                } else error($"{n.id} doesn't exist in {currentStructType}");
            else if (n.id is DotReferencing) {
                DotReferencing dot = n.id as DotReferencing;
                SymReferencing dotid = dot.dotId as SymReferencing;
                SymReferencing id = dot.id as SymReferencing;
                if (StructDic[id.id].Exists(x => x.Item1 == dotid.id)) {
                    Tuple<string, int> tuple = StructDic[id.id].Find(x => x.Item1 == dotid.id);
                    StructDic[id.id].Remove(tuple);
                    StructDic[id.id].Add(new Tuple<string, int>(tuple.Item1, 1));
                } else error($"{dotid.id} doesn't exist in {id.id}");
            } else InitiationTable[GetKey(current.id)] = 1;
            n.child.accept(this);
        }

        public override void Visit(SymReferencing n) {
            if (!KeyValInit(n.id)) error($"{n.id} at {GetKey(n.id).Item1} is not initiated with a value");
        }

        public override void Visit(DotReferencing n) {
            if (n.dotId is SymReferencing) {
                SymReferencing sym = n.dotId as SymReferencing;
                if (!StructDic[n.id.id].Exists(x => x.Item1 == sym.id))
                    error($"{sym.id} doesn't exist in {n.id.id}");
                if (StructDic[n.id.id].Find(x => x.Item1 == sym.id).Item2 != 1)
                    error($"{sym.id} in {n.id.id} is not initiated with a value");
            } else if (n.dotId is DotReferencing) {
                DotReferencing dot = n.dotId as DotReferencing;
                if (!StructDic[n.id.id].Exists(x => x.Item1 == dot.id.id))
                    error($"{dot.id.id} doesn't exist in {n.id.id}");
                n.dotId.accept(this);
            }
        }

        public override void Visit(BooleanConst n) {
        }

        public override void Visit(IntConst n) {
        }

        public override void Visit(FloatConst n) {
        }

        public override void Visit(StringConst n) {
        }

        public override void Visit(ListConst n) {
            foreach (AST ast in n.declarings) {
                ast.accept(this);
            }
        }

        public override void Visit(Expression n) {
            n.childe1.accept(this);
            n.childe2.accept(this);
        }

        public override void Visit(NotExpression n) {
            n.childe.accept(this);
        }

        public override void Visit(ConvertingToFloat n) {
            n.child.accept(this);
        }

        public override void Visit(ConvertingToBool n) {
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
            foreach (KeyValuePair<string, List<Tuple<string, int>>> structvaluePair in StructDic) {
                foreach (Tuple<string, int> item in structvaluePair.Value) {
                    if (item.Item2 != 0) continue;
                    Console.WriteLine($"Variable {structvaluePair.Key}.{item.Item1} is not used");
                }
            }
        }

        private string GetLastID(AST node) {
            if (node is SymReferencing) {
                SymReferencing sym = node as SymReferencing;
                return sym.id;
            } else if (node is DotReferencing) {
                DotReferencing dot = node as DotReferencing;
                return GetLastID(dot.dotId);
            } else {
                return "";
            }
        }

        private void error(string message) {
            throw new Exception(message);
        }
    }
}
