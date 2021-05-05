using Microsoft.VisualStudio.TestTools.UnitTesting;
using ALELA_Compiler;
using ALELA_Compiler.Visitors;
using System;
using System.Collections.Generic;

namespace ALELA_Test {
    [TestClass]
    public class TypeCheckerUnitTest {

        [TestMethod]
        public void AssigningUnitTest() {
            try {
                Assigning assigning = new Assigning(new SymReferencing("a"), new IntConst("4"));
                TypeChecker typeChecker = new TypeChecker();
                AST.SymbolTable =
                    new Dictionary<Tuple<string, string>, int>() {
                        {new Tuple<string, string>("1", "a"), AST.INTTYPE},
                        {new Tuple<string, string>("1", "text"), AST.STRING},
                        {new Tuple<string, string>("1", "testList"), 664},
                        {new Tuple<string, string>("1", "pin"), AST.STRUCT},
                        {new Tuple<string, string>("11", "pinPower"), AST.INTTYPE},
                        {new Tuple<string, string>("111", "value"), AST.INTTYPE}
                    };
                assigning.accept(typeChecker);

                int actual = assigning.type;
                int expected = AST.INTTYPE;
                Assert.IsTrue(actual == expected, "Simple assign faild");

                assigning = new Assigning(new SymReferencing("testList"), new ListConst(new List<AST>() { new ListConst(new List<AST>() { new SymReferencing("text") }), new ListConst(new List<AST>() { new StringConst("\"list\"") }), new ListConst(new List<AST>() { new StringConst("\"thing\"") }) }));
                assigning.accept(typeChecker);

                actual = assigning.type;
                expected = 664;
                Assert.IsTrue(actual == expected, "List assign faild");

                var structDef = new StructDef(new List<AST>() { new IntDcl("pinPower"), new FuncDecl(new VoidDcl("Power"), new List<SymDeclaring>() { new IntDcl("value") }, new List<AST>() { new Assigning(new SymReferencing("pinPower"), new SymReferencing("value")) }, null) });
                structDef.structType = new SymReferencing("pin");
                assigning = new Assigning(new SymReferencing("pin"), structDef);
                assigning.accept(typeChecker);

                actual = assigning.type;
                expected = AST.STRUCT;
                Assert.IsTrue(actual == expected, "StructDef assign faild");
            } finally {
                AST.SymbolTable.Clear();
            }
        }

        [TestMethod]
        public void DotReferencingUnitTest() {
            try {
                DotReferencing dotReferencing = new DotReferencing(new SymReferencing("led"), new SymReferencing("pinPower"));
                TypeChecker typeChecker = new TypeChecker();
                AST.SymbolTable =
                    new Dictionary<Tuple<string, string>, int>() {
                        {new Tuple<string, string>("1", "led"), AST.STRUCT}
                    };
                typeChecker.StructDic.Add("led", new List<Tuple<string, int>>(){ new Tuple<string, int>("pinPower", AST.INTTYPE) });
                dotReferencing.accept(typeChecker);

                int actual = dotReferencing.type;
                int expected = AST.INTTYPE;
                Assert.IsTrue(actual == expected, "DotReferencing faild");
            } finally {
                AST.SymbolTable.Clear();
            }
        }

        [TestMethod]
        public void ListReferencingUnitTest() {
            try {
                ListReferencing listReferencing = new ListReferencing(new SymReferencing("testList"), new List<AST>() { new Expression("-", new IntConst("1"), new IntConst("1")), new IntConst("1") });
                TypeChecker typeChecker = new TypeChecker();
                AST.SymbolTable =
                    new Dictionary<Tuple<string, string>, int>() {
                        {new Tuple<string, string>("1", "testList"), 664}
                    };
                listReferencing.accept(typeChecker);

                int actual = listReferencing.type;
                int expected = AST.STRING;
                Assert.IsTrue(actual == expected, "DotReferencing faild");
            } finally {
                AST.SymbolTable.Clear();
            }
        }

        [TestMethod]
        public void ExpressionUnitTest() {
            try {
                Expression expression = new Expression("+", new IntConst("4"), new Expression("+", new Expression("*", new IntConst("2"), new IntConst("5")), new Expression("%", new Expression("-", new IntConst("2"), new Expression("-", new Expression("/", new IntConst("1"), new IntConst("4")), new IntConst("5"))), new IntConst("5"))));
                TypeChecker typeChecker = new TypeChecker();
                AST.SymbolTable =
                    new Dictionary<Tuple<string, string>, int>() {
                        {new Tuple<string, string>("1", "i"), AST.INTTYPE},
                        {new Tuple<string, string>("1", "f"), AST.FLTTYPE}
                    };
                expression.accept(typeChecker);

                int actual = expression.type;
                int expected = AST.INTTYPE;
                Assert.IsTrue(actual == expected, "Long exp faild");

                expression = new Expression("+", new IntConst("4"), new Expression("+", new Expression("*", new IntConst("2"), new SymReferencing("i")), new Expression("%", new Expression("-", new SymReferencing("f"), new Expression("-", new Expression("/", new IntConst("1"), new IntConst("4")), new IntConst("5"))), new IntConst("5"))));
                expression.accept(typeChecker);

                actual = expression.type;
                expected = AST.FLTTYPE;
                Assert.IsTrue(actual == expected, "Long var convertion exp faild");

                expression = new Expression("+", new SymReferencing("i"), new IntConst("3"));
                expression.accept(typeChecker);

                actual = expression.type;
                expected = AST.INTTYPE;
                Assert.IsTrue(actual == expected, "var exp faild");

                expression = new Expression("+", new SymReferencing("f"), new IntConst("3"));
                expression.accept(typeChecker);

                actual = expression.type;
                expected = AST.FLTTYPE;
                Assert.IsTrue(actual == expected, "var convertion exp faild");

                expression = new Expression("+", new FloatConst("8"), new IntConst("3"));
                expression.accept(typeChecker);

                actual = expression.type;
                expected = AST.FLTTYPE;
                Assert.IsTrue(actual == expected, "convertion exp faild");
            } finally {
                AST.SymbolTable.Clear();
            }
        }
    }
}
