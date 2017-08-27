using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aplication_csharp_ia
{
    public class Cell
    {   
        public int linha { get; set; }

        public int coluna { get; set; }

        public object item { get; set; }

        public override string ToString()
        {
            return item.ToString();
        }
    }
}
