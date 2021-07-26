using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Taishin
{
    public class SPInput_InsPayTransactionLog : SPInput_Base
    {

        /// <summary>
        ///		由商店端產生，雙方識別的唯一值
        ///</summary>        
        public string GUID { get; set; }
        /// <summary>
        ///		商店代號
        ///</summary>
        public string MerchantId { get; set; }
        /// <summary>
        ///		會員虛擬帳號(此欄位與交易條碼需擇一有值)	      
        ///</summary>
        public string AccountId { get; set; }
        /// <summary>
        ///		交易條碼(此欄位與會員虛擬帳號擇一有值)
        ///</summary>
        public string BarCode { get; set; }
        /// <summary>
        ///		POS編號
        ///</summary>
        public string POSId { get; set; }
        /// <summary>
        ///		店家編號
        ///</summary>
        public string StoreId { get; set; }
        /// <summary>
        ///		店家交易時間(發動交易日期) YYYYMMDDhhmmss	      
        ///</summary>
        public string StoreTransDate { get; set; }
        /// <summary>
        ///		商店訂單編號
        ///</summary>
        public string StoreTransId { get; set; }
        /// <summary>
        ///		商店營收日
        ///</summary>
        public string TransmittalDate { get; set; }
        /// <summary>
        ///		交易時間
        ///</summary>
        public string TransDate { get; set; }
        /// <summary>
        ///		台新訂單編號
        ///</summary>
        public string TransId { get; set; }
        /// <summary>
        ///		原台新訂單編號(退款時此欄位應有值)
        ///</summary>
        public string SourceTransId { get; set; }
        /// <summary>
        ///		交易類別
        ///		T001=交易扣款
        ///     T002=交易退款	      
        ///</summary>
        public string TransType { get; set; }
        /// <summary>
        ///		是否使用紅利點數
        ///     Y=是
        ///     N=否      
        ///</summary>
        public string BonusFlag { get; set; }
        /// <summary>
        ///		價金保管
        ///     Y=是
        ///     N=否  
        /// <summary>
        public string PriceCustody { get; set; }
        /// <summary>
        ///		是否購買菸酒類商品
        ///     Y=是
        ///     N=否     
        ///</summary>
        public string SmokeLiqueurFlag { get; set; }
        /// <summary>
        ///		現金帳戶扣款金額
        ///     (不含紅利)	      
        ///</summary>
        public int Amount { get; set; }
        /// <summary>
        ///		實際金額
        ///     (扣除紅利折抵後的金額)
        ///     (交易類別為交易退款時，請放入0，且不比對此欄位)
        ///</summary>
        public int ActualAmount { get; set; }
        /// <summary>
        ///		折抵紅利點數
        ///     退還紅利
        ///</summary>
        public int Bonus { get; set; }
        /// <summary>
        ///		交易來源
        ///     1=POS
        ///     2=APP
        ///     3=WEB
        ///     4=其他
        ///     5=ATM虛擬帳號
        ///     6=銀行帳號存入
        ///     7=活動贈送
        ///     8=商品預售下架轉存
        ///     9=線上刷卡儲值
        ///     A=銀行紅利點數轉存
        ///     B=實體禮物卡轉存
        ///     E=環保杯轉存
        ///     H=職福會儲值	      
        ///</summary>
        public string SourceFrom { get; set; }
        /// <summary>
        ///		帳務處理狀態
        ///     0=正常
        ///     1=未收到回覆，已完成交易
        ///     2=已人工至後台調帳	      
        ///</summary>
        public string AccountingStatus { get; set; }
        /// <summary>
        ///		菸品金額
        ///     (交易退款不需帶值)	      
        ///</summary>
        public int SmokeAmount { get; set; }
        /// <summary>
        ///		禮物卡帳戶扣款金額
        ///</summary>
        public int ActualGiftCardAmount { get; set; }



    }
}
