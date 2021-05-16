using System;
using System.Collections.Generic;
using CommandLine;

namespace ALELA_Compiler {
    class Program {
        static void Main(string[] args) {
            //TODO finis args handler and file output
            //var result = CommandLine.Parser.Default.ParseArguments<Options>(args);
            ArgsHandler argsHandler = new ArgsHandler(args);
            string file = argsHandler.InFile;
            //string file = "./prog.txt";
            Scanner scanner = new Scanner(file);
            Parser parser = new Parser(scanner);
            STD std = new STD();
            parser.Parse();
            std.AddTo(parser.ProgramAST);
            Console.WriteLine($" {parser.errors.count} errors detected\n");

            if (parser.errors.count == 0) {
                //try {
                    Visitors.PrettyprintVisitor prettyprint = new Visitors.PrettyprintVisitor();
                    if (argsHandler.verbose == true) {
                        parser.ProgramAST.accept(prettyprint);
                        Console.WriteLine(prettyprint.Code);
                        Console.WriteLine("  Pretty Printing successful\n");
                    }

                    Visitors.SymbolTableFilling symbolTable = new Visitors.SymbolTableFilling();
                    parser.ProgramAST.accept(symbolTable);
                    Console.WriteLine(symbolTable.PrintSymbolTable());
                    Console.WriteLine("  Symbol Table filling successful");

                    Visitors.TypeChecker typeChecker = new Visitors.TypeChecker();
                    parser.ProgramAST.accept(typeChecker);
                    Console.WriteLine("  Type Checking successful");

                    Visitors.InitiationChecker initiationChecker = new Visitors.InitiationChecker(typeChecker.StructDic);
                    parser.ProgramAST.accept(initiationChecker);
                    Console.WriteLine("  Initiation Checking successful\n");

                    if (argsHandler.verbose == true) {
                        prettyprint.Code = "";
                        parser.ProgramAST.accept(prettyprint);
                        Console.WriteLine(prettyprint.Code);
                        Console.WriteLine("  Pretty Printing successful\n");
                    }

                    if (argsHandler.arduinoCode == true) {
                        Visitors.CppArduinoCodeGenerator cppArduinoCodeGenerator = new Visitors.CppArduinoCodeGenerator(parser.ProgramAST);
                        parser.ProgramAST.accept(cppArduinoCodeGenerator);
                        Console.WriteLine(cppArduinoCodeGenerator.Code);
                        Console.WriteLine("\n  Cpp/Arduino Code Generation successful");
                    }

                    Console.WriteLine(initiationChecker.UnusedVariables());

                /*} catch (Exception e) {
                    Console.WriteLine(e.Message);
                }*/

            }
        }
    }
}
