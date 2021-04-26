using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler.Visitors {
    class PrettyprintVisitor : Visitor {
        public override void Visit(Prog n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            Console.WriteLine();
        }

        public override void Visit(ProgSetup n) {
            Console.WriteLine("setup {");
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void Visit(ProgLoop n) {
            Console.WriteLine("loop {");
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void Visit(SymDeclaring n) {
            throw new NotImplementedException();
        }

        public override void Visit(VoidDcl n) {
            Console.Write($"void {n.id} ");
        }

        public override void Visit(IntDcl n) {
            Console.Write($"int {n.id} ");
        }

        public override void Visit(FloatDcl n) {
            Console.Write($"float {n.id} ");
        }

        public override void Visit(StringDcl n) {
            Console.Write($"string {n.id} ");
        }

        public override void Visit(BooleanDcl n) {
            Console.Write($"boolean {n.id} ");
        }

        public override void Visit(StructDcl n) {
            Console.Write($"struct {n.id} ");
        }

        public override void Visit(ListDcl n) {
            Console.Write($"List<");
            n.listType.accept(this); // TODO change?
            Console.Write($"> {n.id} ");
        }

        public override void Visit(Decl n) {
            if (n.assigning != null) {
                n.declaring.accept(this);
                Console.CursorLeft -= n.declaring.id.Length + 1;
                n.assigning.accept(this);
            } else {
                n.declaring.accept(this);
                Console.WriteLine(";") ;
            }
        }

        public override void Visit(FuncDecl n) {
            n.declaring.accept(this);
            Console.Write("(");
            if (n.declarings.Count > 0) {
                SymDeclaring first = n.declarings[0];
                foreach (SymDeclaring ast in n.declarings) {
                    if (first != ast) {
                        Console.Write(", ");
                    }
                    ast.accept(this);
                }
            }
            Console.Write(") {\n");
            foreach (AST ast in n.statments) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void Visit(StructDcel n) {
            Console.Write("{");
            if (n.declarings.Count > 0) {
                AST first = n.declarings[0];
                foreach (AST ast in n.declarings) {
                    if (first != ast) {
                        Console.Write(", ");
                    }
                    ast.accept(this);
                }
            }
            Console.Write("}\n");
        }

        public override void Visit(StructDef n) {
            Console.Write("{\n");
            if (n.declarings.Count > 0) {
                AST first = n.declarings[0];
                foreach (AST ast in n.declarings) {
                    if (first != ast) {
                        Console.Write("\n");
                    }
                    ast.accept(this);
                    if (ast is SymDeclaring) Console.Write(";");
                }
            }
            Console.Write("}\n");
        }

        public override void Visit(SymStatments n) {
            throw new NotImplementedException();
        }

        public override void Visit(IfStmt n) {
            Console.Write("if(");
            n.logi_expr.accept(this);
            Console.Write(") {\n");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            Console.WriteLine("}");
            if (n.elseIF_Eles is IfStmt) {
                Console.Write("else ");
                n.elseIF_Eles.accept(this);
            } else if (n.elseIF_Eles is ElseStmt) {
                Console.Write("else {");
                n.elseIF_Eles.accept(this);
                Console.Write("}");
            }
        }

        public override void Visit(ElseStmt n) {
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
        }

        public override void Visit(WhileStmt n) {
            Console.Write("while(");
            n.logi_expr.accept(this);
            Console.Write(") {\n");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void Visit(ForStmt n) {
            Console.Write("for(");
            n.stm1.accept(this);
            n.stm2.accept(this);
            Console.Write(" ; ");
            n.stm3.accept(this);
            Console.Write(") {\n");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void Visit(SwitchStmt n) {
            Console.Write($"switch( {n.id} )" + "{\n");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void Visit(SwitchCase n) {
            Console.WriteLine($"case {n.id}" + "{");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void Visit(SwitchDefault n) {
            Console.WriteLine($"default" + "{");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void Visit(FunctionStmt n) {
            n.id.accept(this);
            Console.Write("(");
            if (n.param_list.Count > 0) {
                AST first = n.param_list[0];
                foreach (AST ast in n.param_list) {
                    if (first != ast) {
                        Console.Write(", ");
                    }
                    ast.accept(this);
                }
            }
            Console.WriteLine(");");
        }

        public override void Visit(Assigning n) {
            if (n.id is SymReferencing) {
                SymReferencing sym = n.id as SymReferencing;
                Console.Write($"{sym.id} ");
            } else if (n.id is DotReferencing) {
                n.id.accept(this);
            } else Console.Write($"{n.id} ");
            if (!(n.child is StructDef || n.child is StructDcel)) Console.Write("= ");
            n.child.accept(this);
            Console.WriteLine(";");
        }

        public override void Visit(SymReferencing n) {
            Console.Write($"{n.id}");
        }

        public override void Visit(DotReferencing n) {
            n.id.accept(this);
            Console.Write($".");
            n.dotId.accept(this);
        }

        public override void Visit(BooleanConst n) {
            Console.Write($"{n.val}");
        }

        public override void Visit(IntConst n) {
            Console.Write($"{n.val}");
        }

        public override void Visit(FloatConst n) {
            Console.Write($"{n.val}");
        }

        public override void Visit(StringConst n) {
            Console.Write($"{n.val}");
        }

        public override void Visit(ListConst n) {
            Console.Write("{");
            if (n.declarings.Count > 0) {
                AST first = n.declarings[0];
                foreach (AST ast in n.declarings) {
                    if (first != ast) {
                        Console.Write(", ");
                    }
                    ast.accept(this);
                }
            }
            Console.Write("}");
        }

        public override void Visit(Expression n) {
            n.childe1.accept(this);
            Console.Write($" {n.operation} ");
            n.childe2?.accept(this);
        }

        public override void Visit(NotExpression n) {
            Console.Write($"!");
            n.childe.accept(this);
        }

        public override void Visit(ConvertingToFloat n) {
            Console.Write(" i2f ");
            n.child.accept(this);
        }

        public override void Visit(ConvertingToBool n) {
            Console.Write(" v2b ");
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
