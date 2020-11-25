using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_AutoPayMoney:IAPI_BE_Base
    {
        /// <summary>
        /// 身份證號
        /// </summary>
        public string CUSTID { get; set; }
        
        public List<AutoPayMoneyData> Data { get; set; }
    }
    public partial class AutoPayMoneyData
    {
        /// <summary>
        /// 短租預約編號
        /// </summary>
        public string ORDNO { get; set; }
        /// <summary>
        /// 短租合約編號
        /// </summary>
        public string CNTRNO { get; set; }
        /// <summary>
        /// 付款類別
        /// <para>1:租金</para>
        /// <para>2:罰單</para>
        /// <para>3:停車費</para>
        /// <para>4:ETAG</para>
        /// <para>5:營損</para>
        /// </summary>
        public int PAYMENTTYPE { get; set; }
        /// <summary>
        /// 付款類別名稱
        /// </summary>
        public string SPAYMENTTYPE { get; set; }
        /// <summary>
        /// 需付金額
        /// </summary>
        public int TAMT { get; set; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CARNO { get; set; }
        /// <summary>
        /// 罰單編號
        /// </summary>
        public string POLNO { get; set; }
        /// <summary>
        /// iRent訂單編號
        /// </summary>
        public string IRENTORDNO { get; set; }
    }
}