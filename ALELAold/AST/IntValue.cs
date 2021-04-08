using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALELA.AST
{
    class IntValue : Expr
    {
        int value;
        public IntValue(int value)
        {
            this.value = value;
        }

    }
}
