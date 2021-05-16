using System;
using System.Collections.Generic;
using System.Text;

namespace ALELA_Compiler {
    public class STD {
        public void AddTo(Prog prog) {
            prog.prog.InsertRange(0, StandartConsts());
            prog.prog.InsertRange(prog.prog.Count, StandartFuncs());
        }

        public void RemoveFrom(Prog prog) {
            int consts = StandartConsts().Count - 2;
            int funcs = StandartFuncs().Count;
            prog.prog.RemoveRange(0, consts);
            prog.prog.RemoveRange((prog.prog.Count - funcs), funcs);
        }

        private List<AST> StandartConsts() {
             return new List<AST>() {
                new Decl(new BooleanDcl("HIGH"), new Assigning(new SymReferencing("HIGH"), new BooleanConst("TRUE"))),
                new Decl(new BooleanDcl("LOW"), new Assigning(new SymReferencing("LOW"), new BooleanConst("FALSE"))),
                new Decl(new IntDcl("INPUT"), new Assigning(new SymReferencing("INPUT"), new IntConst("0"))),
                new Decl(new IntDcl("OUTPUT"), new Assigning(new SymReferencing("OUTPUT"), new IntConst("1"))),
                new Decl(new IntDcl("INPUT_PULLUP"), new Assigning(new SymReferencing("INPUT_PULLUP"), new IntConst("2"))),
                new Decl(new IntDcl("A0"), new Assigning(new SymReferencing("A0"), new IntConst("14"))),
                new Decl(new IntDcl("A1"), new Assigning(new SymReferencing("A1"), new IntConst("15"))),
                new Decl(new IntDcl("A2"), new Assigning(new SymReferencing("A2"), new IntConst("16"))),
                new Decl(new IntDcl("A3"), new Assigning(new SymReferencing("A3"), new IntConst("17"))),
                new Decl(new IntDcl("A4"), new Assigning(new SymReferencing("A4"), new IntConst("18"))),
                new Decl(new IntDcl("A5"), new Assigning(new SymReferencing("A5"), new IntConst("19"))),
                new Decl(new BooleanDcl("ON"), new Assigning(new SymReferencing("ON"), new BooleanConst("TRUE"))),
                new Decl(new BooleanDcl("OFF"), new Assigning(new SymReferencing("OFF"), new BooleanConst("FALSE"))),
            };
        }
        private List<AST> StandartFuncs() {
            return new List<AST>() {
                new FuncDecl(new VoidDcl("delay"), new List<SymDeclaring>(){ new IntDcl("micros")}, new List<AST>(), null)
            };
        }
    }
}
