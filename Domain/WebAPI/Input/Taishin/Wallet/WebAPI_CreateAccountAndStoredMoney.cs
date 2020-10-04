using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Wallet
{
    public class WebAPI_CreateAccountAndStoredMoney:WalletBase
    {
   
        /// <summary>
        /// 店家交易時間YYYYmmDDhhMMss
        /// </summary>
        public string StoreTransDate { get; set; }
        /// <summary>
        /// 交易序號
        /// </summary>
        public string StoreTransId { get; set; }
        /// <summary>
        /// 金額類別
        /// <para>1:現金</para>
        /// <para>2:信用卡</para>
        /// <para>3:收款金額</para>
        /// </summary>
        public string AmountType { get; set; }
        /// <summary>
        /// 交易金額
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 紅利點數，預設代0
        /// </summary>
        public int Bonus { get; set; } = 0;
        /// <summary>
        /// 紅利到期日，預設帶空值
        /// </summary>
        public string BonusExpiredate { get; set; } = "";
        /// <summary>
        /// 覆保起日，交易來源為實體禮物卡轉存時，此欄位才需帶值
        /// </summary>
        public string StoreValueReleaseDate { get; set; } = "";
        /// <summary>
        /// 覆保迄日，交易來源為實體禮物卡轉存時，此欄位才需帶值
        /// </summary>
        public string StoreValueExpireDate { get; set; } = "";
        /// <summary>
        /// 禮物卡條碼，預設帶空值
        /// </summary>
        public string GiftCardBarCode { get; set; } = "";
        /// <summary>
        /// 商店名稱
        /// </summary>
        public string StoreName { get; set; } = "";
    }
}
