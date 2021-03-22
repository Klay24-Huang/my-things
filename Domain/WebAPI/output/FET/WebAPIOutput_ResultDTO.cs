using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.FET
{
    public class WebAPIOutput_ResultDTO<T>
    {
        public bool Result { set; get; }
        public string Message { set; get; }
        public T Data { set; get; }
    }
}
