using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Wallet
{

    /// <summary>
    /// 錢包儲值-設定資訊
    /// </summary>
    public class SPOut_GetWalletStoredMoneySet
    {
        /// <summary>
        /// 儲值方式(1:信用卡 2:虛擬帳號 3:超商繳費)
        /// </summary>
        public int StoreType { get; set; }

        /// <summary>
        /// 明細方式(全家:family 7-11:seven)
        /// </summary>
        public string StoreTypeDetail { get; set; }

        /// <summary>
        /// 錢包餘額
        /// </summary>
        public int WalletBalance { get; set; }

        /// <summary>
        /// 尚可儲值
        /// </summary>
        public int Rechargeable { get; set; }


        /// <summary>
        /// 單次儲值最低
        /// </summary>
        public int StoreLimit { get; set; }

        /// <summary>
        /// 單次儲值最高
        /// </summary>
        public int StoreMax { get; set; }

        /// <summary>
        /// 快速按鈕
        /// </summary>
        public string QuickBtns { get; set; }

        /// <summary>
        /// 預設選取(1:是 0:否)
        /// </summary>
        public int defSet { get; set; }
    }
}
