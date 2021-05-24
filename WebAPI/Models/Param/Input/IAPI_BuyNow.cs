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
        public Int64 PayTypeId { get; set; } = 0;
        /// <summary>
        /// 選定發票設定
        /// </summary>
        public Int64 InvoTypeId { get; set; } = 0;
    }

    public class IAPI_BuyNow_Base
    {
        /// <summary>
        /// 選定付款方式
        /// </summary>
        public Int64 PayTypeId { get; set; } = 0;
        /// <summary>
        /// 選定發票設定
        /// </summary>
        public Int64 InvoTypeId { get; set; } = 0;
    }

    public class IAPI_BuyNow_AddMonth: IAPI_BuyNow_Base
    {
        /// <summary>
        /// 月租專案代號
        /// </summary>
        public string MonProjID { get; set; } = "";
        /// <summary>
        /// 月租總期數
        /// </summary>
        public int MonProPeriod { get; set; } = 0;
        /// <summary>
        /// 短天期
        /// </summary>
        public int ShortDays { get; set; } = 0;
        /// <summary>
        /// 設定自動續約-非必填
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int SetSubsNxt { get; set; } = 0;
    }

}