using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
    public class WebAPIOutput_NPR138Save
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
        public List<WebAPIOutput_NPR138SaveData> Data { set; get; }
    }

    public class WebAPIOutput_NPR138SaveData
    {
        public string INVNO { get; set; }
        public string RCVNO { get; set; }

    }
}
