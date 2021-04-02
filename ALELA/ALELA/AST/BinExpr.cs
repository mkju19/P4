using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALELA.AST
{
    class BinExpr : Expr
    {
        Operator op;
        Expr left;
        Expr right;

        public BinExpr(Expr e1, Operator op, Expr e2)
        {
            this.op = op;
            left = e1;
            right = e2;
        }
    }
}
