using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aplication_csharp_ia
{
    public class Cell
    {
        public object item { get; set; }

        public override string ToString()
        {
            return item.ToString();
        }
    }
}
