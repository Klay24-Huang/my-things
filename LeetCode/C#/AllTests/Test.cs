using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllTests
{
    public class Test<TArgs, TWant>
    {
        public string Name { get; set; } = "";

        public TArgs Args { get; set; } 

        public TWant Want { get; set; } 
    }
}
