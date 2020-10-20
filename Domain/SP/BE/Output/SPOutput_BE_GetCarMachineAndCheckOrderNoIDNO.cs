using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Output
{
    public class SPOutput_BE_GetCarMachineAndCheckOrderNoIDNO : SPOutput_Base
    {
     
        /// <summary>
        /// 是否為興聯車機
        /// </summary>
        public int IsCens { set; get; }
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// 遠傳車機token
        /// </summary>
        public string deviceToken { set; get; }
      
    }
}
