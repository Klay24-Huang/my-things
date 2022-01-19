using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Wallet.ResultParam
{
    public class GetAccountValueResultDetail
    {
        /// <summary>
        /// 由商店端產生，雙方識別的唯一值
        /// </summary>
        /// <mark>Ex: 187eb02030bc40a999562d32d928cd91</mark>
        public string GUID { get; set; }
        /// <summary>
        /// 商店代號
        /// </summary>
        /// <mark>StoreABC</mark>
        public string MerchantId { get; set; }
        /// <summary>
        /// 會員虛擬帳號
        /// </summary>
        /// <mark>Stor1811000000000009</mark>
        public string AccountId { get; set; }
        /// <summary>
        /// POS編號
        /// </summary>
        /// <mark>123456789</mark>
        public string POSId { get; set; }
        /// <summary>
        /// 店家編號
        /// </summary>
        /// <mark>12345</mark>
        public string StoreId { get; set; }
        /// <summary>
        /// 店家交易時間YYYYMMDDhhmmss
        /// </summary>
        /// <mark>20181109121534</mark>
        public string StoreTransDate { get; set; }
        /// <summary>
        /// 商店訂單編號
        /// </summary>
        /// <mark>FAM0001</mark>
        public string StoreTransId { get; set; }
        /// <summary>
        /// 台新訂單編號
        /// </summary>
        /// <mark>M001SEVE20181229191551135001</mark>
        public string TransId { get; set; }
        /// <summary>
        /// 交易類別
        /// <para>T001=交易扣款</para>
        /// <para>T002=交易退款</para>
        /// <para>T003=兩階段儲值待確認</para>
        /// <para>T004=兩階段儲值已確認</para>
        /// <para>T005=取消儲值</para>
        /// <para>T006=直接儲值</para>
        /// <para>T007=儲值退款</para>
        /// <para>T008=會員轉贈</para>
        /// <para>T011=批次儲值</para>
        /// </summary>
        public string TransType { get; set; }
        /// <summary>
        /// 交易金額
        /// </summary>
        /// <mark>1000</mark>
        public int Amount { get; set; }
        /// <summary>
        /// 收款交易金額
        /// </summary>
        /// <mark>2000</mark>
        public int IncomeAmount { get; set; }
        /// <summary>
        /// 現金交易金額
        /// </summary>
        /// <mark>100</mark>
        public int CashAmount { get; set; }
        /// <summary>
        /// 信用卡交易金額
        /// </summary>
        /// <mark>7000</mark>
        public int CreditCardAmount { get; set; }
        /// <summary>
        /// 紅利點數交易金額
        /// </summary>
        /// <mark>100</mark>
        public int BonusAmount { get; set; }
        /// <summary>
        /// 交易時間YYYYMMDDhhmmss
        /// </summary>
        /// <mark>20181109121534</mark>
        public string TransDate { get; set; }
        /// <summary>
        /// 交易來源
        /// </summary>
        /// <mark>1</mark>
        public string SourceFrom { get; set; }
        /// <summary>
        /// 履保起日(YYYYMMDD)(交易類別為儲值相關時，此欄位方有值)
        /// </summary>
        /// <mark>20190101</mark>
        public string StoreValueReleaseDate { get; set; }
        /// <summary>
        /// 履保迄日(YYYYMMDD)(交易類別為儲值相關時，此欄位方有值)
        /// </summary>
        /// <mark>20191231</mark>
        public string StoreValueExpiredate { get; set; }
        /// <summary>
        /// 轉贈留言(交易類別為會員轉贈相關時，此欄位方有值)
        /// </summary>
        /// <mark>參加轉贈活動</mark>
        public string TransMemo { get; set; }
        /// <summary>
        /// 禮物卡條碼
        /// </summary>
        /// <mark>BB18122776601285591</mark>
        public string GiftCardBarCode { get; set; }

        /// <summary>
        /// 手機號碼
        /// </summary>
        public string PhoneNo { get; set; }

        //手機號碼 (交易類別="會員轉贈"時方有資料)
        //*當AccountId為贈送者時，此欄位放的是受贈者的
        //手機號碼 (多個手機號碼中間"|"做區隔)
        //Ex:0912345678|0955999123|0922334455...
        //*當AccountId為受贈者時，此欄位放的是贈送者的
        //手機號碼
        //Ex:0955345678        

        /// <summary>
        /// 店名
        /// </summary>
        /// <mark>內湖一號店</mark>
        public string StoreName { get; set; }

        /// <summary>
        /// 回傳代碼，見訊息回應碼定義表
        /// </summary>
        public string ReturnCode { get; set; }

        /// <summary>
        /// 回傳訊息，見訊息回應碼定義表
        /// </summary>
        public string Message { get; set; }
    }
}
