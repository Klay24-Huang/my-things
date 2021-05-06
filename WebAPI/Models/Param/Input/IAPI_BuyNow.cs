using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_BuyNow
    {
        /// <summary>
        /// 呼叫端apiId
        /// </summary>
        public int ApiID { get; set; }
        /// <summary>
        /// 呼叫端json
        /// </summary>
        public string ApiJson { get; set; }
        /// <summary>
        /// 產品名稱
        /// </summary>
        public string ProdNm { get; set; }
        /// <summary>
        /// 產品描述
        /// </summary>
        public string ProdDisc { get; set; } = "";
        /// <summary>
        /// 產品價格
        /// </summary>
        public int ProdPrice { get; set; }
        /// <summary>
        /// 執行付款
        /// <para>0不執行付款</para>
        /// <para>1執行付款</para>
        /// </summary>
        public int DoPay { get; set; } = 0;
        /// <summary>
        /// 選定付款方式
        /// </summary>
        public int PayTypeId { get; set; } = 0;
        /// <summary>
        /// 選定發票設定
        /// </summary>
        public int InvoTypeId { get; set; } = 0;
    }
}