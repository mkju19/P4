using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 

namespace ALELA.AST
{
    abstract class ProgAST : Tree
    {

    }
    abstract class Tree
    {
        public static int FLTTYPE = 0, INTTYPE = 1;
        public int type;
        public static Dictionary<string, int> SymbolTable = new Dictionary<string, int>();
        public abstract void accept(Visitor v);
    }
}
