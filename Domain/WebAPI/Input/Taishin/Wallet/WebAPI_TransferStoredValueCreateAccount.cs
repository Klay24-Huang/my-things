using Domain.WebAPI.Input.Taishin.Wallet.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Wallet
{
    /// <summary>
    /// 會員轉贈+開戶
    /// </summary>
    public class WebAPI_TransferStoredValueCreateAccount
    {
        /// <summary>
        /// 店家交易時間YYYYMMDDhhmmss
        /// </summary>
        public string StoreTransDate { get; set; }
        /// <summary>
        /// 商店訂單編號
        /// </summary>
        public string StoreTransId { get; set; }
        /// <summary>
        /// 交易條碼(無條碼時需要帶會員虛擬帳號)
        /// </summary>
        public string BarCode { get; set; } = "";
        /// <summary>
        /// 轉出會員虛擬帳號(有帶條碼可不用帶轉出會員虛擬帳號)
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// 轉贈金額
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 轉贈留言
        /// </summary>
        public string TransMemo { get; set; }
        /// <summary>
        /// 轉入者資料
        /// </summary>
        public List<AccountData> AccountData { get; set; }
        /// <summary>
        /// 轉出會員姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 轉出手機號碼
        /// </summary>
        public string PhoneNo { get; set; }
        /// <summary>
        /// 轉出Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 轉出證件號碼
        /// </summary>
        public string ID { get; set; }
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
        public string SourceFrom { get; set; }
        /// <summary>
        /// 店名
        /// </summary>
        public string StoreName { get; set; }
    }
}
