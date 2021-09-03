using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Wallet
{
    public class SPOut_GetWalletStoreTradeTransHistory
    {
        /// <summary>
        /// 組織代號(公司代碼)
        /// </summary>
        public string ORGID { get; set; }
        public string IDNO { get; set; }
        /// <summary>
        /// 帳款流水號(by會員)
        /// </summary>
        public int SEQNO { get; set; }
        /// <summary>
        /// 台新交易編號(IR編)
        /// </summary>
        public string TaishinNO { get; set; }
        /// <summary>
        ///  交易類別(對應財務FMFLAG)
        /// </summary>
        public string TradeType { get; set; }
        /// <summary>
        /// 交易代號
        /// </summary>
        public string TradeKey { get; set; }
        /// <summary>
        /// 交易日期
        /// </summary>
        public DateTime TradeDate { get; set; }
        /// <summary>
        /// 信用卡卡號末4碼
        /// </summary>
        public string CardNo { get; set; }
        public string Code0 { get; set; }
        public string CodeName { get; set; }
        /// <summary>
        /// 若1表負項，0表正項
        /// </summary>
        public int Negative { get; set; }
        /// <summary>
        /// 交易金額
        /// </summary>
        public decimal TradeAMT { get; set; }
        /// <summary>
        /// 交易備註
        /// </summary>
        public string TradeNote { get; set; }
    }
}
