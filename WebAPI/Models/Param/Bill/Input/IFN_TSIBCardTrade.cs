using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Bill.Input
{
    public class IFN_TSIBCardTrade
    {
        public string IDNO { get; set; }//必填

        public string ProdNm { get; set; }//必填
        /// <summary>
        /// EC交易序號
        ///最大接受欄位長度為50碼，但實際長度依據商戶設定為準
        /// </summary>
        public string MerchantTradeNo { get; set; }//必填
    }
}