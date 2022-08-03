using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 重算租金
    /// </summary>
    public class IAPI_RePayDetail
    {
        /// <summary>
        /// 操作者員工編號
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 會員編號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 汽車時數
        /// </summary>
        public int Discount { get; set; }

        /// <summary>
        /// 機車時數
        /// </summary>
        public int MotorDiscount { get; set; }
    }
}