using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_WalletStoreTradeTransHistory
    {
        public List<OAPI_WalletStoreTradeTrans> TradeHis { get; set; }
    }

    public class OAPI_WalletStoreTradeTrans
    {      
        /// <summary>
        /// 交易年分
        /// </summary>
        public int TradeYear { get; set; }
        /// <summary>
        /// 交易日期(EX:1/25)
        /// </summary>
        public string TradeDate { get; set; }

        /// <summary>
        /// 交易時間(EX:12:23)
        /// </summary>
        public string TradeTime { get; set; }

        /// <summary>
        /// 交易類別(文字): 儲值, 付款-租汽車, 轉入...
        /// </summary>
        public string TradeTypeNm { get; set; }

        /// <summary>
        /// 交易類別註記(文字): 信用卡*1234, H1234567, 轉贈人 姓O名... 
        /// </summary>
        public string TradeNote { get; set; }

        /// <summary>
        /// 交易金額
        /// </summary>
        public decimal TradeAMT { get; set; }

        /// <summary>
        /// APP上是否顯示：0:隱藏,1:顯示
        /// </summary>
       public int ShowFLG { get; set; }
    }

}