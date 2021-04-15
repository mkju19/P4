using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ALELA_Compiler {
    class ArgsHandler {
        public string[] Args;
        public string InFile, OutFile = "a.bin";
        public ArgsHandler(string[] args) {
            Args = args;
            if (Args.Length == 0) {
                PrintHelpPage();
            } else {
                ArgsDecode();
            }
        }

        private void ArgsDecode() {
            if (Args.Length == 1) {
                if (Args[0] == "-h" || Args[0] == "help") {
                    PrintHelpPage();
                } else {
                    try {
                        Stream stream = new FileStream(Args[0], FileMode.Open, FileAccess.Read, FileShare.Read);
                        stream.Close();
                        InFile = Args[0];
                    } catch (IOException) {
                        Console.WriteLine("Cannot open/find file " + Args[0]);
                        PrintHelpPage();
                    }
                }
            } else if (Args.Length > 3) {
                Console.WriteLine("Too many Arguments");
                PrintHelpPage();
            } else {
                try {
                    Stream stream = new FileStream(Args[0], FileMode.Open, FileAccess.Read, FileShare.Read);
                    stream.Close();
                    InFile = Args[0];
                    int i = 1;
                    while (!Args[i].Contains("-o")) i++;
                    OutFile = Args[++i];
                } catch (IOException) {
                    Console.WriteLine("Cannot open/find file " + Args[0]);
                    PrintHelpPage();
                } catch (IndexOutOfRangeException) {
                    PrintHelpPage();
                }
            }
        }

        private void PrintHelpPage() {
            Console.WriteLine("Help:\nInput parameters: (inputFileName) [-o (OutputfileName)]");
            Environment.Exit(1);
        }
    }

}
