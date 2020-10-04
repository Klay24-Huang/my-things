using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Wallet
{
    /// <summary>
    /// 直接儲值
    /// </summary>
   public class WebAPI_StoredMoney:WalletBase
    {
        public string StoreTransDate { get; set; }
        public string StoreTransId { get; set; }
        public string AccountId { get; set; }
        public string AmountType { get; set; }
        public int Amount { get; set; }
        public int Bonus { get; set; }
        public string BonusExpiredate { get; set; }
        public string BarCode { get; set; }
        public string StoreValueReleaseDate { get; set; }
        public string StoreValueExpireDate { get; set; }
        public string GiftCardBarCode { get; set; }
        public string StoreName { get; set; }
    }
}
