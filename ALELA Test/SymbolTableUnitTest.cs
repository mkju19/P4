using Microsoft.VisualStudio.TestTools.UnitTesting;
using ALELA_Compiler;
using ALELA_Compiler.Visitors;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ALELA_Test {
    [TestClass]
    public class SymbolTableUnitTest {

        [TestMethod]
        public void VoidDclUnitTest() {
            try {
                VoidDcl voidDcl = new VoidDcl("testVar");
                SymbolTableFilling symbolTableFilling = new SymbolTableFilling();
                voidDcl.accept(symbolTableFilling);

                Dictionary<Tuple<string, string>, int> actual = AST.SymbolTable;

                Dictionary<Tuple<string, string>, int> expected =
                    new Dictionary<Tuple<string, string>, int>() {
                        {new Tuple<string, string>("1", "testVar"), 0}
                    };

                Assert.IsTrue(ObjectCompare(actual, expected), "Void Dcl fail");
            } finally {
                AST.SymbolTable.Clear();
            }
        }

        [TestMethod]
        public void ListDclUnitTest() {
            try {
                ListDcl listDcl = new ListDcl("testList", new ListDcl(new StringDcl()));
                SymbolTableFilling symbolTableFilling = new SymbolTableFilling();
                listDcl.accept(symbolTableFilling);

                Dictionary<Tuple<string, string>, int> actual = AST.SymbolTable;

                Dictionary<Tuple<string, string>, int> expected =
                    new Dictionary<Tuple<string, string>, int>() {
                        {new Tuple<string, string>("1", "testList"), 664}
                    };

                Assert.IsTrue(ObjectCompare(actual, expected), "List Dcl faild");
            } finally {
                AST.SymbolTable.Clear();
            }
        }

        [TestMethod]
        public void FuncDeclUnitTest() {
            try {
                FuncDecl funcDecl =
                    new FuncDecl(
                        new IntDcl("testFunc"), 
                        new List<SymDeclaring>() { new IntDcl("numi"), new FloatDcl("numf") },
                        new List<AST>() { new Decl(new IntDcl("ffs"), new Assigning(new SymReferencing("ffs"), new FloatConst("1337"))) },
                        new Expression("+", new SymReferencing("ffs"), new SymReferencing("numi")));

                SymbolTableFilling symbolTableFilling = new SymbolTableFilling();
                funcDecl.accept(symbolTableFilling);

                Dictionary<Tuple<string, string>, int> actual = AST.SymbolTable;

                Dictionary<Tuple<string, string>, int> expected =
                    new Dictionary<Tuple<string, string>, int>() {
                        {new Tuple<string, string>("1", "testFunc"), 2},
                        {new Tuple<string, string>("11", "numi"), 2},
                        {new Tuple<string, string>("11", "numf"), 3},
                        {new Tuple<string, string>("11", "ffs"), 2}
                    };

                Assert.IsTrue(ObjectCompare(actual, expected), "Function dcl faild");
            } finally {
                AST.SymbolTable.Clear();
            }
        }

        [TestMethod]
        public void StructDefUnitTest() {
            try {
                StructDef structDef =
                    new StructDef(
                        new List<AST>() {
                            new IntDcl("pinPower"),
                            new FuncDecl(new VoidDcl("Power"),
                                new List<SymDeclaring>() { new IntDcl("value") },
                                new List<AST>() { 
                                    new Assigning(new SymReferencing("pinPower"), new SymReferencing("value")) },
                                null)
                        });
                structDef.structType = new SymReferencing("pin");
                SymbolTableFilling symbolTableFilling = new SymbolTableFilling();
                structDef.accept(symbolTableFilling);

                Dictionary<Tuple<string, string>, int> actual = AST.SymbolTable;

                Dictionary<Tuple<string, string>, int> expected =
                    new Dictionary<Tuple<string, string>, int>() {
                        {new Tuple<string, string>("11", "pinPower"), 2},
                        {new Tuple<string, string>("11", "Power"), 0},
                        {new Tuple<string, string>("111", "value"), 2}
                    };

                Assert.IsTrue(ObjectCompare(actual, expected), "Struct Def faild");
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
