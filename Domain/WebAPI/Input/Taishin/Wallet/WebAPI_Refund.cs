using Domain.WebAPI.output.Taishin.Escrow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Wallet
{
    public class WebAPI_Refund 
    {
        /// <summary>
        /// API版本(目前版本0.1.01)
        /// </summary>
        public string ApiVersion { get; set; }
        
        /// <summary>
        /// GUID
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
        /// POS機編號
        /// </summary>
        public string POSId { get; set; }

        /// <summary>
        /// 店家編號
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// 店名
        /// </summary>
        public string StoreName { get; set; } = "";

        /// <summary>
        /// 店家交易時間YYYYmmDDhhMMss
        /// </summary>
        public string StoreTransDate { get; set; }

        /// <summary>
        /// 商店訂單編號
        /// </summary>
        public string StoreTransId { get; set; }

        /// <summary>
        /// 交易扣款的交易序號(與OriginStoreTransId擇一 輸入) (PayTransaction.TransId) Ex: M001SEVE20181229191551135001
        /// </summary>
        public string TransId { get; set; }

        /// <summary>
        /// 原交易扣款的商店訂單編號(與TransId擇一輸入) Ex:FAM0001
        ///* 交易退款建議以TransId為主進行退款，如使用
        ///OriginStoreTransId，須確保該編號無重複，如有
        /// 重複的情形，系統將抓取離系統日最近的一筆資料進行交易退款
        ///* 如商店訂單編號會重複的話，建議OriginPOSId 、OriginStoreId、OriginStoreTransDate，放原交易的資訊
        /// </summary>
        public string OriginStoreTransId { get; set; } = "";

        /// <summary>
        ///原交易扣款的POS編號
        /// </summary>
        public string OriginPOSId { get; set; } = "";

        /// <summary>
        ///原交易扣款的店家編號
        /// </summary>
        public string OriginStoreId { get; set; } = "";

        /// <summary>
        ///原交易扣款的店家交易時間YYYYMMDDhhmmss
        /// </summary>
        public string OriginStoreTransDate { get; set; }

        /// <summary>
        ///交易來源 
        /// </summary>
        public string SourceFrom { get; set; } = "1";

        /// <summary>
        /// 退款金額(不含紅利)
        /// </summary>
        public int RefundAmount { get; set; }

        /// <summary>
        /// 退還紅利
        /// </summary>
        public int RefundBonus { get; set; } = 0;

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
        /// BarCode
        /// </summary>
        public string BarCode { get; set; } = "";

    }
}
