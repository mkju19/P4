using System;
using System.Collections.Generic;

namespace cocor_compiler {
    class Program {
        public static void Main(string[] arg) {
            string file = @"D:\Google Drive\Software AAU uni 2019-2022\4. semester\Projekt\parser-scanner generator\cocor test\cocor_compiler\prog.txt";
            Scanner scanner = new Scanner(file);
            Parser parser = new Parser(scanner);
            parser.Parse();
            Console.WriteLine($" {parser.errors.count} errors detected");
            parser.ProgramAST.accept(new PrettyprintVisitor());
            Console.WriteLine("  Pretty Printing successful");
            parser.ProgramAST.accept(new SymbolTableFilling());

            string dictionaryString = "{";
            foreach (KeyValuePair<string, int> keyValues in AST.SymbolTable) {
                dictionaryString += keyValues.Key + " : " + keyValues.Value + ", ";
            }
            Console.WriteLine(dictionaryString.TrimEnd(',', ' ') + "}");

            Console.WriteLine("  Symbol Table filling successful");
            parser.ProgramAST.accept(new TypeChecker());
            Console.WriteLine("  Type Checking successful");
            parser.ProgramAST.accept(new PrettyprintVisitor());
            Console.WriteLine("  Pretty Printing successful\n");
            parser.ProgramAST.accept(new CCodeGenerator());
            Console.WriteLine("\n  C Code Generation successful");
        }
    }

}
