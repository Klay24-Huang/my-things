using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Wallet
{
    public class WalletBase
    {
        /// <summary>
        /// API版本
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
        /// 商店會員編號，最長20碼
        /// </summary>
        public string MemberId { get; set; }
        /// <summary>
        /// POS機編號，預設帶""
        /// </summary>
        public string POSId { get; set; } = "";
        /// <summary>
        /// 商店代碼，預設帶""
        /// <para>1:錢包儲值</para>
        /// </summary>
        public string StoreId { get; set; } = "";
        /// <summary>
        /// 會員姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 會員電話
        /// </summary>
        public string PhoneNo { get; set; }
        /// <summary>
        /// 會員email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 會員身份證
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 帳戶類別，預設帶2
        /// <para>1:個人一類</para>
        /// <para>2:個人二類(預設)</para>
        /// <para>3:法人一類</para>
        /// <para>4:法人二類</para>
        /// <para>5:法人三類</para>
        /// </summary>
        public string AccountType { get; set; } = "2";
        /// <summary>
        /// 會員虛擬帳號建立來源
        ///<para>1=使用商店會員編號</para>
        ///<para>2=由系統產生</para>
        /// </summary>
        public string CreateType { get; set; } = "1";
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
        public string SourceFrom { get; set; } = "9";
    }
}
