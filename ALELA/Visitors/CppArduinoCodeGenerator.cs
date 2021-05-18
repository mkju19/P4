using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler.Visitors {
    class CppArduinoCodeGenerator : Visitor {
        public string Code = "#include <LinkedList.h>\n";
        private Dictionary<string, string> pinValue = new Dictionary<string, string>();
        private string serialTempString = "SerialMonitorTemporaryString";
        
        public CppArduinoCodeGenerator(Prog n) {
            STD std = new STD();
            std.RemoveFrom(n);
            bool tempExists;
            do {
                tempExists = false;
                foreach (var key in AST.SymbolTable.Keys) {
                    if (key.Item2 == serialTempString) {
                        serialTempString += "x";
                        tempExists = true;
                        break;
                    }
                }
            } while (tempExists);
            n.prog.Insert(0, new Decl(new StringDcl(serialTempString), new Assigning(new SymReferencing(serialTempString), new StringConst("\"\""))));
        }

        public void emit(string c) {
            Code += c;
        }
        public override void Visit(Prog n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
        }

        public override void Visit(ProgSetup n) {
            emit("void setup () {\n");
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            emit("}\n");
        }

        public override void Visit(ProgLoop n) {
            emit("void loop () {\n");
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            emit("\n}\n");
        }

        public override void Visit(SymDeclaring n) {
            throw new NotImplementedException();
        }

        public override void Visit(VoidDcl n) {
            emit($"void {n.id} ");
        }

        public override void Visit(IntDcl n) {
            emit($"int {n.id} ");
        }

        public override void Visit(FloatDcl n) {
            emit($"float {n.id} ");
        }

        public override void Visit(StringDcl n) {
            emit($"String {n.id} ");
        }

        public override void Visit(BooleanDcl n) {
            emit($"boolean {n.id} ");
        }

        public override void Visit(StructDcl n) {
            emit($"struct {n.id} ");
        }

        public override void Visit(ListDcl n) {
            emit($"LinkedList<");
            n.listType.accept(this);
            emit($"> {n.id}");

            /*int depth = 0;
            AST sT = n;
            while (sT is ListDcl) {
                depth++;
                ListDcl listDcl = sT as ListDcl;
                sT = listDcl.listType;
            }
            sT.accept(this);
            emit($"{n.id}");
            for (int i = 0; i < depth; i++) {
                emit("[]");
            }*/
        }

        public override void Visit(Decl n) {
            if (n.assigning != null && !(n.declaring is VoidDcl)) {
                n.declaring.accept(this);
                if (n.declaring is ListDcl) {
                    emit(" = ");
                    Assigning assigning = n.assigning as Assigning;
                    assigning.child.accept(this);
                    emit(";\n");
                } else {
                    Code = Code.Remove((Code.Length - n.declaring.id.Length) - 1);
                    n.assigning.accept(this);
                }
            } else if (!(n.declaring is VoidDcl)) {
                n.declaring.accept(this);
                emit(";\n");
            }
        }

        public override void Visit(FuncDecl n) {
            n.declaring.accept(this);
            emit("(");
            if (n.declarings.Count > 0) {
                SymDeclaring first = n.declarings[0];
                foreach (SymDeclaring ast in n.declarings) {
                    if (first != ast) {
                        emit(", ");
                    }
                    ast.accept(this);
                }
            }
            emit(") {\n");
            foreach (AST ast in n.statments) {
                ast.accept(this);
            }
            if (n.returnValue != null) {
                emit("return ");
                n.returnValue.accept(this); emit(";\n");
            }
            emit("}\n");
        }

        public override void Visit(StructDcel n) {
            SymReferencing sym = n.structType as SymReferencing;
            Code = Code.Remove(Code.Length - $"{sym.id} = ".Length); //TODO check is "struct {sym.id} = " is nessecary
            n.structType.accept(this);
            emit(" ");
            n.structId.accept(this);
            emit("{");
            if (n.declarings.Count > 0) {
                AST first = n.declarings[0];
                foreach (AST ast in n.declarings) {
                    if (first != ast) {
                        emit(", ");
                    }
                    Assigning assigning = ast as Assigning;
                    assigning.child.accept(this);
                    //Code = Code.Remove(Code.Length - 2);
                }
            }
            emit("}");
        }

        public override void Visit(StructDef n) {
            emit("{\n");
            if (n.declarings.Count > 0) {
                AST first = n.declarings[0];
                foreach (AST ast in n.declarings) {
                    if (first != ast) {
                        emit("\n");
                    }
                    ast.accept(this);
                    if (ast is SymDeclaring) emit(";");
                }
            }
            emit("}");
        }

        public override void Visit(SymStatments n) {
            throw new NotImplementedException();
        }

        public override void Visit(IfStmt n) {
            emit("if(");
            n.logi_expr.accept(this);
            emit(") {\n");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            emit("}\n");
            if (n.elseIF_Eles is IfStmt) {
                emit("else ");
                n.elseIF_Eles.accept(this);
            } else if (n.elseIF_Eles is ElseStmt) {
                emit("else {");
                n.elseIF_Eles.accept(this);
                emit("}");
            }
        }

        public override void Visit(ElseStmt n) {
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
        }

        public override void Visit(WhileStmt n) {
            emit("while(");
            n.logi_expr.accept(this);
            emit(") {\n");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            emit("}\n");
        }

        public override void Visit(ForStmt n) {
            emit("for(");
            n.stm1.accept(this);
            n.stm2.accept(this);
            emit(" ; ");
            n.stm3.accept(this);
            if (Code[Code.Length - 2] == ';') Code = Code.Remove(Code.Length - 2);
            emit(") {\n");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            emit("}\n");
        }

        public override void Visit(SwitchStmt n) {
            emit($"switch( {n.id} )" + "{\n");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            emit("}\n");
        }

        public override void Visit(SwitchCase n) {
            emit($"case {n.id} :\n");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            //emit("}\n");
        }

        public override void Visit(SwitchDefault n) {
            emit($"default :\n");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            //emit("}\n");
        }

        public override void Visit(FunctionStmt n) {
            if (n.id is DotReferencing dot && dot.id.type == AST.PIN) {
                var pinFunc = dot.dotId as SymReferencing;
                switch (pinFunc.id) {
                    case "pinMode":
                        if (n.param_list[0] is SymConst symConst) {
                            if (pinValue.ContainsKey(dot.id.id)) {
                                pinValue[dot.id.id] = symConst.val;
                            } else pinValue.Add(dot.id.id, symConst.val);
                        } else if (n.param_list[0] is SymReferencing symReferencing) {
                            if (pinValue.ContainsKey(dot.id.id)) {
                                pinValue[dot.id.id] = symReferencing.id;
                            } else pinValue.Add(dot.id.id, symReferencing.id);
                        }
                        emit("pinMode(");
                        AST first = n.param_list[0];
                        foreach (AST ast in n.param_list) {
                            if (first != ast) {
                                emit(", ");
                            }
                            ast.accept(this);
                            IsFuncSTM(ast);
                        }
                        emit(");\n");
                        break;
                    case "digitalPower":
                        emit($"digitalWrite({pinValue[dot.id.id]}, ");
                        n.param_list[0].accept(this);
                        IsFuncSTM(n.param_list[0]);
                        emit(");\n");
                        break;
                    case "analogPower":
                        emit($"analogWrite({pinValue[dot.id.id]}, map(constrain(");
                        n.param_list[0].accept(this);
                        IsFuncSTM(n.param_list[0]);
                        emit(", 0, 100), 0, 100, 0, 255));\n");
                        break;
                    case "powerValue":
                        emit($"analogWrite({pinValue[dot.id.id]}, constrain(");
                        n.param_list[0].accept(this);
                        IsFuncSTM(n.param_list[0]);
                        emit(", 0, 255));\n");
                        break;
                    case "digitalRead":
                        emit($"digitalRead({pinValue[dot.id.id]});\n");
                        break;
                    case "analogRead":
                        emit($"map(analogRead({pinValue[dot.id.id]}), 0, 1023, 0, 100);\n");
                        break;
                    case "analogReadValue":
                        emit($"analogRead({pinValue[dot.id.id]});\n");
                        break;
                }
            } else if (n.id is DotReferencing dotserial && dotserial.id.type == AST.UART && (dotserial.dotId is SymReferencing dotId && (dotId.id == "print" || dotId.id == "printLine"))) {
                if (n.param_list[0] is Expression) {
                    emit($"{serialTempString} = \"\";\n");
                    SerialExpressionPrint(n.param_list[0]);
                    n.id.accept(this);
                    emit($"({serialTempString});\n");
                } else {
                    n.id.accept(this);
                    emit($"(");
                    n.param_list[0].accept(this);
                    IsFuncSTM(n.param_list[0]);
                    emit(");\n");
                }
            } else if (n.id is SymReferencing sym && sym.id == "timer") {
                emit("(millis() % 32767);\n");

            } else {
                n.id.accept(this);
                emit($"(");
                if (n.param_list.Count > 0) {
                    AST first = n.param_list[0];
                    foreach (AST ast in n.param_list) {
                        if (first != ast) {
                            emit(", ");
                        }
                        ast.accept(this);
                        IsFuncSTM(ast);
                    }
                }
                emit(");\n");
            }
        }

        private void SerialExpressionPrint(AST node) {
            if (node is Expression expression && expression.type == AST.STRING) {
                if (expression.childe1 is Expression) {
                    SerialExpressionPrint(expression.childe1);
                } else {
                    AddToSerialTempString(expression.childe1);
                }
                if (expression.childe2 is Expression) {
                    SerialExpressionPrint(expression.childe2);
                } else {
                    AddToSerialTempString(expression.childe2);
                }
            } else {
                AddToSerialTempString(node);
            }
        }

        private void AddToSerialTempString(AST node) {
            emit($"{serialTempString} += ");
            node.accept(this);
            IsFuncSTM(node);
            emit(";\n");
        }

        public override void Visit(Assigning n) {
            /*if (n.id is SymReferencing) {
                SymReferencing sym = n.id as SymReferencing;
                emit($"{sym.id} ");
            } else if (n.id is DotReferencing) {
                n.id.accept(this);
            } else emit($"{n.id} = ");*/
            n.id.accept(this);
            if (!(n.child is StructDef)) emit($" = ");
            n.child.accept(this);
            IsFuncSTM(n.child);
            emit(";\n");
        }

        public override void Visit(SymReferencing n) {
            emit($"{n.id}");
        }

        public override void Visit(DotReferencing n) {
            n.id.accept(this);
            emit($".");
            if (n.id.type == AST.UART && n.dotId is SymReferencing symSerial && symSerial.id == "printLine") {
                symSerial.id = "println";
                symSerial.accept(this);
                symSerial.id = "printLine";
            } else if (n.id.type == AST.PIN && n.dotId is SymReferencing symPin) {
                switch (symPin.id) {
                    case "digitalpower":
                        symPin.id = "digitalWrite";
                        symPin.accept(this);
                        symPin.id = "digitalpower";
                        break;
                    case "analogpower":
                        symPin.id = "analogWrite";
                        symPin.accept(this);
                        symPin.id = "analogpower";
                        break;
                    case "powerValue":
                        symPin.id = "analogWrite";
                        symPin.accept(this);
                        symPin.id = "powerValue";
                        break;
                    case "analogReadValue":
                        symPin.id = "analogRead";
                        symPin.accept(this);
                        symPin.id = "analogReadValue";
                        break;
                    default:
                        symPin.accept(this);
                        break;
                }
            } else n.dotId.accept(this);
        }

        public override void Visit(ListReferencing n) {
            n.id.accept(this);
            foreach (AST item in n.index) {
                emit("[");
                item.accept(this);
                emit("]");
            }
        }

        public override void Visit(BooleanConst n) {
            emit($"{n.val.ToLower()}");
        }

        public override void Visit(IntConst n) {
            emit($"{n.val}");
        }

        public override void Visit(FloatConst n) {
            emit($"{n.val}");
        }

        public override void Visit(StringConst n) {
            emit($"{n.val}");
        }

        public override void Visit(ListConst n) {
            emit("{");
            if (n.declarings.Count > 0) {
                AST first = n.declarings[0];
                foreach (AST ast in n.declarings) {
                    if (first != ast) {
                        emit(", ");
                    }
                    ast.accept(this);
                }
            }
            emit("}");
        }

        public override void Visit(Expression n) {
            n.childe1.accept(this);
            IsFuncSTM(n.childe1);
            emit($" {n.operation} ");
            n.childe2?.accept(this);
            IsFuncSTM(n.childe2);
        }

        public override void Visit(LogiExpression n) {
            n.childe1.accept(this);
            IsFuncSTM(n.childe1);
            emit($" {n.operation} ");
            n.childe2?.accept(this);
            IsFuncSTM(n.childe2);
        }

        private void IsFuncSTM(AST node) {
            if (node is FunctionStmt ||
                (node is ConvertingToBool toBool && toBool.child is FunctionStmt) ||
                (node is ConvertingToFloat toFloat && toFloat.child is FunctionStmt) ||
                (node is ConvertingToString toString && toString.child is FunctionStmt)) {
                Code = Code.Remove(Code.Length - 2);
            }
        }

        public override void Visit(NotExpression n) {
            emit($"!");
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
    }
}
