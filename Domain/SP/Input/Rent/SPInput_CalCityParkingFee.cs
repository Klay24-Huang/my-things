using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_CalCityParkingFee : SPInput_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// 起日
        /// </summary>
        public DateTime SD { set; get; }
        /// <summary>
        /// 迄日
        /// </summary>
        public DateTime ED { set; get; }
        /// <summary>
        /// 身分證字號
        /// </summary>
        public string IDNO { set; get; }
        //public 
    }
}
