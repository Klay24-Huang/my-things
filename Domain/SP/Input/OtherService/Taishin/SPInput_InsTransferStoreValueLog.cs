using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Taishin
{
	public class SPInput_InsTransferStoreValueLog : SPInput_Base
    {
		/// <summary>
		///	由商店端產生，雙方識別的唯一值	 
		/// </summary>
		public string GUID { get; set; }
		/// <summary>
		///	商店代號	 
		/// </summary>
		public string MerchantId { get; set; }
		/// <summary>
		///	會員虛擬帳號	 
		/// </summary>
		public string AccountId { get; set; }
		/// <summary>
		///	交易條碼	 
		/// </summary>
		public string BarCode { get; set; }
		/// <summary>
		///	POS編號	 
		/// </summary>
		public string POSId { get; set; }
		/// <summary>
		///	店家編號	 
		/// </summary>
		public string StoreId { get; set; }
		/// <summary>
		///	店家交易時間(發動交易日期) YYYYMMDDhhmmss	 
		/// </summary>
		public string StoreTransDate { get; set; }
		/// <summary>
		///	商店訂單編號	 
		/// </summary>
		public string StoreTransId { get; set; }
		/// <summary>
		///	商店營收日	 
		/// </summary>
		public string TransmittalDate { get; set; }
		/// <summary>
		///	交易時間YYYYMMDDhhmmss	 
		/// </summary>
		public string TransDate { get; set; }
		/// <summary>
		///	台新訂單編號	 
		/// </summary>
		public string TransId { get; set; }
		/// <summary>
		///	交易金額	 
		/// </summary>
		public int Amount { get; set; }
		/// <summary>
		///	實際金額 (交易金額乘轉贈人數)	 
		/// </summary>
		public int ActualAmount { get; set; }
		/// <summary>
		///	受贈的會員虛擬帳號	 
		/// </summary>
		public string TransAccountId { get; set; }
		/// <summary>
		///	"交易來源
		/// 1=POS
		/// 2=APP
		/// 3=WEB
		/// 4=其他
		/// 5=ATM虛擬帳號
		/// 6=銀行帳號存入
		/// 7=活動贈送
		/// 8=商品預售下架轉存
		/// 9=線上刷卡儲值
		/// A=銀行紅利點數轉存
		/// B=實體禮物卡轉存
		/// E=環保杯轉存
		/// H=職福會儲值"	 
		/// </summary>
		public string SourceFrom { get; set; }
		/// <summary>
		///	"金額類別
		/// 1=現金
		/// 2=信用卡
		/// 3=收款金額
		/// 5=台新履保實體禮物卡
		/// 6=非台新履保實體禮物卡
		/// 7=環保杯轉存"	 
		/// </summary>
		public string AmountType { get; set; }

	}
}
