using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler.Visitors {
    class PrettyprintVisitor : Visitor {
        public string Code = "";
        public void emit(string c) {
            Code += c;
        }
        public override void Visit(Prog n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            emit("\n");
        }

        public override void Visit(ProgSetup n) {
            emit("setup {\n");
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            emit("}\n");
        }

        public override void Visit(ProgLoop n) {
            emit("loop {\n");
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            emit("}\n");
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
            emit($"string {n.id} ");
        }

        public override void Visit(BooleanDcl n) {
            emit($"boolean {n.id} ");
        }

        public override void Visit(StructDcl n) {
            emit($"struct {n.id} ");
        }

        public override void Visit(ListDcl n) {
            emit($"List<");
            n.listType.accept(this);
            emit($"> {n.id} ");
        }

        public override void Visit(Decl n) {
            if (n.assigning != null) {
                n.declaring.accept(this);
                Code = Code.Remove(Code.Length - (n.declaring.id.Length + 1));
                n.assigning.accept(this);
            } else {
                n.declaring.accept(this);
                emit(";\n") ;
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
            emit("}\n");
        }

        public override void Visit(StructDcel n) {
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
            emit("}\n");
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
            emit("}\n");
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
            emit($"case {n.id}" + "{\n");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            emit("}\n");
        }

        public override void Visit(SwitchDefault n) {
            emit($"default" + "{\n");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            emit("}\n");
        }

        public override void Visit(FunctionStmt n) {
            n.id.accept(this);
            emit("(");
            if (n.param_list.Count > 0) {
                AST first = n.param_list[0];
                foreach (AST ast in n.param_list) {
                    if (first != ast) {
                        emit(", ");
                    }
                    ast.accept(this);
                }
            }
            emit(");\n");
        }

        public override void Visit(Assigning n) {
            if (n.id is SymReferencing) {
                SymReferencing sym = n.id as SymReferencing;
                emit($"{sym.id} ");
            } else if (n.id is DotReferencing) {
                n.id.accept(this);
            } else emit($"{n.id} ");
            if (!(n.child is StructDef || n.child is StructDcel)) emit("= ");
            n.child.accept(this);
            emit(";\n");
        }

        public override void Visit(SymReferencing n) {
            emit($"{n.id}");
        }

        public override void Visit(DotReferencing n) {
            n.id.accept(this);
            emit($".");
            n.dotId.accept(this);
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
            emit($"{n.val}");
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
            emit($" {n.operation} ");
            n.childe2?.accept(this);
        }

        public override void Visit(NotExpression n) {
            emit($"!");
            n.childe.accept(this);
        }

        public override void Visit(ConvertingToFloat n) {
            emit(" i2f ");
            n.child.accept(this);
        }

        public override void Visit(ConvertingToBool n) {
            emit(" v2b ");
            n.child.accept(this);
        }

        private string GetType(AST node) {
            if (node is VoidDcl) {
                return "void ";
            } else if (node is IntDcl) {
                return "int ";
            } else if (node is FloatDcl) {
                return "float ";
            } else if (node is StringDcl) {
                return "string ";
            } else if (node is BooleanDcl) {
                return "boolean ";
            } else if (node is ListDcl) {
                return "List ";
            } else {
                return "";
            }
        }
    }
}
