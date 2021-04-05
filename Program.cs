using System;
using System.Collections.Generic;

namespace ALELA_Compiler {
    class Program {
        static void Main(string[] args) {
            string file = "./prog.txt";
            Scanner scanner = new Scanner(file);
            Parser parser = new Parser(scanner);
            parser.Parse();
            Console.WriteLine($" {parser.errors.count} errors detected\n");
            parser.ProgramAST.accept(new PrettyprintVisitor());
            Console.WriteLine("  Pretty Printing successful\n");
            parser.ProgramAST.accept(new SymbolTableFilling());

            string dictionaryString = "{";
            foreach (KeyValuePair<string, int> keyValues in AST.SymbolTable) {
                dictionaryString += keyValues.Key + " : " + keyValues.Value + ", ";
            }
            Console.WriteLine(dictionaryString.TrimEnd(',', ' ') + "}");

            Console.WriteLine("  Symbol Table filling successful\n");
            parser.ProgramAST.accept(new TypeChecker());
            Console.WriteLine("  Type Checking successful\n");
            parser.ProgramAST.accept(new PrettyprintVisitor());
            Console.WriteLine("  Pretty Printing successful\n");
            /*parser.ProgramAST.accept(new CCodeGenerator());
            Console.WriteLine("\n  C Code Generation successful");*/
        }
    }
}
