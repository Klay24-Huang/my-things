using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Booking
{
    public class SPOutput_CarMachineCommon : SPOutput_Base
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
        /// 是否為機車車機
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMotor { set; get; }
        /// <summary>
        /// 遠傳車機才有值
        /// </summary>
        public string deviceToken { set; get; }
        public int car_mgt_status{set;get;}
        public int cancel_status {set;get;}
        public int booking_status{set;get;}
    }
}
