using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
    public class WebAPIOutput_NPR060Save
    {
        /// <summary>
        /// 處理結果
        /// <para>True:成功</para>
        /// <para>False:失敗</para>
        /// </summary>
        public bool Result { set; get; }
        public string RtnCode { set; get; }
        /// <summary>
        /// 回覆訊息
        /// </summary>
        public string Message { set; get; }
        public WebAPIOutput_NPR060SaveData[] Data { set; get; }
    
    }
    public class WebAPIOutput_NPR060SaveData
    {
        /// <summary>
        /// 回傳的短租訂單編號
        /// </summary>
        public string ORDNO { set; get; }
    }
}
