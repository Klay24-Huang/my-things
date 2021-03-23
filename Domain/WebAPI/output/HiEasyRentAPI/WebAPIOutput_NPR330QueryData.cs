using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
    /// <summary>
    /// 欠費資料
    /// </summary>
    public class WebAPIOutput_NPR330QueryData
    {
        ///// <summary>
        ///// iRent訂單編號
        ///// </summary>
        //public string IRENTORDNO { set; get; }
        ///// <summary>
        ///// 合約編號
        ///// </summary>
        //public string CNTRNO { set; get; }
        ///// <summary>
        ///// 1:租金,2:罰單,3:停車費,4:ETAG
        ///// </summary>
        //public string PAYMENTTYPE { set; get; }
        ///// <summary>
        ///// 付款類別中文
        ///// </summary>
        //public string SPAYMENTTYPE { set; get; }
        ///// <summary>
        ///// 金額
        ///// </summary>
        //public string TAMT { set; get; }
        ///// <summary>
        ///// 車號
        ///// </summary>
        //public string CARNO { set; get; }
        ///// <summary>
        ///// 罰單號碼
        ///// </summary>
        //public string POLNO { set; get; }
        ///// <summary>
        ///// 出車時間
        ///// </summary>
        //public string GIVEDATE { set; get; }
        ///// <summary>
        ///// 還車時間
        ///// </summary>
        //public string RNTDATE { set; get; }
        ///// <summary>
        ///// 出車據點
        ///// </summary>
        //public string INBRNHCD { set; get; }
        /// <summary>
        /// 會員id
        /// </summary>
        public string CUSTID { get; set; }
        /// <summary>
        /// 短租訂單編號
        /// </summary>
        public string ORDNO { get; set; }
        /// <summary>
        /// 短租合約號
        /// </summary>
        public string CNTRNO { get; set; }
        /// <summary>
        /// 類別
        /// <para>1:租金</para>
        /// <para>2:罰單</para>
        /// <para>3:停車費</para>
        /// <para>4:ETAG</para>
        /// <para>5:營損/代收停車費</para>
        /// </summary>
        public long PAYMENTTYPE { get; set; }
        /// <summary>
        /// 類別說明
        /// </summary>
        public string SPAYMENTTYPE { get; set; }
        /// <summary>
        /// 應付金額
        /// </summary>
        public long DUEAMT { get; set; }
        /// <summary>
        /// 已付金額
        /// </summary>
        public long PAIDAMT { get; set; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CARNO { get; set; }
        /// <summary>
        /// 罰單編號
        /// </summary>
        public string POLNO { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PAYTYPE { get; set; }
        /// <summary>
        /// 取車時間
        /// </summary>
        public string GIVEDATE { get; set; }
        /// <summary>
        /// 還車時間
        /// </summary>
        public string RNTDATE { get; set; }
        /// <summary>
        /// 還車據點
        /// </summary>
        public string INBRNHCD { get; set; }
        /// <summary>
        /// iRent訂單編號
        /// </summary>
        public string IRENTORDNO { get; set; }
        /// <summary>
        /// 總金額
        /// </summary>
        public long TAMT { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string MEMO { get; set; }
    }
}
