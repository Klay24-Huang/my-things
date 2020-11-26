using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
    public class WebAPIOutput_NPR013Reg
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
        public WebAPIOutput_NPR013RegData[] Data { set; get; }
    }
    public class WebAPIOutput_NPR013RegData
    {
        /// <summary>
        /// 回傳的短租會員編號
        /// </summary>
        public string MEMRFNBR { set; get; }
    }
}
