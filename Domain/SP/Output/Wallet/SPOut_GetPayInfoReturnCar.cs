using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Wallet
{
    public class SPOut_GetPayInfoReturnCar
    {
        /// <summary>
        /// 付款方式(1信用卡,2錢包)
        /// </summary>
        public int CheckoutMode { get; set; }
        /// <summary>
        /// 付款方式代號
        /// </summary>
        public string CheckoutCode { get; set; }
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
}
