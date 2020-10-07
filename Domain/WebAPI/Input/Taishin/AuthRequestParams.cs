using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin
{
    public class AuthRequestParams
    {
      /// <summary>
        /// 替代性信用卡卡號或替代性銀行帳號
        /// </summary>
        public string CardToken { get; set; }
        /// <summary>
        /// 信用卡分期付款
        ///<para>0:不分期</para>
        ///<para>大於0代表分幾期</para>
        /// </summary>
        public string InstallPeriod { get; set; } = "0";
        /// <summary>
        /// 電子發票註記，預設N
        /// </summary>
        public string InvoiceMark { get; set; } = "N";
    /// <summary>
        /// 商品資料及金額
        /// </summary>
        public List<AuthItem> Item { get; set; }
        /// <summary>
        /// EC交易日期 8碼
        /// </summary>
        public string MerchantTradeDate { get; set; }
        /// <summary>
        /// EC交易序號
        ///最大接受欄位長度為50碼，但實際長度依據商戶設定為準
        /// </summary>
        public string MerchantTradeNo { get; set; }

        /// <summary>
        /// EC交易時間6碼
        /// </summary>
        public string MerchantTradeTime { get; set; }
       /// <summary>
        /// 不可折抵金額包含兩位小數，如100代表1.00元
        /// </summary>
        public string NonRedeemAmt { get; set; }
        /// <summary>
        /// 不可折抵說明代碼
        /// </summary>
        public string NonRedeemdescCode { get; set; }
    
        /// <summary>
        /// 備註一
        /// </summary>
        public string Remark1 { get; set; }
        /// <summary>
        /// 備註二
        /// </summary>
        public string Remark2 { get; set; }
        /// <summary>
        /// 備註三
        /// </summary>
        public string Remark3 { get; set; }
        /// <summary>
        /// 非同步通知Api位址
        /// </summary>
        public string ResultUrl { get; set; }
        /// <summary>
        /// 交易金額，包含兩位小數，如100代表1.00
        /// </summary>
        public string TradeAmount { get; set; }

        /// <summary>
        /// 交易類別
        /// <para>1:授權</para>
        /// </summary>
        public string TradeType { get; set; }

        /// <summary>
        /// 使用紅利折抵，預設N
        /// </summary>
        public string UseRedeem { get; set; } = "N";
 
    
    
    }
}
