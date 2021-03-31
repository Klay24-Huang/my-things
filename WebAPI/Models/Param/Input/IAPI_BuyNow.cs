using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_BuyNow
    {
        /// <summary>
        /// 呼叫端apiId
        /// </summary>
        public int ApiID { get; set; }
        /// <summary>
        /// 呼叫端json
        /// </summary>
        public string ApiJson { get; set; }
        /// <summary>
        /// 產品名稱
        /// </summary>
        public string ProdNm { get; set; }
        /// <summary>
        /// 產品價格
        /// </summary>
        public int ProPrice { get; set; }
    }
}