using System;
using System.Collections.Generic;
using System.Text;

namespace cocor_compiler {
    public class PrettyprintVisitor : Visitor {
        public override void visit(Assigning n) {
            Console.Write($"{n.id} = ");
            n.child1.accept(this);
            Console.WriteLine(" ");
        }

        public override void visit(Expression n) {
            n.childe1?.accept(this);
            Console.Write($" {n.operation} ");
            n.childe2.accept(this);
        }

        public override void visit(ConvertingToFloat n) {
            Console.Write(" i2f ");
            n.child.accept(this);
        }

        public override void visit(FloatConsting n) {
            Console.Write(n.val);
        }

        public override void visit(IntConsting n) {
            Console.Write(n.val);
        }

        public override void visit(Printing n) {
            Console.Write($"p {n.id} ");
        }

        public override void visit(Prog n) {
            foreach (AST ast in n.prog) {
                ast.accept(this);
                //Console.WriteLine();
            }
            Console.WriteLine();
        }

        public override void visit(SymDeclaring n) {
            throw new NotImplementedException();
        }

        public override void visit(FloatDcl n) {
            //Console.Write($"f {n.id} ");
            Console.WriteLine($"f {n.id} ");
        }

        public override void visit(IntDcl n) {
            //Console.Write($"i {n.id} ");
            Console.WriteLine($"i {n.id} ");
        }

        public override void visit(SymReferencing n) {
            Console.Write(n.id);
        }
    }
}
