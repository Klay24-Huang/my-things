using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Taishin
{
    public class SPInput_InsStoreValueCreateAccountLog : SPInput_Base
    {
        /// <summary>
        /// 由商店端產生，雙方識別的唯一值
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 商店代號
        /// </summary>
        public string MerchantId { get; set; }
        /// <summary>
        /// 會員虛擬帳號
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// POS編號
        /// </summary>
        public string POSId { get; set; }
        /// <summary>
        /// 店家編號
        /// </summary>
        public string StoreId { get; set; }
        /// <summary>
        /// 店家交易時間(發動交易日期) YYYYMMDDhhmmss
        /// </summary>
        public string StoreTransDate { get; set; }
        /// <summary>
        /// 商店訂單編號
        /// </summary>
        public string StoreTransId { get; set; }
        /// <summary>
        /// 商店營收日
        /// </summary>
        public string TransmittalDate { get; set; }
        /// <summary>
        /// 交易時間YYYYMMDDhhmmss
        /// </summary>
        public string TransDate { get; set; }
        /// <summary>
        /// 台新訂單編號
        /// </summary>
        public string TransId { get; set; }
        /// <summary>
        /// 原台新訂單編號 退款時此欄位應有值
        /// </summary>
        public string SourceTransId { get; set; }
        /// <summary>
        ///交易類別
        ///T003 = 兩階段儲值待確認
        ///T004 = 兩階段儲值已確認
        ///T005 = 取消儲值
        ///T006 = 直接儲值
        ///T007 = 儲值退款
        ///T011 = 批次儲值
        /// </summary>
        public string TransType { get; set; }
        /// <summary>
        /// 金額類別
        /// 1=現金
        /// 2=信用卡
        /// 3=收款金額
        /// 5=台新履保實體禮物卡
        /// 6=非台新履保實體禮物卡
        /// 7=環保杯轉存(不產出)
        /// </summary>
        public string AmountType { get; set; }
        /// <summary>
        /// 儲值金額/儲值退款金額
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 紅利點數
        /// </summary>
        public int Bonus { get; set; }
        /// <summary>
        /// 紅利到期日YYYYMMDD
        /// </summary>
        public string BonusExpiredate { get; set; }
        /// <summary>
        /// 儲值條碼
        /// </summary>
        public string BarCode { get; set; }
        /// <summary>
        /// 履保起日YYYYMMDD
        /// </summary>
        public string StoreValueReleaseDate { get; set; }
        /// <summary>
        /// 履保迄日YYYYMMDD
        /// </summary>
        public string StoreValueExpireDate { get; set; }
        /// <summary>
        /// 交易來源
        ///1=POS
        ///2=APP
        ///3=WEB
        ///4=其他
        ///5=ATM虛擬帳號
        ///6=銀行帳號存入
        ///7=活動贈送
        ///8=商品預售下架轉存
        ///9=線上刷卡儲值
        ///A=銀行紅利點數轉存
        ///B=實體禮物卡轉存
        ///E=環保杯轉存(不產出)
        ///H=職福會儲值
        /// </summary>
        public string SourceFrom { get; set; }
        /// <summary>
        ///		帳務處理狀態
        ///     0=正常
        ///     1=未收到回覆，已完成交易
        ///     2=已人工至後台調帳	      
        ///</summary>
        public string AccountingStatus { get; set; }
        /// <summary>
        /// 禮物卡條碼
        /// </summary>
        public string GiftCardBarCode { get; set; }
    }
}
