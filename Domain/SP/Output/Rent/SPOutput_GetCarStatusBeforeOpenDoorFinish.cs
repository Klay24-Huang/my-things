using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Rent
{
   public  class SPOutput_GetCarStatusBeforeOpenDoorFinish : SPOutput_Base
    {
        public string CID { set; get; }
        public string StationID { set; get; }
        /// <summary>
        /// 是否為興聯車機
        /// </summary>
        public int IsCens { set; get; }
        /// <summary>
        /// 是否為機車
        /// </summary>
        public int IsMotor { set; get; }
        /// <summary>
        /// 遠傳車機token
        /// </summary>
        public string deviceToken { set; get; }
    }
}
