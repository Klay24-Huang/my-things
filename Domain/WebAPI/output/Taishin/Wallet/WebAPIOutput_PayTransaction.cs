using Domain.WebAPI.output.Taishin.Wallet.ResultParam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Wallet
{
    /// <summary>
    /// 扣款輸出
    /// </summary>
   public class WebAPIOutput_PayTransaction
    {
        public PayTransactionResult Result { get; set; }
        public string ReturnCode { get; set; }
        public string Message { get; set; }
        public string ExceptionData { get; set; }
    }
}
