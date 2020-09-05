using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
    public class WebAPIOutput_NPR260Send
    {
     
        /// <summary>
        /// 執行結果
        /// </summary>
        public bool Result { set; get; }
        /// <summary>
        /// 回傳碼
        /// </summary>
        public int RtnCode { set; get; }
        /// <summary>
        /// 回傳訊息
        /// </summary>
        public string Message { set; get; }
        /// <summary>
        /// 回傳資料（這個api一律回null)
        /// </summary>
        public object Data { get; set; }
    }
}
