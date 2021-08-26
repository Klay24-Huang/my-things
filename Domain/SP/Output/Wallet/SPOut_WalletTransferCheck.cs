using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Wallet
{
    public class SPOut_WalletTransferCheck
    {
        /// <summary>
        /// 驗證結果(1成功,0失敗)
        /// </summary>
        public int CkResult { get; set; }
        /// <summary>
        /// 會員名稱
        /// </summary>
        public string MemNm { get; set; }
        /// <summary>
        /// 會員手機號碼
        /// </summary>
        public string MemPhone { get; set; }
        /// <summary>
        /// 錢包剩餘金額
        /// </summary>
        public int WalletAmount { get; set; }
        /// <summary>
        /// 當月入賬總金額
        /// </summary>
        public decimal MonTransIn { get; set; }
        /// <summary>
        /// 當月轉出總金額
        /// </summary>
        public decimal MonTransOut { get; set; }
    }
}
