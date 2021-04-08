using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALELA.AST
{
    class Id : Expr
    {
        object obj;
        public Id(object obj)
        {
            this.obj = obj;
        }
    }
}
