using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
    /// <summary>
    /// 折抵時數轉贈確認查詢API輸出
    /// </summary>
    public class WebAPIOutput_NPR370Check
    {
        /// <summary>
        /// 處理結果
        /// <para>True:成功</para>
        /// <para>False:失敗</para>
        /// </summary>
        public bool Result { set; get; }
        /// <summary>
        /// 回傳代碼
        /// </summary>
        public string RtnCode { set; get; }
        /// <summary>
        /// 回覆訊息
        /// </summary>
        public string Message { set; get; }
        public WebAPIOutput_NPR370CheckData[] Data { set; get; }
    }
}
