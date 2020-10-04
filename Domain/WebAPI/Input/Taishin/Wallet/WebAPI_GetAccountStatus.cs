using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Wallet
{
    /// <summary>
    /// 查詢帳號狀態
    /// </summary>
    public class WebAPI_GetAccountStatus
    {
        /// <summary>
        /// API 版號
        /// </summary>
        public string ApiVersion { get; set; }
        /// <summary>
        /// GUID
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 商店代碼
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// 會員虛擬帳號（此欄位與會員虛擬帳號/台新訂單編號三擇一查詢）
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// pos編號
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
        /// 店家名稱
        /// </summary>
        public string StoreName { get; set; }

    }
}
