using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Wallet
{
    public class SPOutput_GetWalletReturn 
    {
        /// <summary>
        /// 序號
        /// </summary>
        public long AuthSeq { get; set; }
        /// <summary>
        /// 身分證號
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// 訂單號碼
        /// </summary>
        public long Order_number { get; set; }

        /// <summary>
        /// 退款金額
        /// </summary>
        public int ReturnAmt { get; set; }

        /// <summary>
        /// 原始商店交易編號
        /// </summary>
        public string Ori_transaction_no { get; set; }
        /// <summary>
        /// 交易序號
        /// </summary>
        public string Transaction_no { get; set; }
        /// <summary>
        /// 支付類別(0:和泰; 1:台新; 2:錢包)
        /// </summary>        
        public int CardType { get; set; }
        /// <summary>
        /// POS編號
        /// </summary>
        public string POSId { get; set; }
        /// <summary>
        /// 店家編號
        /// </summary>
        public string StoreId { get; set; }
        /// <summary>
        /// 交易來源
        /// </summary>
        public string SourceFrom { get; set; }
        /// <summary>
        /// 會員虛擬代號
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// 店家交易時間
        /// </summary>
        public string StoreTransDate { get; set; }
        /// <summary>
        /// 退款商店訂單編號
        /// </summary>
        public string StoreTransId { get; set; }

        /// <summary>
        /// 退款台新訂單編號
        /// </summary>
        public string TransId { get; set; }

        /// <summary>
        /// 交易類別(寫TB_WalletTradeMain)
        /// </summary>
        public string TradeType { get; set; }


    }
}
