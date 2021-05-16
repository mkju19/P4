using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler.Visitors {
    public class TypeChecker : Visitor {
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
            FunctionCheck();
        }

        public override void Visit(ProgSetup n) {
            PlusScope();
            foreach (AST ast in n.prog) {
                ast.accept(this);
            };
            MinusScope();
        }

        public override void Visit(ProgLoop n) {
            PlusScope();
            foreach (AST ast in n.prog) {
                ast.accept(this);
            };
            MinusScope();
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
            PlusScope();
            foreach (SymDeclaring ast in n.declarings) {
                ast.accept(this);
            }
            foreach (AST ast in n.statments) {
                ast.accept(this);
            }
            if (n.returnValue == null && n.declaring.type != AST.VOID) {
                Error($"function {n.declaring.id} must use \"return\"");
            } else if (n.returnValue != null && n.declaring.type == AST.VOID) {
                Error($"function {n.declaring.id} must not use \"return\"");
            } else if (n.returnValue != null) {
                n.returnValue.accept(this);
                if (n.declaring.type != n.returnValue.type) Error($"return must be of type {n.declaring.type}");
            }
            MinusScope();
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
            PlusScope();
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
            MinusScope();
            SymReferencing current = n.structType as SymReferencing;
            StructDic.Add(current.id, tuples);
        }

        public override void Visit(SymStatments n) {
            throw new NotImplementedException();
        }

        public override void Visit(IfStmt n) {
            n.logi_expr.accept(this);
            PlusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            MinusScope();
            n.elseIF_Eles?.accept(this);
        }

        public override void Visit(ElseStmt n) {
            PlusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            MinusScope();
        }

        public override void Visit(WhileStmt n) {
            n.logi_expr.accept(this);
            PlusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            MinusScope();
        }

        public override void Visit(ForStmt n) {
            PlusScope();
            n.stm1.accept(this);
            n.stm2.accept(this);
            n.stm3.accept(this);
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            MinusScope();
        }

        public override void Visit(SwitchStmt n) {
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
        }

        public override void Visit(SwitchCase n) {
            PlusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            MinusScope();
        }

        public override void Visit(SwitchDefault n) {
            PlusScope();
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            MinusScope();
        }

        public override void Visit(FunctionStmt n) {
            if (n.id is DotReferencing) {
                n.id.accept(this);
                n.type = n.id.type;
            } else {
                SymReferencing current = n.id as SymReferencing;
                n.type = AST.SymbolTable[GetKeyVal(current.id)];
            }
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
                else Error($"{n.id} doesn't exist in {currentStructType}");
            else if (n.id is DotReferencing || n.id is ListReferencing) {
                n.id.accept(this);
                m = n.id.type;
            } else {
                m = AST.SymbolTable[GetKeyVal(current.id)];
            }
            int t = Generalize(n.child.type, m);
            n.child = Convert(n.child, m);
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
            if (n.id.type.ToString().Contains("6")) {
                SymReferencing sym = n.dotId as SymReferencing;
                int newType = int.Parse(n.id.type.ToString().Substring(1));
                switch (sym.id) {
                    case "size":
                        n.dotId.type = AST.INTTYPE;
                        break;
                    case "add":
                        n.dotId.type = AST.BOOLEAN;
                        break;
                    case "unshift":
                        n.dotId.type = AST.BOOLEAN;
                        break;
                    case "set":
                        n.dotId.type = AST.BOOLEAN;
                        break;
                    case "remove":
                        n.dotId.type = newType;
                        break;
                    case "pop":
                        n.dotId.type = newType;
                        break;
                    case "shift":
                        n.dotId.type = newType;
                        break;
                    case "get":
                        n.dotId.type = newType;
                        break;
                    case "clear":
                        n.dotId.type = AST.VOID;
                        break;
                    default:
                        Error($"{sym.id} is not a valid function on Lists");
                        break;
                }
            } else if(n.id.type == AST.UART) {
                SymReferencing sym = n.dotId as SymReferencing;
                switch (sym.id) {
                    case "begin":
                        n.dotId.type = AST.VOID;
                        break;
                    case "print":
                        n.dotId.type = AST.VOID;
                        break;
                    case "printLine":
                        n.dotId.type = AST.VOID;
                        break;
                    case "read":
                        n.dotId.type = AST.STRING;
                        break;
                    case "parseInt":
                        n.dotId.type = AST.INTTYPE;
                        break;
                    case "available":
                        n.dotId.type = AST.BOOLEAN;
                        break;
                    default:
                        Error($"{sym.id} is not a valid function on serial");
                        break;
                }
            } else if (n.id.type == AST.PIN) {
                SymReferencing sym = n.dotId as SymReferencing;
                switch (sym.id) {
                    case "pinMode":
                        n.dotId.type = AST.VOID;
                        break;
                    case "digitalPower":
                        n.dotId.type = AST.VOID;
                        break;
                    case "analogPower":
                        n.dotId.type = AST.VOID;
                        break;
                    case "powerValue":
                        n.dotId.type = AST.VOID;
                        break;
                    case "digitalRead":
                        n.dotId.type = AST.BOOLEAN;
                        break;
                    case "analogRead":
                        n.dotId.type = AST.INTTYPE;
                        break;
                    case "analogReadValue":
                        n.dotId.type = AST.INTTYPE;
                        break;
                    default:
                        Error($"{sym.id} is not a valid function on pins");
                        break;
                }
            } else if (n.id.type != AST.STRUCT) Error($"{n.id.id} is not of type struct");
            else {
                if (n.dotId is SymReferencing) {
                    SymReferencing sym = n.dotId as SymReferencing;
                    if (!StructDic[n.id.id].Exists(x => x.Item1 == sym.id))
                        Error($"{sym.id} doesn't exist in {n.id.id}");
                    n.dotId.type = StructDic[n.id.id].Find(x => x.Item1 == sym.id).Item2;
                } else if (n.dotId is DotReferencing) {
                    DotReferencing dot = n.dotId as DotReferencing;
                    if (!StructDic[n.id.id].Exists(x => x.Item1 == dot.id.id))
                        Error($"{dot.id.id} doesn't exist in {n.id.id}");
                    n.dotId.accept(this);
                }
            }
            n.type = n.dotId.type;
        }

        public override void Visit(ListReferencing n) {
            n.id.accept(this);
            int inum = 0;
            string type = n.id.type.ToString();
            SymReferencing sym = n.id as SymReferencing;
            if (type.Length < n.index.Count) Error($"too many index dereferences in {sym.id}");
            foreach (AST item in n.index) {
                item.accept(this);
                if (item.type != AST.INTTYPE) Error($"{sym.id}'s {inum} index is not of type int");
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
                else if (ast.type != elementType) Error($"List in {scopeLevel} not of same type");
            }
            if (elementType != -1) type += elementType.ToString();
            n.type = int.Parse(type);
        }

        public override void Visit(Expression n) {
            n.childe1.accept(this);
            n.childe2.accept(this);
            int m = Generalize(n.childe1.type, n.childe2.type);
            n.childe1 = Convert(n.childe1, m);
            n.childe2 = Convert(n.childe2, m);
            if ((n.childe1 is ConvertingToString || n.childe2 is ConvertingToString)
                && n.operation != "+") Error("Illegal expression on STRING");
            n.type = m;
        }

        public override void Visit(LogiExpression n) {
            n.childe1.accept(this);
            n.childe2?.accept(this);
            n.type = AST.BOOLEAN;
        }

        public override void Visit(NotExpression n) {
            n.childe.accept(this);
            n.type = n.childe.type;
        }

        public override void Visit(ConvertingToString n) {
            n.child.accept(this);
            n.type = AST.STRING;
        }

        public override void Visit(ConvertingToFloat n) {
            n.child.accept(this);
            n.type = AST.FLTTYPE;
        }

        public override void Visit(ConvertingToBool n) {
            n.child.accept(this);
            n.type = AST.BOOLEAN;
        }

        private int Generalize(int t1, int t2) {
            if (t1 == AST.VOID || t2 == AST.VOID) return AST.VOID;
            else if (t1 == AST.STRING || t2 == AST.STRING) return AST.STRING;
            else if (t1 == AST.BOOLEAN || t2 == AST.BOOLEAN) return AST.BOOLEAN;
            else if (t1 == AST.FLTTYPE || t2 == AST.FLTTYPE) return AST.FLTTYPE;
            else if (t1 == AST.STRUCT || t2 == AST.STRUCT) return AST.STRUCT;
            else if (t1.ToString().Length > 1 || t1 == AST.LIST) return t1;
            else if (t2.ToString().Length > 1 || t2 == AST.LIST) return t2;
            else return AST.INTTYPE;
        }

        private AST Convert(AST n, int t) {
            /*n = note to convert t = final type*/
            if (n.type == AST.VOID || t == AST.VOID) Error("Illegal VOID conversion");
            if (n.type == AST.STRING && t != AST.STRING) Error("Illegal STRING conversion");
            if ((n.type == AST.BOOLEAN || n.type == AST.INTTYPE || n.type == AST.FLTTYPE ) && t == AST.STRING) return new ConvertingToString(n);
            if (n.type != AST.STRUCT && t == AST.STRUCT) Error("Illegal STRUCT conversion");
            if (n.type == AST.STRUCT && t != AST.STRUCT) Error("Illegal STRUCT conversion");
            if (n.type != AST.LIST && t == AST.LIST) Error("Illegal LIST conversion");
            if (n.type == AST.LIST && t != AST.LIST) Error("Illegal LIST conversion");
            if ((n.type.ToString().Length > 1 || t.ToString().Length > 1)
                && n.type != t) Error("Illegal LIST conversion");
            if (n.type == AST.BOOLEAN && t != AST.BOOLEAN) Error("Illegal BOOLEAN conversion");
            else if (n.type != AST.BOOLEAN && t == AST.BOOLEAN) Error("Illegal BOOLEAN conversion"); //return new ConvertingToBool(n);
            if (n.type == AST.FLTTYPE && t == AST.INTTYPE) Error("Illegal FLTTYPE conversion");
            else if (n.type == AST.INTTYPE && t == AST.FLTTYPE) return new ConvertingToFloat(n);
            return n;
        }

        private void Error(string message) {
            throw new Exception(message);
        }

        private void PlusScope() {
            if (scopekey.Count <= scopeLevel++) scopekey.Add(1);
        }

        private void MinusScope() {
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
                if (val.Length == 0) Error($"{id} dose not existe!");
                val = val.Substring(0, val.Length - 1);
                tuple = new Tuple<string, string>(val, id);
            }
            return tuple;
        }

        private void FunctionCheck() {
            foreach (FuncDecl item in funcs) {
                var func = funcCalls.FindAll(x => FindId(x.id) == item.declaring.id);
                int parameterAmunt = item.declarings.Count;
                foreach (FunctionStmt stmt in func) {
                    if (parameterAmunt != stmt.param_list.Count) Error("too many/few parameters");
                    else {
                        for (int i = 0; i < parameterAmunt; i++) {
                            if (item.declarings[i].type != stmt.param_list[i].type) Error($"parameter {i} in {stmt.id} is of wrong type");
                        }
                    }
                }
            }
            ListFunctionCheck();
            UARTFunctionCheck();
            PinFunctionCheck();
        }

        private void ListFunctionCheck() {
            foreach (FunctionStmt item in funcCalls) {
                if (!(item.id is DotReferencing dot)) continue;
                else if (!(dot.id.type.ToString().Contains('6'))) continue;
                else {
                    var sym = dot.dotId as SymReferencing;
                    switch (sym.id) {
                        case "size":
                            if (item.param_list.Count != 0)
                                Error("size()");
                            break;
                        case "add":
                            if (!((item.param_list.Count == 1 && item.param_list[0].type == int.Parse(dot.id.type.ToString().Substring(1)))
                                || (item.param_list.Count == 2 && item.param_list[0].type == AST.INTTYPE
                                    && item.param_list[1].type == int.Parse(dot.id.type.ToString().Substring(1))))){
                                Error("add(T) or add(int index, T)");
                            }
                            break;
                        case "unshift":
                            if (item.param_list.Count != 1 || item.param_list[0].type != int.Parse(dot.id.type.ToString().Substring(1)))
                                Error("unshift(T)");
                            break;
                        case "set":
                            if (item.param_list.Count != 2 || item.param_list[0].type != AST.INTTYPE || item.param_list[2].type != int.Parse(dot.id.type.ToString().Substring(1)))
                                Error("set(int index, T)");
                            break;
                        case "remove":
                            if (item.param_list.Count != 1 || item.param_list[0].type != AST.INTTYPE)
                                Error("remove(int index)");
                            break;
                        case "pop":
                            if (item.param_list.Count != 0)
                                Error("pop()");
                            break;
                        case "shift":
                            if (item.param_list.Count != 0)
                                Error("shift()");
                            break;
                        case "get":
                            if (item.param_list.Count != 1 || item.param_list[0].type != AST.INTTYPE)
                                Error("get(int index)");
                            break;
                        case "clear":
                            if (item.param_list.Count != 0)
                                Error("clear()");
                            break;
                        default:
                            Error($"{sym.id} is not a valid function on Lists");
                            break;
                    }
                }
            }

        }

        private void UARTFunctionCheck() {
            foreach (FunctionStmt item in funcCalls) {
                if (!(item.id is DotReferencing dot)) continue;
                else if (dot.id.type != AST.UART) continue;
                else {
                    var sym = dot.dotId as SymReferencing;
                    switch (sym.id) {
                        case "begin":
                            if (item.param_list.Count != 1 || item.param_list[0].type != AST.INTTYPE)
                                Error("begin(int val)");
                            break;
                        case "print":
                            //TODO add support for "Serial.print(val, format)"
                            if (item.param_list.Count != 1 || item.param_list[0].type == AST.VOID)
                                Error("print(val)");
                            break;
                        case "printLine":
                            //TODO add support for "Serial.print(val, format)"
                            if (item.param_list.Count != 1 || item.param_list[0].type == AST.VOID)
                                Error("printLine(val)");
                            break;
                        case "read":
                            if (item.param_list.Count != 0)
                                Error("read()");
                            break;
                        case "parseInt":
                            //TODO add support for "Serial.parseInt(lookahead)"
                            //TODO add support for "Serial.parseInt(lookahead, ignore)"
                            if (item.param_list.Count != 0)
                                Error("parseInt()");
                            break;
                        case "available":
                            if (item.param_list.Count != 0)
                                Error("available()");
                            break;
                        default:
                            Error($"{sym.id} is not a valid function on serial");
                            break;
                    }
                }
            }
        }

        private void PinFunctionCheck() {
            foreach (FunctionStmt item in funcCalls) {
                if (!(item.id is DotReferencing dot)) continue;
                else if (dot.id.type != AST.PIN) continue;
                else {
                    var sym = dot.dotId as SymReferencing;
                    switch (sym.id) {
                        case "pinMode":
                            if (item.param_list.Count != 2 || item.param_list[0].type != AST.INTTYPE || item.param_list[1].type != AST.INTTYPE)
                                Error("pinMode(int pin, int mode)");
                            break;
                        case "digitalPower":
                            if (item.param_list.Count != 1 || item.param_list[0].type != AST.BOOLEAN)
                                Error("digitalPower(int val)");
                            break;
                        case "analogPower":
                            if (item.param_list.Count != 1 || item.param_list[0].type != AST.INTTYPE)
                                Error("analogPower(int val)");
                            break;
                        case "powerValue":
                            if (item.param_list.Count != 1 || item.param_list[0].type != AST.INTTYPE)
                                Error("powerValue(int val)");
                            break;
                        case "digitalRead":
                            if (item.param_list.Count != 0)
                                Error("digitalRead()");
                            break;
                        case "analogRead":
                            if (item.param_list.Count != 0)
                                Error("analogRead()");
                            break;
                        case "analogReadValue":
                            if (item.param_list.Count != 0)
                                Error("analogReadValue()");
                            break;
                        default:
                            Error($"{sym.id} is not a valid function on pin");
                            break;
                    }
                }
            }
        }

        private string FindId(AST node) {
            if (node is DotReferencing) {
                return GetLastID(node);
            } else {
                SymReferencing current = node as SymReferencing;
                return current.id;
            }
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
                Error("not a type");
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
