using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Wallet
{
    /// <summary>
    /// 錢包扣款
    /// </summary>
   public class WebAPI_PayTransaction
    {
        /// <summary>
        /// 店家交易時間YYYYMMDDhhmmss
        /// </summary>
        public string StoreTransDate { get; set; }
        /// <summary>
        /// 商店訂單編號（20碼）
        /// </summary>
        public string StoreTransId { get; set; }
        /// <summary>
        /// 會員虛擬帳號 (有帶條碼可不用帶會員虛擬帳號)
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// 交易條碼 (無條碼時需要帶會員虛擬帳號)
        /// </summary>
        public string BarCode { get; set; }
        /// <summary>
        /// 交易金額
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 是否使用紅利點數(如有帶條碼，則以條碼內的紅利折抵欄位為主)
        /// </summary>
        public string BonusFlag { get; set; } = "N";
        /// <summary>
        /// 價金保管
        /// </summary>
        public string Custody { get; set; } = "N";
        /// <summary>
        /// 是否購買菸酒類商品
        /// </summary>
        public string SmokeLiqueurFlag { get; set; } = "N";
        /// <summary>
        /// API版本(目前版本0.1.01)
        /// </summary>
        public string ApiVersion { get; set; }
        /// <summary>
        /// 由商店端產生，雙方識別的唯一值
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 商店代號
        /// </summary>
        public string MerchantId { get; set; }
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
        /// <para>1:POS</para>
        /// <para>2:APP</para>
        /// <para>3:WEB</para>
        /// <para>4:其他</para>
        /// <para>5:ATM虛擬帳號</para>
        /// <para>6:銀行帳號存入</para>
        /// <para>7:活動贈送</para>
        /// <para>8:商品預售下架轉存</para>
        /// <para>9:線上刷卡儲值</para>
        /// <para>A:銀行紅利點數轉存</para>
        /// <para>B:實體禮物卡轉存</para>
        /// <para>C:中獎發票轉存</para>
        /// <para>Z:線下退款</para>
        /// </summary>
        public string SourceFrom { get; set; } = "2";
        /// <summary>
        /// 店名
        /// </summary>
        public string StoreName { get; set; }
    }
}
