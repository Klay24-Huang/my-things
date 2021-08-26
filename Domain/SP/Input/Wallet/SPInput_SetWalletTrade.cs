using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Wallet
{
    public class SPInput_SetWalletTrade
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        /// <summary>
        /// 財務-上游批號(IR編)
        /// </summary>
        public string TaishinNO { get; set; }
        /// <summary>
        /// 更新資料之程式名稱
        /// </summary>
        public string UPDPRGID { get; set; }
        /// <summary>
        /// 交易類別(對應財務FMFLAG)
        /// </summary>
        public string TradeType { get; set; }
        /// <summary>
        /// 交易代號
        /// </summary>
        public string TradeKey { get; set; }
        /// <summary>
        /// 交易金額
        /// </summary>
        public decimal TradeAMT { get; set; }
        public DateTime? SetNow { get; set; }
    }
}
