using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALELA.AST
{
    class StringValue : Expr
    {
        string value;
        public StringValue(string value)
        {
            this.value = value;
        }
    }
}
