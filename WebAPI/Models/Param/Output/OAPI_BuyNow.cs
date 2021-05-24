using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_BuyNow
    {
        /// <summary>
        /// 產品名稱
        /// </summary>
        public string ProdNm { get; set; } = "";
        /// <summary>
        /// 產品描述
        /// </summary>
        public string ProdDisc { get; set; } = "";
        /// <summary>
        /// 產品價格
        /// </summary>
        public int ProdPrice { get; set; } = 0;
        /// <summary>
        /// 付款方式
        /// </summary>
        public List<OPAI_TypeListParam> PayTypes { get; set; }
        /// <summary>
        /// 發票設定
        /// </summary>
        public List<OPAI_TypeListParam> InvoTypes { get; set; }
        /// <summary>
        /// 付費結果
        /// </summary>
        public int PayResult { get; set; } = 0;
        /// <summary>
        /// 付款錯誤訊息
        /// </summary>
        //public string PayErrMsg { get; set; } = "";
    }

    public class OAPI_BuyNow_Base
    {
        /// <summary>
        /// 付費結果
        /// </summary>
        public int PayResult { get; set; } = 0;
    }

}