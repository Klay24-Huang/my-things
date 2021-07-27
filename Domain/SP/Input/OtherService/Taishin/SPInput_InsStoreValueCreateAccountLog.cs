using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Taishin
{
    public class SPInput_InsStoreValueCreateAccountLog : SPInput_Base
    {
        public string GUID { get; set; }
        public string MerchantId { get; set; }
        public string AccountId { get; set; }
        public string POSId { get; set; }
        public string StoreId { get; set; }
        public string StoreTransDate { get; set; }
        public string StoreTransId { get; set; }
        public string TransmittalDate { get; set; }
        public string TransDate { get; set; }
        public string TransId { get; set; }
        public string SourceTransId { get; set; }
        public string TransType { get; set; }
        public string AmountType { get; set; }
        public int Amount { get; set; }
        public int Bonus { get; set; }
        public string BonusExpiredate { get; set; }
        public string BarCode { get; set; }
        public string StoreValueReleaseDate { get; set; }
        public string StoreValueExpireDate { get; set; }
        public string SourceFrom { get; set; }
        public string AccountingStatus { get; set; }
        public string GiftCardBarCode { get; set; }
    }
}
