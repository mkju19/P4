using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommandLine;

namespace ALELA_Compiler {

    class ArgsHandler {
        // TODO extende args handler
        public string[] Args;
        public string InFile, OutFile = "a.ino";
        public bool verbose = true;
        public bool arduinoCode = true;
        public bool createFile = true;
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

    class Options : ParserSettings {
        public Options() {
            AutoHelp = true;
        }

        [Value(0)]
        public string InputFile { get; set; }

        [Option('o', Required = false,
          HelpText = "Output file name.")]
        public string OutputFile { get; set; }

        [Option('v', "verbose", Required = false,
          HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('a', "arduino", Required = false,
          HelpText = "Outputs a arduino code file.")]
        public bool arduino { get; set; }


    }

}
