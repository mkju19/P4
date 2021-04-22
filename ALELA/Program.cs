using System;
using System.Collections.Generic;

namespace ALELA_Compiler {
    class Program {
        static void Main(string[] args) {
            ArgsHandler argsHandler = new ArgsHandler(args);
            string file = argsHandler.InFile;
            //string file = "./prog.txt";
            Scanner scanner = new Scanner(file);
            Parser parser = new Parser(scanner);
            parser.Parse();
            Console.WriteLine($" {parser.errors.count} errors detected\n");
            parser.ProgramAST.accept(new Visitors.PrettyprintVisitor());
            Console.WriteLine("  Pretty Printing successful\n");
            parser.ProgramAST.accept(new Visitors.SymbolTableFilling());

            string dictionaryString = "{";
            foreach (KeyValuePair<Tuple<string, string>, int> keyValues in AST.SymbolTable) {
                dictionaryString += keyValues.Key + " : " + keyValues.Value + ", ";
            }
            Console.WriteLine(dictionaryString.TrimEnd(',', ' ') + "}");

            Console.WriteLine("  Symbol Table filling successful\n");
            Visitors.TypeChecker typeChecker = new Visitors.TypeChecker();
            parser.ProgramAST.accept(typeChecker);
            Console.WriteLine("  Type Checking successful\n");
            parser.ProgramAST.accept(new Visitors.InitiationChecker(typeChecker.StructDic));
            Console.WriteLine("  Initiation Checking successful\n");
            parser.ProgramAST.accept(new Visitors.PrettyprintVisitor());
            Console.WriteLine("  Pretty Printing successful\n");
            parser.ProgramAST.accept(new Visitors.CppArduinoCodeGenerator());
            Console.WriteLine("\n  Cpp/Arduino Code Generation successful");
        }
    }
}
