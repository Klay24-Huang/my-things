using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
    public class WebAPIOutput_NPR130Save
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
        public WebAPIOutput_NPR130SaveData[] Data { set; get; }
    }

    public class WebAPIOutput_NPR130SaveData
    {
        /// <summary>
        /// 回傳的發票號碼
        /// </summary>
        public string INVNO { set; get; }
        /// <summary>
        /// 發票日期
        /// </summary>
        public string INDATE { set; get; }
        /// <summary>
        /// 發票金額
        /// </summary>
        public int INVAMT { set; get; }
    }
}
