using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler {
    public abstract class AST {
        public static int VOID = 0, INTTYPE = 1, FLTTYPE = 2, STRING = 3, BOOLEAN = 4;
        public int type;
        public static Dictionary<Tuple<string, string>, int> SymbolTable = new Dictionary<Tuple<string, string>, int>();
        public abstract void accept(Visitor v);
    }

    public class Prog : AST {
        public List<AST> prog;
        public Prog(List<AST> prg) {
            prog = prg;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class ProgSetup : AST {
        public List<AST> prog;
        public ProgSetup(List<AST> prg) {
            prog = prg;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class ProgLoop : AST {
        public List<AST> prog;
        public ProgLoop(List<AST> prg) {
            prog = prg;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public abstract class SymDeclaring : AST {
        public string id;
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class VoidDcl : SymDeclaring {
        public VoidDcl() { }
        public VoidDcl(string i) { id = i; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class IntDcl : SymDeclaring {
        public IntDcl() {}
        public IntDcl(string i) { id = i; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class FloatDcl : SymDeclaring {
        public FloatDcl() {}
        public FloatDcl(string i) { id = i; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class StringDcl : SymDeclaring {
        public StringDcl() {}
        public StringDcl(string i) { id = i; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class BooleanDcl : SymDeclaring {
        public BooleanDcl() {}
        public BooleanDcl(string i) { id = i; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class Decl : AST {
        public SymDeclaring declaring;
        public AST assigning;
        public Decl(SymDeclaring Declaring, AST Assigning) {
            declaring = Declaring;
            assigning = Assigning;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class FuncDecl : AST {
        public SymDeclaring declaring;
        public List<SymDeclaring> declarings;
        public List<AST> statments;
        public FuncDecl(SymDeclaring Declaring, List<SymDeclaring> Declarings, List<AST> Statments) {
            declaring = Declaring;
            declarings = Declarings;
            statments = Statments;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public abstract class SymStatments : AST {
        //public SymStatments(string i) { id = i; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class IfStmt : SymStatments {
        public AST logi_expr;
        public List<AST> stmt_list;
        public AST elseIF_Eles;
        public IfStmt(AST Logi_expr, List<AST> Stmt_list, AST ElseIF_Eles) {
            logi_expr = Logi_expr;
            stmt_list = Stmt_list;
            elseIF_Eles = ElseIF_Eles;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class ElseStmt : SymStatments {
        public List<AST> stmt_list;
        public ElseStmt(List<AST> Stmt_list) {
            stmt_list = Stmt_list;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class WhileStmt : SymStatments {
        public AST logi_expr;
        public List<AST> stmt_list;
        public WhileStmt(AST Logi_expr, List<AST> Stmt_list) {
            logi_expr = Logi_expr;
            stmt_list = Stmt_list;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class ForStmt : SymStatments {
        public AST stm1, stm2, stm3;
        public List<AST> stmt_list;
        public ForStmt(AST Stm1, AST Stm2, AST Stm3, List<AST> Stmt_list) {
            stm1 = Stm1;
            stm2 = Stm2;
            stm3 = Stm3;
            stmt_list = Stmt_list;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class SwitchStmt : SymStatments {
        public string id;
        public List<AST> stmt_list;
        public SwitchStmt(string Id, List<AST> Stmt_list) {
            id = Id;
            stmt_list = Stmt_list;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class SwitchCase : AST {
        public string id;
        public List<AST> stmt_list;
        public SwitchCase(string Id, List<AST> Stmt_list) {
            id = Id;
            stmt_list = Stmt_list;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class SwitchDefault : AST {
        public List<AST> stmt_list;
        public SwitchDefault(List<AST> Stmt_list) {
            stmt_list = Stmt_list;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class FunctionStmt : SymStatments {
        public string id;
        public List<AST> param_list;
        public FunctionStmt(string Id, List<AST> Param_list) {
            id = Id;
            param_list = Param_list;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class Assigning : SymStatments {
        public string id;
        public AST child;
        public Assigning(string i, AST ch1) { id = i; child = ch1; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class SymReferencing : AST {
        public string id;
        public SymReferencing(string i) { id = i; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class IntConst : AST {
        public string val;
        public IntConst(string v) { val = v; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class FloatConst : AST {
        public string val;
        public FloatConst(string v) { val = v; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class StringConst : AST {
        public string val;
        public StringConst(string v) { val = v; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class BooleanConst : AST {
        public string val;
        public BooleanConst(string v) { val = v; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class Expression : AST {
        public string operation;
        public AST childe1, childe2;
        public Expression(string op, AST ch1, AST ch2) {
            operation = op;
            childe1 = ch1;
            childe2 = ch2;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class NotExpression : AST {
        public AST childe;
        public NotExpression(AST ch1) {
            childe = ch1;
        }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class ConvertingToFloat : AST {
        public AST child;
        public ConvertingToFloat(AST n) { child = n; }
        public override void accept(Visitor v) { v.visit(this); }
    }

    public class ConvertingToBool : AST {
        public AST child;
        public ConvertingToBool(AST n) { child = n; }
        public override void accept(Visitor v) { v.visit(this); }
    }
}
