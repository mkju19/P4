using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler.Visitors {
    class TypeChecker : Visitor {
        // TODO inplemaent functions like Structs
        private int scopeLevel = 1;
        private List<int> scopekey = new List<int>() { 1 };
        private List<FuncDecl> funcs = new List<FuncDecl>();
        private List<FunctionStmt> funcCalls = new List<FunctionStmt>();
        string currentStructType = "";
        public Dictionary<string, List<Tuple<string, int>>> StructDic = new Dictionary<string, List<Tuple<string, int>>>();

        public override void Visit(Prog n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
            };
            Functioncheck();
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
            n.type = AST.VOID;
        }

        public override void Visit(IntDcl n) {
            n.type = AST.INTTYPE;
        }

        public override void Visit(FloatDcl n) {
            n.type = AST.FLTTYPE;
        }

        public override void Visit(StringDcl n) {
            n.type = AST.STRING;
        }

        public override void Visit(BooleanDcl n) {
            n.type = AST.BOOLEAN;
        }

        public override void Visit(StructDcl n) {
            n.type = AST.STRUCT;
        }

        public override void Visit(ListDcl n) {
            n.type = AST.LIST;
            n.listType.accept(this);
        }

        public override void Visit(Decl n) {
            n.declaring.accept(this);
            n.assigning?.accept(this);
        }

        public override void Visit(FuncDecl n) {
            n.declaring.accept(this);
            plusScope();
            foreach (SymDeclaring ast in n.declarings) {
                ast.accept(this);
            }
            foreach (AST ast in n.statments) {
                ast.accept(this);
            }
            minusScope();
            funcs.Add(n);
        }

        public override void Visit(StructDcel n) {
            n.type = AST.STRUCT;
            string pastStructType = "";
            if (currentStructType != "") pastStructType = currentStructType;
            SymReferencing current = n.structType as SymReferencing;
            currentStructType = current.id;
            foreach (AST ast in n.declarings) {
                ast.accept(this);

            }
            currentStructType = pastStructType;
        }

        public override void Visit(StructDef n) {
            n.type = AST.STRUCT;
            List<Tuple<string, int>> tuples = new List<Tuple<string, int>>();
            plusScope();
            foreach (AST ast in n.declarings) {
                ast.accept(this);
                if (ast is SymDeclaring) {
                    SymDeclaring symDeclaring = ast as SymDeclaring;
                    tuples.Add(new Tuple<string, int>(symDeclaring.id, GetType(ast)));
                } else if (ast is FuncDecl) {
                    FuncDecl funcDecl = ast as FuncDecl;
                    SymDeclaring symDeclaring = funcDecl.declaring as SymDeclaring;
                    tuples.Add(new Tuple<string, int>(symDeclaring.id, GetType(funcDecl.declaring)));
                }
            }
            minusScope();
            SymReferencing current = n.structType as SymReferencing;
            StructDic.Add(current.id, tuples);
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
            n.type = AST.SymbolTable[GetKeyVal(current.id)];
            foreach (AST ast in n.param_list) {
                ast.accept(this);
            }
            funcCalls.Add(n);
        }

        public override void Visit(Assigning n) {
            n.child.accept(this);
            int m = -99;
            SymReferencing current = n.id as SymReferencing;
            if (currentStructType != "") 
                if (StructDic[currentStructType].Exists(x => x.Item1 == current.id))
                    m = StructDic[currentStructType].Find(x => x.Item1 == current.id).Item2;
                else error($"{n.id} doesn't exist in {currentStructType}");
            else if (n.id is DotReferencing || n.id is ListReferencing) {
                n.id.accept(this);
                m = n.id.type;
            } else {
                m = AST.SymbolTable[GetKeyVal(current.id)];
            }
            int t = generalize(n.child.type, m);
            n.child = convert(n.child, m);
            n.type = t;
            if (n.child is StructDcel) {
                StructDcel assStructDcel = n.child as StructDcel;
                SymReferencing strucid = assStructDcel.structId as SymReferencing;
                SymReferencing structype = assStructDcel.structType as SymReferencing;
                StructDic.Add(strucid.id, StructDic[structype.id]);
            }
        }

        public override void Visit(SymReferencing n) {
            n.type = AST.SymbolTable[GetKeyVal(n.id)];
        }

        public override void Visit(DotReferencing n) { //TODO add support for structs in structs
            n.id.accept(this);
            if (n.id.type != AST.STRUCT) error($"{n.id.id} is not of type struct");
            if (n.dotId is SymReferencing) {
                SymReferencing sym = n.dotId as SymReferencing;
                if (!StructDic[n.id.id].Exists(x => x.Item1 == sym.id))
                    error($"{sym.id} doesn't exist in {n.id.id}");
                n.dotId.type = StructDic[n.id.id].Find(x => x.Item1 == sym.id).Item2;
            } else if (n.dotId is DotReferencing) {
                DotReferencing dot = n.dotId as DotReferencing;
                if (!StructDic[n.id.id].Exists(x => x.Item1 == dot.id.id))
                    error($"{dot.id.id} doesn't exist in {n.id.id}");
                n.dotId.accept(this);
            }
            n.type = n.dotId.type;
        }

        public override void Visit(ListReferencing n) {
            n.id.accept(this);
            int inum = 0;
            string type = n.id.type.ToString();
            SymReferencing sym = n.id as SymReferencing;
            if (type.Length < n.index.Count) error($"too many index dereferences in {sym.id}");
            foreach (AST item in n.index) {
                item.accept(this);
                if (item.type != AST.INTTYPE) error($"{sym.id}'s {inum} index is not of type int");
                inum++;
                type = type.Remove(0, 1);
            }
            n.type = int.Parse(type);
        }

        public override void Visit(BooleanConst n) {
            n.type = AST.BOOLEAN;
        }

        public override void Visit(IntConst n) {
            n.type = AST.INTTYPE;
        }

        public override void Visit(FloatConst n) {
            n.type = AST.FLTTYPE;
        }

        public override void Visit(StringConst n) {
            n.type = AST.STRING;
        }

        public override void Visit(ListConst n) {
            string type = AST.LIST.ToString();
            int elementType = -1;
            foreach (AST ast in n.declarings) {
                ast.accept(this);
                if (elementType == -1) elementType = ast.type;
                else if (ast.type != elementType) error($"List in {scopeLevel} not of same type");
            }
            if (elementType != -1) type += elementType.ToString();
            n.type = int.Parse(type);
        }

        public override void Visit(Expression n) {
            n.childe1.accept(this);
            n.childe2.accept(this);
            int m = generalize(n.childe1.type, n.childe2.type);
            n.childe1 = convert(n.childe1, m);
            n.childe2 = convert(n.childe2, m);
            n.type = m;
        }

        public override void Visit(NotExpression n) {
            n.childe.accept(this);
            n.type = n.childe.type;
        }

        public override void Visit(ConvertingToFloat n) {
            n.child.accept(this);
            n.type = AST.FLTTYPE;
        }

        public override void Visit(ConvertingToBool n) {
            n.child.accept(this);
            n.type = AST.BOOLEAN;
        }

        private int generalize(int t1, int t2) {
            if (t1 == AST.VOID || t2 == AST.VOID) return AST.VOID;
            else if (t1 == AST.BOOLEAN || t2 == AST.BOOLEAN) return AST.BOOLEAN;
            else if (t1 == AST.STRING || t2 == AST.STRING) return AST.STRING;
            else if (t1 == AST.FLTTYPE || t2 == AST.FLTTYPE) return AST.FLTTYPE;
            else if (t1 == AST.STRUCT || t2 == AST.STRUCT) return AST.STRUCT;
            else if (t1.ToString().Length > 1 || t1 == AST.LIST) return t1;
            else if (t2.ToString().Length > 1 || t2 == AST.LIST) return t2;
            else return AST.INTTYPE;
        }

        private AST convert(AST n, int t) {
            /*n = note to convert t = final type*/
            if (n.type == AST.VOID || t == AST.VOID) error("Illegal VOID conversion");
            if (n.type != AST.STRING && t == AST.STRING) error("Illegal STRING conversion");
            if (n.type == AST.STRING && t != AST.STRING) error("Illegal STRING conversion");
            if (n.type != AST.STRUCT && t == AST.STRUCT) error("Illegal STRUCT conversion");
            if (n.type == AST.STRUCT && t != AST.STRUCT) error("Illegal STRUCT conversion");
            if (n.type != AST.LIST && t == AST.LIST) error("Illegal LIST conversion");
            if (n.type == AST.LIST && t != AST.LIST) error("Illegal LIST conversion");
            if ((n.type.ToString().Length > 1 || t.ToString().Length > 1)
                && n.type != t) error("Illegal LIST conversion");
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
            if (scopekey.Count <= scopeLevel++) scopekey.Add(1);
        }

        private void minusScope() {
            if (scopekey.Count >= (--scopeLevel)) scopekey[scopeLevel]++;
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
                if (val.Length == 0) error($"{id} dose not existe!");
                val = val.Substring(0, val.Length - 1);
                tuple = new Tuple<string, string>(val, id);
            }
            return tuple;
        }

        private void Functioncheck() {
            foreach (FuncDecl item in funcs) {
                var func = funcCalls.FindAll(x => FindId(x.id) == item.declaring.id);
                int parameterAmunt = item.declarings.Count;
                foreach (FunctionStmt stmt in func) {
                    if (parameterAmunt != stmt.param_list.Count) error("too many/few parameters");
                    else {
                        for (int i = 0; i < parameterAmunt; i++) {
                            if (item.declarings[i].type != stmt.param_list[i].type) error($"parameter {i} in {stmt.id} is of wrong type");
                        }
                    }
                }
            }
        }

        private string FindId(AST node) {
            SymReferencing current = node as SymReferencing;
            return current.id;
        }

        private int GetType(AST node) {
            if (node is VoidDcl) {
                return 0;
            } else if (node is BooleanDcl) {
                return 1;
            } else if (node is IntDcl) {
                return 2;
            } else if (node is FloatDcl) {
                return 3;
            } else if (node is StringDcl) {
                return 4;
            } else if (node is StructDcl) {
                return 5;
            } else if (node is ListDcl) {
                return 6;
            } else {
                error("not a type");
                return -24;
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
    }
}
