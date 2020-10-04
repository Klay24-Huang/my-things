using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Wallet.ResultParam
{
    /// <summary>
    /// 轉贈result
    /// </summary>
   public  class TransferStoreValueCreateAccountResult
    {
        public string GUID { get; set; }
        public string MerchantId { get; set; }
        public string AccountId { get; set; }
        public string StoreTransId { get; set; }
        public int ActualAmount { get; set; }
        public int Amount { get; set; }
        public int Bonus { get; set; }
        public List<TransferData> TransferData { get; set; }
        public string TransId { get; set; }
        public string TransDate { get; set; }
    }
}
