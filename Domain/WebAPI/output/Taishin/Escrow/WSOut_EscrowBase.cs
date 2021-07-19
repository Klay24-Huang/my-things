using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Escrow
{
    public class WSOut_EscrowBase: IWSOut_EscrowBase
    {
        /// <summary>
        /// 回傳代碼，見訊息回應碼定義表,當ReturnCode非成功時，不會回傳Result
        /// </summary>
        public string ReturnCode { get; set; }
        /// <summary>
        /// 回傳訊息，見訊息回應碼定義表
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 回傳異常錯誤訊息 (僅在發生系統未預期錯誤時，此欄位方有值)
        /// </summary>
        public string ExceptionData { get; set; }
    }

    public interface IWSOut_EscrowBase
    {
        string ReturnCode { get; set; }
        string Message { get; set; }
        string ExceptionData { get; set; }
    }
}
