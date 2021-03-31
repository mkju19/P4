using System;
using cocor_compiler;

namespace ALELA
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = "./prog.txt";
            Scanner scanner = new Scanner(file);
            Parser parser = new Parser(scanner);
            parser.Parse();
            Console.WriteLine($" {parser.errors.count} errors detected");
        }
    }
}
