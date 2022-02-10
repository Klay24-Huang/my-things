using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Wallet.Param
{

    /// <summary>
    /// 繳費明細請求物件
    /// </summary>
    public class CvsPayInfoReq
    {
        /// <summary>
        /// 交易類別 新增：i、刪除：d
        /// </summary>
        public string txType { get; set; }

        /// <summary>
        /// 上傳交易日期 YYYYMMDD
        /// </summary>
        public string txDate { get; set; }

        /// <summary>
        /// 用戶交易批號, 最大長度:20,檢核規則:英數字 須為唯一值
        /// Example : "20191118001"
        /// </summary>
        public string txBatchNo { get; set; }

        /// <summary>
        /// 筆數, 最大值:999999999, 檢核規則:{5000}筆
        /// </summary>
        public Int32 recordCount { get; set; }

        /// <summary>
        /// 繳費總金額
        /// </summary>
        public Int32 totalAmount { get; set; }

        /// <summary>
        /// 超商代收碼
        /// </summary>
        public CvsCode cvsCode { get; set; }

        /// <summary>
        /// 繳費明細
        /// </summary>
        public List<CVSPayInfoDetailReq> detail { get; set; }
    }

    public class CvsCode 
    {
        /// <summary>
        /// 超商代收碼 長度:3
        /// </summary>
        public string cvsCode { get; set; }

        /// <summary>
        /// 超商類型 0 : 7-11 1: 全家 2: 萊爾富
        /// </summary>
        public Int32 cvsType { get; set; }
    }

    public class CVSPayInfoDetailReq
    {
        /// <summary>
        /// 繳費期限 YYYYMMDD
        /// </summary>
        public string paidDue { get; set; }

        /// <summary>
        /// 銷帳編號, 固定長度:16, 檢核規則:1. 檢核英數字，不可為特殊字元或空白 2. 格式：業者識別碼3~10碼+自訂序號，不足16碼則右靠左補0
        /// </summary>
        public string paymentId { get; set; }

        /// <summary>
        /// 繳費金額, 最大值:999999999
        /// </summary>
        public Int32 payAmount { get; set; }

        /// <summary>
        /// 期數
        /// </summary>
        public Int32 payPeriod { get; set; }

        /// <summary>
        /// 是否允許溢繳, Y：是、N：否
        /// </summary>
        public string overPaid { get; set; }

        /// <summary>
        /// 繳費人客戶編號, 繳費人證號、繳費人編號 格式英數字
        /// </summary>
        public string custId { get; set; }

        /// <summary>
        /// 繳費人行動電話, 最大長度:20
        /// </summary>
        public string custMobile { get; set; }

        /// <summary>
        /// 繳費人Email, 最大長度:50
        /// </summary>
        public string custEmail { get; set; }

        /// <summary>
        /// 備註, 最大長度:50
        /// </summary>
        public string memo { get; set; }

    }
}