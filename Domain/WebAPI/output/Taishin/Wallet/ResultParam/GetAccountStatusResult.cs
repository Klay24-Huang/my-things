using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Wallet.ResultParam
{
   public  class GetAccountStatusResult
    {
        /// <summary>
        /// guid
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 由商店端產生，雙方識別的唯一值
        /// </summary>
        public string MerchantId { get; set; }
        /// <summary>
        /// 商店會員編號
        /// </summary>
        public string MemberId { get; set; }
        /// <summary>
        /// 會員虛擬帳號
        /// </summary>
        public string AccountId { get; set; }

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
        /// 帳戶類別
        /// </summary>
        public string AccountType { get; set; }
        /// <summary>
        /// 狀態
        ///  <para>1=未啟用</para>
        ///  <para>2=啟用</para>
        ///  <para>3=凍結</para>
        ///  <para>4=註記刪除</para>
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 履保/信託序號
        /// </summary>
        public string GuaranteeNo { get; set; }
        /// <summary>
        /// 帳戶總餘額
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 紅利點數
        /// </summary>
        public int Bonus { get; set; }
        /// <summary>
        /// 單筆交易限額(本次所回傳的資訊為系統預設值)
        /// </summary>
        public int EachUpper { get; set; }
        /// <summary>
        /// 每日交易限額(本次所回傳的資訊為系統預設值)
        /// </summary>
        public int DayUpper { get; set; }
        /// <summary>
        /// 每月交易限額(本次所回傳的資訊為系統預設值)
        /// </summary>
        public int MonthUpper { get; set; }
        /// <summary>
        /// 開戶日期YYYYMMDDhhmmss
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// 交易時間YYYYMMDDhhmmss
        /// </summary>
        public string TransDate { get; set; }
    }
}
