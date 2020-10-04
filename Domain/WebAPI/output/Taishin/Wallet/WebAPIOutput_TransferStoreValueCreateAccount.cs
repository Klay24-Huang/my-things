using Domain.WebAPI.output.Taishin.Wallet.ResultParam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Wallet
{
    public class WebAPIOutput_TransferStoreValueCreateAccount
    {
        public TransferStoreValueCreateAccountResult Result { get; set; }
        public string ReturnCode { get; set; }
        public string Message { get; set; }
        public object ExceptionData
        {
            get; set;
        }
    }
}
