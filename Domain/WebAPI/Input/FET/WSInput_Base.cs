using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.FET
{
    public class WSInput_Base<T>
        where T:class
    {
        public bool command { get; set; }
        public string requestId { get; set; }
        public string method { get; set; }
        
        public T _params { get; set; }
    }
 
}
