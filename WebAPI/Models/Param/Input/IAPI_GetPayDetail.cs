using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 租金明細輸入
    /// </summary>
    public class IAPI_GetPayDetail
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 短期的月租Id
        /// </summary>
        public string ShortTermIds { get; set; }
        /// <summary>
        /// 汽車使用的點數
        /// </summary>
        public int Discount { set; get; }

        /// <summary>
        /// 機車使用的點數
        /// </summary>
        public int MotorDiscount { set; get; }
    }
}