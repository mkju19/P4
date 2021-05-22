using System;
using System.IO;
using System.Text;
using ALELA_Compiler.Visitors;
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
            Prog ProgramAST;

            STD std = new STD();
            parser.Parse();
            std.AddTo(parser.ProgramAST);
            ProgramAST = parser.ProgramAST;
            Console.WriteLine($" {parser.errors.count} errors detected\n");

            if (parser.errors.count == 0) {
                //try {
                    PrettyprintVisitor prettyprint = new PrettyprintVisitor();
                    if (argsHandler.verbose == true) {
                        ProgramAST.accept(prettyprint);
                        Console.WriteLine(prettyprint.Code);
                        Console.WriteLine("  Pretty Printing successful\n");
                    }

                    SymbolTableFilling symbolTable = new SymbolTableFilling();
                    ProgramAST.accept(symbolTable);
                    if (argsHandler.verbose == true)  Console.WriteLine(symbolTable.PrintSymbolTable());
                    Console.WriteLine("  Symbol Table filling successful");

                    TypeChecker typeChecker = new TypeChecker();
                    ProgramAST.accept(typeChecker);
                    Console.WriteLine("  Type Checking successful");

                    InitiationChecker initiationChecker = new InitiationChecker(typeChecker.StructDic);
                    ProgramAST.accept(initiationChecker);
                    Console.WriteLine("  Initiation Checking successful\n");

                    if (argsHandler.verbose == true) {
                        prettyprint.Code = "";
                        ProgramAST.accept(prettyprint);
                        Console.WriteLine(prettyprint.Code);
                        Console.WriteLine("  Pretty Printing successful\n");
                    }

                    if (argsHandler.arduinoCode == true) {
                        var cppArduinoCodeGenerator = new CppArduinoCodeGenerator(ProgramAST);
                        ProgramAST.accept(cppArduinoCodeGenerator);
                        if (argsHandler.createFile == true) {
                            string currentPath = Directory.GetCurrentDirectory();
                            using (FileStream fs = File.Create(Path.Combine(currentPath, argsHandler.OutFile))) {
                                byte[] info = new UTF8Encoding(true).GetBytes(cppArduinoCodeGenerator.Code);
                                fs.Write(info, 0, info.Length);
                            }
                        } else Console.WriteLine(cppArduinoCodeGenerator.Code);
                        
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
