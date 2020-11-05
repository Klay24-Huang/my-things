using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
   public  class WebAPIOutput_NPR172Query
    {
        /// <summary>
        /// 處理結果
        /// <para>True:成功</para>
        /// <para>False:失敗</para>
        /// </summary>
        public bool Result { set; get; }
        /// <summary>
        /// 回覆訊息
        /// </summary>
        public string Message { set; get; }
        public WebAPIOutput_NPR172QueryData[] Data { set; get; }

    }
    public class WebAPIOutput_NPR172QueryData
    {
        /// <summary>
        /// 會員編號
        /// </summary>
        public string MEMIDNO { set; get; }
        /// <summary>
        /// 原因類別
        /// </summary>
        public string REASON { set; get; }
        /// <summary>
        /// 備註
        /// </summary>
        public string MEMO { set; get; }
        /// <summary>
        /// 起日
        /// </summary>
        public string STADT { set; get; }
        /// <summary>
        /// 迄日
        /// </summary>
        public string ENDDT { set; get; }
    }
}
