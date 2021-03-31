using System;
using System.Collections.Generic;
using System.Text;

namespace cocor_compiler {
    public abstract class AST {
        public static int FLTTYPE = 0, INTTYPE = 1;
        public int type;
        public static Dictionary<string, int> SymbolTable = new Dictionary<string, int>();
        public abstract void accept(Visitor v);
    }

    public class Prog : AST {
        public List<AST> prog;
        public Prog(List<AST> prg) {
            prog = prg;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public abstract class SymDeclaring : AST {
        public string id;
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class IntDcl : SymDeclaring {
        public IntDcl(string i) { id = i; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class FloatDcl : SymDeclaring {
        public FloatDcl(string i) { id = i; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class SymReferencing : AST {
        public string id;
        public SymReferencing(string i) { id = i; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class Assigning : AST {
        public string id;
        public AST child1;
        public Assigning(string i, AST ch1) { id = i; child1 = ch1; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class IntConsting : AST {
        public string val;
        public IntConsting(string v) { val = v; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class FloatConsting : AST {
        public string val;
        public FloatConsting(string v) { val = v; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class Expression : AST {
        public string operation;
        public AST childe1, childe2;
        public Expression(string op, AST ch1, AST ch2) { operation = op; childe1 = ch1; childe2 = ch2; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class Printing : AST {
        public string id;
        public Printing(string c) { id = c; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class ConvertingToFloat : AST {
        public AST child;
        public ConvertingToFloat(AST n) { child = n; }
        public override void accept(Visitor v) { v.visit(this); }
    }
}
