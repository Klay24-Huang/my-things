using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Wallet.ResultParam
{
    public class GetAccountValueResultDetail
    {
        public string GUID { get; set; }
        public string MerchantId { get; set; }
        public string AccountId { get; set; }
        public string POSId { get; set; }
        public string StoreId { get; set; }
        public string StoreTransDate { get; set; }
        public string StoreTransId { get; set; }
        public string TransId { get; set; }
        public string TransType { get; set; }
        public int Amount { get; set; }
        public int IncomeAmount { get; set; }
        public int CashAmount { get; set; }
        public int CreditCardAmount { get; set; }
        public int BonusAmount { get; set; }
        public string TransDate { get; set; }
        public string SourceFrom { get; set; }
        public string StoreValueReleaseDate { get; set; }
        public string StoreValueExpiredate { get; set; }
        public string TransMemo { get; set; }
        public string GiftCardBarCode { get; set; }
        public string PhoneNo { get; set; }
        public string StoreName { get; set; }
        public string ReturnCode { get; set; }
        public string Message { get; set; }
    }
}
