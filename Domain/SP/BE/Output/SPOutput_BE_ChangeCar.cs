using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Output
{
    public class SPOutput_BE_ChangeCar:SPOutput_Base
    {
        /// <summary>
        /// 新車號
        /// </summary>
        public string NewCarNo { set; get; }
    }
}
