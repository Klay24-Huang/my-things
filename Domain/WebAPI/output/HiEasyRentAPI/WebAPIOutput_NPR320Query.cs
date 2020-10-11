using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
    public class WebAPIOutput_NPR320Query
    {
        /// <summary>
        /// 回傳的訊息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 執行結果
        /// <para>true:成功</para>
        /// <para>false:失敗</para>
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        ///回傳代碼
        ///<para>0:成功</para>
        /// </summary>
        public string RtnCode { get; set; }
    }
}
