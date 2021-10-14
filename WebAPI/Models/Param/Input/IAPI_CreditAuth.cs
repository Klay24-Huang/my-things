using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_CreditAuth
    {
        /// <summary>
        /// 付款模式
        /// <para>0:租金</para>
        /// <para>1:罰金/補繳</para>
        /// </summary>
        public int PayType { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 罰金或補繳代碼
        /// </summary>
        public string CNTRNO { set; get; }
        ///// <summary>
        ///// 欠款查詢主表ID, 欠費補繳用
        ///// </summary>
        //public int? NPR330Save_ID { get; set; }

        /// <summary>
        /// 付款方式(0:信用卡、1:和雲錢包)
        /// </summary>
        public int CheckoutMode { get; set; }
    }
}