using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class NPR330SaveDetail
    {
        public int NPR330Save_ID { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 金額
        /// </summary>
        public int Amount { set; get; }
        /// <summary>
        /// IRENT訂單編號
        /// </summary>
        public string IRENTORDNO { set; get; }
        /// <summary>
        /// 短租預約編號
        /// </summary>
        public string ORDNO { set; get; }
        /// <summary>
        /// 短租合約編號
        /// </summary>
        public string CNTRNO { set; get; }
        /// <summary>
        /// 罰單編號
        /// </summary>
        public string POLNO { set; get; }
        public string PAYMENTTYPE { set; get; }

    }
}