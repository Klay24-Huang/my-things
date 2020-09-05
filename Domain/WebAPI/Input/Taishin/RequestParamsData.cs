using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin
{
    /// <summary>
    /// 上行電文內容
    /// </summary>
    public class RequestParamsData
    {
        /// <summary>
        /// 失敗回傳網址
        /// </summary>
        public string FailUrl { set; get; }
        /// <summary>
        /// 會員id
        /// </summary>
        public string MemberId { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 支付工具類別
        ///<para>03 Account Link</para>
        ///<para>04 信用卡(預設)</para>
        /// </summary>
        public string PaymentType { set; get; }
        /// <summary>
        /// 16碼防偽隨機碼
        /// </summary>
        //public string Random { set; get; }
        /// <summary>
        /// 接收結果網址
        /// </summary>
        public string ResultUrl { set; get; }
        /// <summary>
        /// 成功回傳網址
        /// </summary>
        public string SuccessUrl { set; get; }
        /// <summary>
        /// 電文產生時間
        /// </summary>
        //public string TimeStamp { set; get; }
      
    
    }
}
