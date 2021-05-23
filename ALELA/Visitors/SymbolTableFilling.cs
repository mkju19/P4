using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler.Visitors {
    public class SymbolTableFilling : Visitor {
        private int scopeLevel = 1;
        private List<int> scopekey = new List<int>() { 1 };
        
        public override void Visit(Prog n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
        }

        public override void Visit(ProgSetup n) {
            plusScope();
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void Visit(ProgLoop n) {
            plusScope();
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void Visit(SymDeclaring n) {
            //throw new NotImplementedException();
        }

        public override void Visit(VoidDcl n) {
            if (!KeyValExists(n.id)) AST.SymbolTable.Add(GetKeyVal(n.id), AST.VOID);
            else error("variable " + n.id + " is already declared");
        }

        public override void Visit(IntDcl n) {
            if (!KeyValExists(n.id)) AST.SymbolTable.Add(GetKeyVal(n.id), AST.INTTYPE);
            else error("variable " + n.id + " is already declared");
        }

        public override void Visit(FloatDcl n) {
            if (!KeyValExists(n.id)) AST.SymbolTable.Add(GetKeyVal(n.id), AST.FLTTYPE);
            else error("variable " + n.id + " is already declared");
        }

        public override void Visit(StringDcl n) {
            if (!KeyValExists(n.id)) AST.SymbolTable.Add(GetKeyVal(n.id), AST.STRING);
            else error("variable " + n.id + " is already declared");
        }

        public override void Visit(BooleanDcl n) {
            if (!KeyValExists(n.id)) AST.SymbolTable.Add(GetKeyVal(n.id), AST.BOOLEAN);
            else error("variable " + n.id + " is already declared");
        }

        public override void Visit(StructDcl n) {
            if (!KeyValExists(n.id)) AST.SymbolTable.Add(GetKeyVal(n.id), AST.STRUCT);
            else error("variable " + n.id + " is already declared");
        }

        public override void Visit(ListDcl n) {
            string type = AST.LIST.ToString();
            type += n.listType is ListDcl ? ListType(n.listType as ListDcl) : ListType(n.listType);
            if (!KeyValExists(n.id)) AST.SymbolTable.Add(GetKeyVal(n.id), int.Parse(type));
            else error("variable " + n.id + " is already declared");
        }

        private string ListType(ListDcl node) {
            return $"{GetType(node)}{GetType(node.listType)}";
        }

        private string ListType(SymDeclaring node) {
            return $"{GetType(node)}";
        }

        public override void Visit(Decl n) {
            n.declaring.accept(this);
            n.assigning?.accept(this);
        }

        public override void Visit(FuncDecl n) {
            n.declaring.accept(this);
            plusScope();
            foreach (AST ast in n.declarings) {
                ast.accept(this);
            }
            foreach (AST ast in n.statments) {
                ast.accept(this);
            }
            if (n.returnValue != null) n.returnValue.accept(this);
            minusScope();
        }

        public override void Visit(StructDecl n) {
            foreach (AST ast in n.declarings) {
                ast.accept(this);
            }
        }

        public override void Visit(StructDef n) {
            plusScope();
            foreach (AST ast in n.declarings) {
                ast.accept(this);
            }
            minusScope();
        }

        public override void Visit(SymStatments n) {
            //throw new NotImplementedException();
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
            n.id.accept(this);
            foreach (AST ast in n.param_list) {
                ast.accept(this);
            }
        }

        public override void Visit(Assigning n) {
            n.child.accept(this);
        }

        public override void Visit(SymReferencing n) {
        }

        public override void Visit(DotReferencing n) {
            if (n.dotId is SymReferencing dotSerial && dotSerial.id == "begin") {
                if ((n.id.id == "Serial" || n.id.id == "Serial1" || n.id.id == "Serial2" || n.id.id == "Serial3") && (!KeyValExists(n.id.id))) {
                    if (!KeyValExists(n.id.id)) AST.SymbolTable.Add(new Tuple<string, string>("1", n.id.id), AST.UART);
                    else error("variable " + n.id.id + " is already declared");
                }
            } else if (n.dotId is SymReferencing dotPin && dotPin.id == "pinMode") {
                if (!KeyValExists(n.id.id)) AST.SymbolTable.Add(new Tuple<string, string>("1", n.id.id), AST.PIN);
                else error("variable " + n.id.id + " is already declared");
            }
        }

        public override void Visit(ListReferencing n) {
            //throw new NotImplementedException();
        }

        public override void Visit(BooleanConst n) {
            //throw new NotImplementedException();
        }

        public override void Visit(IntConst n) {
            //throw new NotImplementedException();
        }

        public override void Visit(FloatConst n) {
            //throw new NotImplementedException();
        }

        public override void Visit(StringConst n) {
            //throw new NotImplementedException();
        }

        public override void Visit(ListConst n) { // may need implpamentation
            /*foreach (AST ast in n.declarings) {
                ast.accept(this);
            }*/
        }

        public override void Visit(Expression n) {
            n.childe1.accept(this);
            n.childe2.accept(this);
        }

        public override void Visit(LogiExpression n) {
            n.childe1.accept(this);
            n.childe2?.accept(this);
        }

        public override void Visit(NotExpression n) {
            n.childe.accept(this);
        }

        public override void Visit(ConvertingToString n) {
            n.child.accept(this);
        }

        public override void Visit(ConvertingToFloat n) {
            n.child.accept(this);
        }

        public override void Visit(ConvertingToBool n) {
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

        private string GetType(AST node) {
            if (node is VoidDcl) {
                return "0";
            } else if (node is BooleanDcl) {
                return "1";
            } else if (node is IntDcl) {
                return "2";
            } else if (node is FloatDcl) {
                return "3";
            } else if (node is StringDcl) {
                return "4";
            } else if (node is StructDcl) {
                return "5";
            } else if (node is ListDcl) {
                return "6";
            } else {
                return "";
            }
        }

        public string PrintSymbolTable() {
            string dictionaryString = "{";
            foreach (KeyValuePair<Tuple<string, string>, int> keyValues in AST.SymbolTable) {
                dictionaryString += keyValues.Key + " : " + keyValues.Value + ", ";
            }
            return dictionaryString.TrimEnd(',', ' ') + "}";
        }
    }
}
