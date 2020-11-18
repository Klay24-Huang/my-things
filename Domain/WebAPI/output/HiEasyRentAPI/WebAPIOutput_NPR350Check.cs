using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.HiEasyRentAPI
{
   public class WebAPIOutput_NPR350Check
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
        public WebAPIOutput_NPR350CheckData[] Data { set; get; }
    }
    public class WebAPIOutput_NPR350CheckData
    {
        /// <summary>
        /// iRent OrderNo
        /// </summary>
        public string IRENTORDNO { set; get; }
        /// <summary>
        /// 合約編號
        /// </summary>
        public string CNTRNO { set; get; }
        /// <summary>
        /// 短租狀態
        /// <para>01:未出車</para>
        /// <para>02:已出車</para>
        /// <para>03:已還車</para>
        /// <para>04:已收款</para>
        /// </summary>
        public string STATUS { set; get; }
        /// <summary>
        /// 發票狀態
        /// <para>Y:可作廢</para>
        /// <para>N:不可作廢</para>
        /// </summary>
        public string INVSTATUS { set; get; }
    }
}
