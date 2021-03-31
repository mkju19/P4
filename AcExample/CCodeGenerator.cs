using System;
using System.Collections.Generic;
using System.Text;

namespace cocor_compiler {
    class CCodeGenerator : Visitor {
        string code = "";
        public void emit(string c) {
            code = code + c;
        }

        public override void visit(Assigning n) {
            emit(n.id + " = ");
            n.child1.accept(this);
            emit(";\n");
        }

        public override void visit(Expression n) {
            n.childe1.accept(this);
            emit(" " + n.operation + " ");
            n.childe2.accept(this);
        }

        public override void visit(ConvertingToFloat n) {
            n.child.accept(this);
        }

        public override void visit(FloatConsting n) {
            emit(" " + n.val + " ");
        }

        public override void visit(IntConsting n) {
            emit(" " + n.val + " ");
        }

        public override void visit(Printing n) {
            if (n.type == AST.INTTYPE) emit("printf(\"%d\",");
            else emit("printf(\"%1.5f\",");
            emit(n.id);
            emit(");\n");
        }

        public override void visit(Prog n) {
            emit("#include < stdio.h>\n\n");
            emit("void main()\n{\n");
            foreach (AST ast in n.prog) {
                ast.accept(this);
            };
            emit("return 0;");
            emit("\n}");

            Console.WriteLine(code);
        }

        public override void visit(SymDeclaring n) {
            //throw new NotImplementedException();
        }

        public override void visit(FloatDcl n) {
            emit("float " + n.id + ";\n");
        }

        public override void visit(IntDcl n) {
            emit("int " + n.id + ";\n");
        }

        public override void visit(SymReferencing n) {
            emit(" " + n.id + " ");
        }
    }
}
