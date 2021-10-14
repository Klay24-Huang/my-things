using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Taishin
{
    public class SPInput_InsWalletStoreShopLog : SPInput_Base
    {

        /// <summary>
        /// 用戶端交易代碼
        /// </summary>
        public string CTxSn { get; set; }

        /// <summary>
        /// 交易序號
        /// </summary>
        public string TxSn { get; set; }

        /// <summary>
        /// 交易類別 新增：i、刪除：d
        /// </summary>
        public string TxType { get; set; }

        /// <summary>
        /// 銷帳編號
        /// </summary>
        public string PaymentId { get; set; }

        /// <summary>
        /// 超商類型 0 : 7-11 1: 全家 2: 萊爾富
        /// </summary>
        public Int32 CvsType { get; set; }

        /// <summary>
        /// 超商代收碼 長度:3
        /// </summary>
        public string CvsCode { get; set; }

        /// <summary>
        /// 繳費金額 
        /// </summary>
        public int PayAmount { get; set; }

        /// <summary>
        /// 期數 
        /// </summary>
        public int PayPeriod { get; set; }

        /// <summary>
        /// 繳費期限 YYYYMMDD 
        /// </summary>
        public string DueDate { get; set; }

        /// <summary>
        /// 是否允許溢繳, Y：是、N：否
        /// </summary>
        public string OverPaid { get; set; }

        /// <summary>
        /// 繳費人客戶編號
        /// </summary>
        public string CustId { get; set; }

        /// <summary>
        /// 繳費人行動電話, 最大長度:20
        /// </summary>
        public string CustMobile { get; set; }

        /// <summary>
        ///  繳費人Email, 最大長度:50
        /// </summary>
        public string CustEmail { get; set; }

        /// <summary>
        /// 備註, 最大長度:50
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 回應狀態代碼
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// 狀態說明
        /// </summary>
        public string StatusDesc { get; set; }

    }
}
