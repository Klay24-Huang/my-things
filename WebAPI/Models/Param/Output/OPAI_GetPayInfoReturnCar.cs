using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OPAI_GetPayInfoReturnCar
    {
        /// <summary>
        /// 付款方式列表
        /// </summary>
        public List<OPAI_GetPayInfoReturnCar_CheckoutMode> CheckoutModes { get; set; }

        /// <summary>
        /// 付款資訊
        /// </summary>
        public OPAI_GetPayInfoReturnCar_PayInfo PayInfo { get; set; }
    }

    public class OPAI_GetPayInfoReturnCar_CheckoutMode
    {
        /// <summary>
        /// 付款方式(1信用卡,2錢包)
        /// </summary>
        public int CheckoutMode { get; set; }
        /// <summary>
        /// 付款方式名稱
        /// </summary>
        public string CheckoutNM { get; set; }
        /// <summary>
        /// 付款方式備註
        /// </summary>
        public string CheckoutNote { get; set; }
        /// <summary>
        /// 是否為預設值1是0否
        /// </summary>
        public int IsDef { get; set; }
    }

    public class OPAI_GetPayInfoReturnCar_PayInfo
    {
        /// <summary>
        /// 錢包金額
        /// </summary>
        public int WalletAmount { get; set; }
        /// <summary>
        /// 自動儲值金額
        /// </summary>
        public int CreditStoreAmount { get; set; }

    }
}