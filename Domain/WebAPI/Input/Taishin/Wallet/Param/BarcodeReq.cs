using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Wallet.Param
{

    /// <summary>
    /// 條碼請求物件
    /// </summary>
    public class BarcodeReq
    {
        /// <summary>
        /// 繳費期限 YYYYMMDD
        /// Example : "20200101"
        /// </summary>
        public string dueDate { get; set; }

        /// <summary>
        /// 超商類型 0:7-11,1:全家 ,2:萊爾富
        /// Example : 0
        /// </summary>
        public Int32 cvsType { get; set; }
        //public string cvsType { get; set; }

        /// <summary>
        /// 超商代收碼
        /// Example : "K9C"
        /// </summary>
        public string cvsCode { get; set; }

        /// <summary>
        /// 銷帳編號, 固定長度:16, 檢核規則:1. 檢核英數字，不可為特殊字元或空白 2. 格式：業者識別碼3~10碼+自訂序號，不足16碼則右靠左補0
        /// Example : "9628311000000001"
        /// </summary>
        public string paymentId { get; set; }

        /// <summary>
        /// 繳費金額, 最大值:999999999 
        /// </summary>
        public Int32 payAmount { get; set; }

        /// <summary>
        /// 期數, 最大值:99 
        /// </summary>
        public Int32 payPeriod { get; set; }
        //public string payPeriod { get; set; }

        /// <summary>
        /// 備註, 最大長度:50
        /// Example : "捐款項目-獎助學金"
        /// </summary>
        public string memo { get; set; }
    }
}
