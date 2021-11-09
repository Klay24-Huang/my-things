using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_GetOrderStatusByOrderNo : SPInput_Base
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
        /// JWT TOKEN
        /// </summary>
        public string Token { set; get; }
    }


    public class SPInput_GetOrderAuthList : SPInput_Base
    {
        /// <summary>
        /// 逾時重送時間
        /// </summary>
        public int MINUTES { set; get; }
    }

    public class SPInput_GetOrderAuthListV2 : SPInput_Base
    {
        /// <summary>
        /// 發送通道
        /// </summary>
        public int GateNo { set; get; }
        /// <summary>
        /// 重發訂單
        /// </summary>
        public int Retry { set; get; }
    }
}
