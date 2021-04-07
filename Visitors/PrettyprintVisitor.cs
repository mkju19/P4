using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler {
    class PrettyprintVisitor : Visitor {
        public override void visit(Prog n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            Console.WriteLine();
        }

        public override void visit(ProgSetup n) {
            Console.WriteLine("setup {");
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void visit(ProgLoop n) {
            Console.WriteLine("loop {");
            foreach (AST ast in n.prog) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void visit(SymDeclaring n) {
            throw new NotImplementedException();
        }

        public override void visit(VoidDcl n) {
            Console.Write($"void {n.id} ");
        }

        public override void visit(IntDcl n) {
            Console.Write($"int {n.id} ");
        }

        public override void visit(FloatDcl n) {
            Console.Write($"float {n.id} ");
        }

        public override void visit(StringDcl n) {
            Console.Write($"string {n.id} ");
        }

        public override void visit(BooleanDcl n) {
            Console.Write($"Boolean {n.id} ");
        }

        public override void visit(Decl n) {
            if (n.assigning != null) {
                if (n.declaring is VoidDcl) {
                    Console.Write("void ");
                } else if (n.declaring is IntDcl) {
                    Console.Write("int ");
                } else if (n.declaring is FloatDcl) {
                    Console.Write("float ");
                } else if (n.declaring is StringDcl) {
                    Console.Write("string ");
                } else if (n.declaring is BooleanDcl) {
                    Console.Write("boolean ");
                }
                n.assigning.accept(this);
            } else {
                n.declaring.accept(this);
                Console.WriteLine(";") ;
            }
        }

        public override void visit(FuncDecl n) {
            n.declaring.accept(this);
            Console.Write("(");
            foreach (SymDeclaring ast in n.declarings) {
                ast.accept(this);
            }
            Console.Write(") {\n");
            foreach (AST ast in n.statments) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void visit(SymStatments n) {
            throw new NotImplementedException();
        }

        public override void visit(IfStmt n) {
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

        public override void visit(ElseStmt n) {
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
        }

        public override void visit(WhileStmt n) {
            Console.Write("while(");
            n.logi_expr.accept(this);
            Console.Write(") {\n");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void visit(ForStmt n) {
            Console.Write("for(");
            n.num_from.accept(this);
            Console.Write(" to ");
            n.num_to.accept(this);
            Console.Write(") {\n");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void visit(SwitchStmt n) {
            Console.Write($"switch( {n.id} )" + "{\n");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void visit(SwitchCase n) {
            Console.WriteLine($"case {n.id}" + "{");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void visit(SwitchDefault n) {
            Console.WriteLine($"default" + "{");
            foreach (AST ast in n.stmt_list) {
                ast.accept(this);
            }
            Console.WriteLine("}");
        }

        public override void visit(FunctionStmt n) {
            Console.Write($"{n.id}(");
            foreach (AST ast in n.param_list) {
                ast.accept(this);
            }
            Console.WriteLine(");");
        }

        public override void visit(Assigning n) {
            Console.Write($"{n.id} = ");
            n.child.accept(this);
            Console.WriteLine(";");
        }

        public override void visit(SymReferencing n) {
            Console.Write($"{n.id}");
        }

        public override void visit(IntConst n) {
            Console.Write($"{n.val}");
        }

        public override void visit(FloatConst n) {
            Console.Write($"{n.val}");
        }

        public override void visit(StringConst n) {
            Console.Write($"{n.val}");
        }

        public override void visit(BooleanConst n) {
            Console.Write($"{n.val}");
        }

        public override void visit(Expression n) {
            n.childe1.accept(this);
            Console.Write($" {n.operation} ");
            n.childe2?.accept(this);
        }

        public override void visit(NotExpression n) {
            Console.Write($"!");
            n.childe.accept(this);
        }

        public override void visit(ConvertingToFloat n) {
            Console.Write(" i2f ");
            n.child.accept(this);
        }

        public override void visit(ConvertingToBool n) {
            Console.Write(" v2b ");
            n.child.accept(this);
        }
    }
}
