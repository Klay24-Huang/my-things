using System;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
    public class WebAPIOutput_TransIRentMemCMK
    {
        /// <summary>
        /// 處理結果
        /// <para>True:成功</para>
        /// <para>False:失敗</para>
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 回傳代碼
        /// </summary>
        public string RtnCode { get; set; }

        /// <summary>
        /// 回覆訊息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 回傳內容
        /// </summary>
        public Object[] Data { get; set; }
    }
}