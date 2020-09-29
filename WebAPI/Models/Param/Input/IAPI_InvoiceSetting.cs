using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_InvoiceSetting
    {
        
        /// <summary>
        /// 訂單編號，還車設定時才需帶入
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 發票方式
        /// <para>1:愛心碼</para>
        /// <para>2:email</para>
        /// <para>3:二聯</para>
        /// <para>4:三聯</para>
        /// <para>5:手機條碼</para>
        /// <para>6:自然人憑證</para>
        /// </summary>
        public int InvoiceType { set; get; }
        /// <summary>
        /// 統編，當InvoiceType=4時，需有值
        /// </summary>
        public string UniCode { set; get; }
        /// <summary>
        /// 愛心碼，當InvoiceType=1時，為必填
        /// </summary>
        public string NOBAN { set; get; }
        /// <summary>
        /// 當InvoiceType=5或6時，為必填
        /// </summary>
        public string CARRIERID { set; get; }

    }
}