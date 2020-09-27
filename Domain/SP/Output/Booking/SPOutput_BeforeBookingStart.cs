using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Booking
{
    /// <summary>
    /// 取車前判斷
    /// </summary>
   public  class SPOutput_BeforeBookingStart : SPOutput_Base
    {
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// 是否是興聯車機
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsCens { set; get; }
        /// <summary>
        /// 遠傳車機才有值
        /// </summary>
        public string deviceToken { set; get; }
        /// <summary>
        /// 是否為機車
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMotor { set; get; }
    }
}
