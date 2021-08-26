using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_WalletTransferStoredValue
    {
        /// <summary>
        /// 受轉贈者身份證
        /// </summary>
        public string  TransIDNO{set;get;}
        /// <summary>
        /// 轉贈金額
        /// </summary>
        public int Amount { set; get; }
        /// <summary>
        /// 更新資料之程式名稱
        /// </summary>
        public string UPDPRGID { get; set; }
        /// <summary>
        /// 交易類別
        /// </summary>
        public string TradeType { get; set; }
        /// <summary>
        /// 交易Key值
        /// </summary>
        public string TradeKey { get; set; }
    }
}