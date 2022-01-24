using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Subscription
{
    public class SPInput_InsMonthlyInvErr
    {
        public string ApiInput { get; set; }
        /// <summary>
        /// 身分證字號
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// LOG編號
        /// </summary>
        public Int64 LogID { get; set; }
        /// <summary>
        /// 訂閱編號
        /// </summary>
        public int MonthlyRentID { get; set; }
        /// <summary>
        /// 方案編號
        /// </summary>
        public string MonProjID { get; set; }
        /// <summary>
        /// 方案總期數
        /// </summary>
        public int MonProPeriod { get; set; }
        /// <summary>
        /// 短天期天數
        /// </summary>
        public int ShortDays { get; set; }
        /// <summary>
        /// 目前期數
        /// </summary>
        public int NowPeriod { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public Int64 PayTypeId { get; set; }
        /// <summary>
        /// 發票設定
        /// </summary>
        public Int64 InvoTypeId { get; set; }
        /// <summary>
        /// 發票寄送方式
        /// </summary>
        public string InvoiceType { get; set; }
        /// <summary>
        /// 統編
        /// </summary>
        public string UNIMNO { get; set; }
        /// <summary>
        /// 載具條碼
        /// </summary>
        public string CARRIERID { get; set; }
        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN { get; set; }
        /// <summary>
        /// 發票金額
        /// </summary>
        public int INVAMT { get; set; }

        /// <summary>
        /// 程式名稱
        /// </summary>
        public string PRGID { get; set; } = "";

        /// <summary>
        /// 短租回傳錯誤代碼
        /// </summary>
        public string RtnCode { get; set; } = "";

        /// <summary>
        /// 短租回傳錯誤訊息
        /// </summary>
        public string RtnMsg { get; set; } = "";
    }
}
