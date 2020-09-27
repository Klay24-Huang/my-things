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
        /// <summary>
        /// iRent訂單編號
        /// </summary>
        public string IRENTORDNO { set; get; }
        /// <summary>
        /// 合約編號
        /// </summary>
        public string CNTRNO { set; get; }
        /// <summary>
        /// 1:租金,2:罰單,3:停車費,4:ETAG
        /// </summary>
        public string PAYMENTTYPE { set; get; }
        /// <summary>
        /// 付款類別中文
        /// </summary>
        public string SPAYMENTTYPE { set; get; }
        /// <summary>
        /// 金額
        /// </summary>
        public string TAMT { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CARNO { set; get; }
        /// <summary>
        /// 罰單號碼
        /// </summary>
        public string POLNO { set; get; }
        /// <summary>
        /// 出車時間
        /// </summary>
        public string GIVEDATE { set; get; }
        /// <summary>
        /// 還車時間
        /// </summary>
        public string RNTDATE { set; get; }
        /// <summary>
        /// 出車據點
        /// </summary>
        public string INBRNHCD { set; get; }
    }
}
