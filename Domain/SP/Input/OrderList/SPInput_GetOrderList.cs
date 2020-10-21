using System;

namespace Domain.SP.Input.OrderList
{
    public class SPInput_GetOrderList:SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }
    }
}