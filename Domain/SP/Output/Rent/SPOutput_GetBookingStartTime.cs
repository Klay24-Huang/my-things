using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Rent
{
    public class SPOutput_GetBookingStartTime:SPOutput_Base
    {
        /// <summary>
        /// 實際出車時間
        /// </summary>
        public DateTime SD { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public DateTime ED { set; get; }
        public DateTime Pure { set; get; }
        /// <summary>
        /// 是否為舊TB
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int isOldBooking { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        public string CarType { set; get; }
        public string ProjID { set; get; }
    }
}
