using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
    /// <summary>
    /// 點數查詢資料
    /// </summary>
    public class WebAPIOutput_NPR270QueryData
    {
        /// <summary>
        /// 贈送編號
        /// </summary>
        public string SEQNO { set; get; }
        /// <summary>
        /// 贈送說明
        /// </summary>
        public string GIFTNAME { set; get; }
        /// <summary>
        /// 贈送時數
        /// </summary>
        public string GIFTPOINT { set; get; }
        /// <summary>
        /// 有效日期起
        /// </summary>
        public string SDATE { set; get; }
        /// <summary>
        /// 有效日期迄
        /// </summary>
        public string EDATE { set; get; }
        /// <summary>
        /// 剩餘點數
        /// </summary>
        public string LASTPOINT { set; get; }
        /// <summary>
        /// 是否為付費點數
        /// <para>Y:是</para>
        /// <para>N或其他為否</para>
        /// </summary>
        public string RCVFLG { set; get; }
        /// <summary>
        /// 點數類別
        /// <para>01:汽車</para>
        /// <para>02:機車</para>
        /// </summary>
        public string GIFTTYPE { set; get; }
    }
}
