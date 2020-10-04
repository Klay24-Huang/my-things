using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Wallet
{
    /// <summary>
    /// 查詢錢包交易明細
    /// </summary>
    public class WebAPI_GetAccountValue
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
        /// 商店訂單編號（此欄位與會員虛擬帳號/台新訂單編號三擇一查詢）
        /// </summary>
        public string StoreTransId { get; set; }
        /// <summary>
        /// 台新訂單編號（此欄位與會員虛擬帳號/台新訂單編號三擇一查詢）
        /// </summary>
        public string TransId { get; set; }
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
        /// 查詢起日–如未傳入查詢起迄日，則固定回傳當年度前200筆資料，格式：yyyyMMdd
        /// </summary>
        public string REQStarDate { get; set; }
        /// <summary>
        /// 查詢迄日–如未傳入查詢起迄日，固定回傳當年度前200筆資料，格式：yyyyMMdd
        /// </summary>
        public string REQEndDate { get; set; }
        /// <summary>
        /// 資料起始筆數 –如未傳入筆數，固定回傳從第1筆資料開始回傳, 初次查詢:固定放入1
        ///接續查詢:如資料總筆數為301筆，前次已取得200筆，則本次傳入201，表示要從201筆開始取資料
        ///Ex: 1
        /// </summary>
        public int REQStartCount { get; set; } = 1;
        /// <summary>
        /// 店家名稱
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 欲取得的交易類別
        ///<para>空值=全部交易類別</para>
        ///<para>T001 = 交易扣款</para>
        ///<para>T002=交易退款</para>
        ///<para>T003 = 兩階段儲值待確認</para>
        ///<para>T004=兩階段儲值已確認</para>
        ///<para>T005 = 取消儲值</para>
        ///<para>T006=直接儲值</para>
        ///<para>T007 = 儲值退款</para>
        ///<para>T008=會員轉贈</para>
        ///<para>T011 = 批次儲值</para>
        /// </summary>
        public List<string> TransType { get; set; }
        /// <summary>
        /// 交易狀態
        ///<para>空值=成功+失敗交易</para>
        ///<para>1=成功交易</para>
        ///<para>2=失敗交易</para>
        /// </summary>
        public string TransStatus { get; set; } = "1";
    }
}
