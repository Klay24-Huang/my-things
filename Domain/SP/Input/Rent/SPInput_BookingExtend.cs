using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_BookingExtend:SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// auth token
        /// </summary>
        public string Token { set; get; }

        /// <summary>
        /// 延長起始時間
        /// </summary>
        public DateTime SD { set; get; }
        /// <summary>
        /// 延長結束時間
        /// </summary>
        public DateTime ED { set; get; }
        public string CarNo { set; get; }
    }
}
