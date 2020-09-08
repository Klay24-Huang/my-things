using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.GenerateCheckSum
{
    /// <summary>
    /// 台新_取回綁定(信用卡|銀行帳號)列表，產CheckSum使用
    /// </summary>
    public class PartOfGetCreditCardList
    {
        /// <summary>
        /// 版號
        /// </summary>
        public string ApiVer { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string ApposId { set; get; }
        /// <summary>
        /// 隨機碼
        /// </summary>
        public string Random { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public GetCreditCardListRequestParamasData RequestParams { set; get; }
        /// <summary>
        /// 電文產生時間
        /// </summary>
        public string TimeStamp { set; get; }
        /// <summary>
        /// 交易序號，每次交易時，皆需產生新的序號，不可重覆交易
        /// </summary>
        public string TransNo { set; get; }
    }
}
