using Microsoft.VisualStudio.TestTools.UnitTesting;
using ALELA_Compiler;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ALELA_Test {
    [TestClass]
    public class CocoUnitTest {
        [TestMethod]
        public void ASTTestMethod() {
            string code = "TestCode.txt";
            Scanner scanner = new Scanner(code);
            Parser parser = new Parser(scanner);
            parser.Parse();
            Prog actual = parser.ProgramAST;

            var structDef6 = new StructDef(new List<AST>() { new IntDcl("pinPower"), new FuncDecl(new VoidDcl("Power"), new List<SymDeclaring>() { new IntDcl("value") }, new List<AST>() { new Assigning(new SymReferencing("pinPower"), new SymReferencing("value")) }, null) });
            structDef6.structType = new SymReferencing("pin");

            var structDcel7 = new StructDcel(new SymReferencing("pin"), new List<AST>() { new Assigning(new SymReferencing("pinPower"), new IntConst("50")) });
            structDcel7.structId = new SymReferencing("led");

            Prog expected = new Prog(new List<AST>() {
                new Decl(new IntDcl("a"), new Assigning(new SymReferencing("a"), new IntConst("4"))), //int a = 4;
                new Decl(new FloatDcl("f"), new Assigning(new SymReferencing("f"), new FloatConst("2.5"))), //float f = 2.5;
                new Decl(new BooleanDcl("asd"), new Assigning(new SymReferencing("asd"), new BooleanConst("TRUE"))), //boolean asd = TRUE;
                new Decl(new StringDcl("text"), new Assigning(new SymReferencing("text"), new StringConst("\"th8is a 2 test !\""))), //string text = "th8is a 2 test !";
                new Decl(new ListDcl("testList", new ListDcl(new StringDcl())), null), //List<List<string>> testList;
                new Decl(new StructDcl("pin"), new Assigning(new SymReferencing("pin"), structDef6)),
                new Decl(new StructDcl("led"), new Assigning(new SymReferencing("led"), structDcel7)), //struct pin led{pinPower = 50};
                new Decl(new IntDcl("ledPin"), new Assigning(new SymReferencing("ledPin"), new DotReferencing(new SymReferencing("led"), new SymReferencing("pinPower")))), // int ledPin = led.pinNum;
                new ProgSetup(new List<AST>(){
                    new Assigning(new DotReferencing(new SymReferencing("led"), new SymReferencing("pinPower")), new IntConst("100")),
                    new Decl(new IntDcl("b"), new Assigning(new SymReferencing("b"), new Expression("+", new IntConst("4"), new Expression("+", new Expression("*", new IntConst("2"), new IntConst("5")), new Expression("%", new Expression("-", new IntConst("2"), new Expression("-", new Expression("/", new IntConst("1"), new IntConst("4")), new IntConst("5"))), new IntConst("5")))))),
                    new ForStmt(new Decl(new IntDcl("i"), new Assigning(new SymReferencing("i"), new IntConst("0"))), new LogiExpression("<=", new SymReferencing("i"), new IntConst("10")), new Assigning(new SymReferencing("i"), new Expression("+", new SymReferencing("i"), new IntConst("1"))), new List<AST>() { new Assigning(new SymReferencing("f"), new Expression("+", new SymReferencing("f"), new IntConst("3")))})
                    }),
                new ProgLoop(new List<AST>(){
                    new Decl(new IntDcl("q"), new Assigning(new SymReferencing("q"), new IntConst("1"))),
                    new Decl(new FloatDcl("b"), new Assigning(new SymReferencing("b"), new FloatConst("3"))),
                    new Assigning(new SymReferencing("a"), new Expression("+", new SymReferencing("a"), new FunctionStmt(new SymReferencing("test"), new List<AST>(){ new SymReferencing("q"), new SymReferencing("b") })))
                }),
                new FuncDecl(new IntDcl("test"), new List<SymDeclaring>(){ new IntDcl("numi"), new FloatDcl("numf") }, new List<AST>(){ new Decl(new IntDcl("ffs"), new Assigning(new SymReferencing("ffs"), new FloatConst("1337"))) }, new Expression("+", new SymReferencing("ffs"), new SymReferencing("numi")))
            });

            Assert.IsTrue(parser.errors.count == 0, "Parser encountered errors");
            Assert.IsTrue(ObjectCompare(expected, actual), "Parsing faild");
        }

        private bool ObjectCompare(object o1, object o2) {
            if ((o1 == null) || (o2 == null)) return false;
            if (ReferenceEquals(o1, o2)) return true;
            if (o1.GetType() != o2.GetType()) return false;

            var objJson = JsonConvert.SerializeObject(o1);
            var anotherJson = JsonConvert.SerializeObject(o2);

            return objJson == anotherJson;
        }
    }
}
