using Microsoft.VisualStudio.TestTools.UnitTesting;
using ALELA_Compiler;
using ALELA_Compiler.Visitors;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ALELA_Test {
    [TestClass]
    public class InitiationCheckerUnitTest {

        [TestMethod]
        public void AssigningUnitTest() {
            try {
                AST.SymbolTable =
                    new Dictionary<Tuple<string, string>, int>() {
                        {new Tuple<string, string>("1", "text"), AST.STRING},
                        {new Tuple<string, string>("1", "testList"), 664},
                        {new Tuple<string, string>("1", "pin"), AST.STRUCT},
                        {new Tuple<string, string>("11", "pinPower"), AST.INTTYPE},
                        {new Tuple<string, string>("111", "value"), AST.INTTYPE}
                    };
                Dictionary<string, List<Tuple<string, int>>> structDic =
                    new Dictionary<string, List<Tuple<string, int>>>() {
                        {"pin", new List<Tuple<string, int>>(){
                            new Tuple<string, int>("pinPower", AST.INTTYPE),
                            new Tuple<string, int>("Power", AST.VOID)}
                        },
                        {"led", new List<Tuple<string, int>>(){
                            new Tuple<string, int>("pinPower", AST.INTTYPE),
                            new Tuple<string, int>("Power", AST.VOID)}
                        }
                    };
                InitiationChecker initiationChecker = new InitiationChecker(structDic);

                Assigning assigning = new Assigning(new SymReferencing("text"), new StringConst("4"));
                assigning.accept(initiationChecker);

                Dictionary<Tuple<string, string>, int> actualSym = initiationChecker.getInitiationTable;
                Dictionary<string, List<Tuple<string, int>>> actualStruct = initiationChecker.getStructDic;
                Dictionary<Tuple<string, string>, int> expectedSym =
                    new Dictionary<Tuple<string, string>, int>() {
                        {new Tuple<string, string>("1", "text"), 1},
                        {new Tuple<string, string>("1", "testList"), 0},
                        {new Tuple<string, string>("1", "pin"), 0},
                        {new Tuple<string, string>("11", "pinPower"), 0},
                        {new Tuple<string, string>("111", "value"), 0}
                    };
                Dictionary<string, List<Tuple<string, int>>> expectedStruct =
                    new Dictionary<string, List<Tuple<string, int>>>() {
                        {"pin", new List<Tuple<string, int>>(){
                            new Tuple<string, int>("pinPower", 0),
                            new Tuple<string, int>("Power", 0)}
                        },
                        {"led", new List<Tuple<string, int>>(){
                            new Tuple<string, int>("pinPower", 0),
                            new Tuple<string, int>("Power", 0)}
                        }
                    };
                Assert.IsTrue(ObjectCompare(actualSym, expectedSym) && ObjectCompare(actualStruct, expectedStruct), "Simple InitiationCheck faild");

                assigning = new Assigning(new SymReferencing("testList"), new ListConst(new List<AST>() { new ListConst(new List<AST>() { new SymReferencing("text") }), new ListConst(new List<AST>() { new StringConst("\"list\"") }), new ListConst(new List<AST>() { new StringConst("\"thing\"") }) }));
                assigning.accept(initiationChecker);

                actualSym = initiationChecker.getInitiationTable;
                actualStruct = initiationChecker.getStructDic;
                expectedSym =
                    new Dictionary<Tuple<string, string>, int>() {
                        {new Tuple<string, string>("1", "text"), 1},
                        {new Tuple<string, string>("1", "testList"), 1},
                        {new Tuple<string, string>("1", "pin"), 0},
                        {new Tuple<string, string>("11", "pinPower"), 0},
                        {new Tuple<string, string>("111", "value"), 0}
                    };
                expectedStruct =
                    new Dictionary<string, List<Tuple<string, int>>>() {
                        {"pin", new List<Tuple<string, int>>(){
                            new Tuple<string, int>("pinPower", 0),
                            new Tuple<string, int>("Power", 0)}
                        },
                        {"led", new List<Tuple<string, int>>(){
                            new Tuple<string, int>("pinPower", 0),
                            new Tuple<string, int>("Power", 0)}
                        }
                    };
                Assert.IsTrue(ObjectCompare(actualSym, expectedSym) && ObjectCompare(actualStruct, expectedStruct), "List InitiationCheck faild");

                var structDef = new StructDef(new List<AST>() { new IntDcl("pinPower"), new FuncDecl(new VoidDcl("Power"), new List<SymDeclaring>() { new IntDcl("value") }, new List<AST>() { new Assigning(new SymReferencing("pinPower"), new SymReferencing("value")) }, null) });
                structDef.structType = new SymReferencing("pin");
                assigning = new Assigning(new SymReferencing("pin"), structDef);
                assigning.accept(initiationChecker);

                actualSym = initiationChecker.getInitiationTable;
                actualStruct = initiationChecker.getStructDic;
                expectedSym =
                    new Dictionary<Tuple<string, string>, int>() {
                        {new Tuple<string, string>("1", "text"), 1},
                        {new Tuple<string, string>("1", "testList"), 1},
                        {new Tuple<string, string>("1", "pin"), 1}
                    };
                expectedStruct =
                    new Dictionary<string, List<Tuple<string, int>>>() {
                        {"pin", new List<Tuple<string, int>>(){
                            new Tuple<string, int>("pinPower", 0),
                            new Tuple<string, int>("Power", 1)}
                        },
                        {"led", new List<Tuple<string, int>>(){
                            new Tuple<string, int>("pinPower", 0),
                            new Tuple<string, int>("Power", 0)}
                        }
                    };
                Assert.IsTrue(ObjectCompare(actualSym, expectedSym) && ObjectCompare(actualStruct, expectedStruct), "StructDef InitiationCheck faild");

                var structDcel = new StructDecl(new SymReferencing("pin"), new List<AST>() { new Assigning(new SymReferencing("pinPower"), new IntConst("50")) });
                structDcel.structId = new SymReferencing("led");
                assigning = new Assigning(new SymReferencing("led"), structDcel);
                assigning.accept(initiationChecker);

                actualSym = initiationChecker.getInitiationTable;
                actualStruct = initiationChecker.getStructDic;
                expectedSym =
                    new Dictionary<Tuple<string, string>, int>() {
                        {new Tuple<string, string>("1", "text"), 1},
                        {new Tuple<string, string>("1", "testList"), 1},
                        {new Tuple<string, string>("1", "pin"), 1},
                        {new Tuple<string, string>("1", "led"), 1}
                    };
                expectedStruct =
                    new Dictionary<string, List<Tuple<string, int>>>() {
                        {"pin", new List<Tuple<string, int>>(){
                            new Tuple<string, int>("Power", 1),
                            new Tuple<string, int>("pinPower", 1)}
                        },
                        {"led", new List<Tuple<string, int>>(){
                            new Tuple<string, int>("Power", 1),
                            new Tuple<string, int>("pinPower", 1) }
                        }
                    };
                Assert.IsTrue(ObjectCompare(actualSym, expectedSym) && ObjectCompare(actualStruct, expectedStruct), "StructDcel InitiationCheck faild");
            } finally {
                AST.SymbolTable.Clear();
            }
        }

        [TestMethod]
        public void DotReferencingUnitTest() {
            try {
                AST.SymbolTable = new Dictionary<Tuple<string, string>, int>();
                Dictionary<string, List<Tuple<string, int>>> structDic =
                    new Dictionary<string, List<Tuple<string, int>>>() {
                        {"led", new List<Tuple<string, int>>(){
                            new Tuple<string, int>("pinPower", AST.INTTYPE),
                            new Tuple<string, int>("Power", AST.VOID)}
                        }
                    };
                InitiationChecker initiationChecker = new InitiationChecker(structDic);

                var assign = new Assigning(new DotReferencing(new SymReferencing("led"), new SymReferencing("pinPower")), new IntConst("50"));
                assign.accept(initiationChecker);
                DotReferencing dotReferencing = new DotReferencing(new SymReferencing("led"), new SymReferencing("pinPower"));
                dotReferencing.accept(initiationChecker);

                Dictionary<Tuple<string, string>, int>  actualSym = initiationChecker.getInitiationTable;
                Dictionary<string, List<Tuple<string, int>>> actualStruct = initiationChecker.getStructDic;
                Dictionary<Tuple<string, string>, int>  expectedSym = new Dictionary<Tuple<string, string>, int>();
                Dictionary<string, List<Tuple<string, int>>>  expectedStruct =
                    new Dictionary<string, List<Tuple<string, int>>>() {
                        {"led", new List<Tuple<string, int>>(){
                            new Tuple<string, int>("Power", 0),
                            new Tuple<string, int>("pinPower", 1) }
                        }
                    };
                Assert.IsTrue(ObjectCompare(actualSym, expectedSym) && ObjectCompare(actualStruct, expectedStruct), "DotReferencing faild");
            } finally {
                AST.SymbolTable.Clear();
            }
        }

        [TestMethod]
        public void UnusedVariablesUnitTest() {
            try {
                AST.SymbolTable =
                    new Dictionary<Tuple<string, string>, int>() {
                            {new Tuple<string, string>("1", "text"), AST.STRING},
                            {new Tuple<string, string>("1", "testList"), 664},
                            {new Tuple<string, string>("1", "pin"), AST.STRUCT},
                            {new Tuple<string, string>("11", "pinPower"), AST.INTTYPE},
                            {new Tuple<string, string>("111", "value"), AST.INTTYPE}
                    };
                Dictionary<string, List<Tuple<string, int>>> structDic =
                    new Dictionary<string, List<Tuple<string, int>>>() {
                            {"pin", new List<Tuple<string, int>>(){
                                new Tuple<string, int>("pinPower", AST.INTTYPE),
                                new Tuple<string, int>("Power", AST.VOID)}
                            },
                            {"led", new List<Tuple<string, int>>(){
                                new Tuple<string, int>("pinPower", AST.INTTYPE),
                                new Tuple<string, int>("Power", AST.VOID)}
                            }
                    };
                InitiationChecker initiationChecker = new InitiationChecker(structDic);
                string actual = initiationChecker.UnusedVariables();
                string expected = "Variable text at 1 is not used / initiated\nVariable testList at 1 is not used / initiated\nVariable pin at 1 is not used / initiated\nVariable pinPower at 11 is not used / initiated\nVariable value at 111 is not used / initiated\nVariable pin.pinPower is not used / initiated\nVariable pin.Power is not used / initiated\nVariable led.pinPower is not used / initiated\nVariable led.Power is not used / initiated\n";

                Assert.AreEqual(actual, expected, "UnusedVariables function faild");

            } finally {
                AST.SymbolTable.Clear();
            }
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
