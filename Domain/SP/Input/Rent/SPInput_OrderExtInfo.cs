using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_OrderExtInfo : SPInput_Base
    {

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }

        /// <summary>
        /// 會員帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 執行程式名稱
        /// </summary>
        public string PRGID { get; set; }

        /// <summary>
        /// 取預授權方式(0信用卡，1錢包，2LinePay，3街口，4和泰Pay)
        /// </summary>
        public int PreAuthMode { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public int CheckoutMode { get; set; }

        /// <summary>
        /// 差額
        /// </summary>
        public int DiffAmount { get; set; }


    }
}
