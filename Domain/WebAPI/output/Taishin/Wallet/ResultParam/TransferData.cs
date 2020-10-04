using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Wallet.ResultParam
{
    public class TransferData
    {
        public string TransferAccountId { get; set; }
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string ID { get; set; }
        public string AccountType { get; set; }
        public string Status { get; set; }
        public string GuaranteeNo { get; set; }
        public int Amount { get; set; }
        public int Bonus { get; set; }
        public int EachUpper { get; set; }
        public int DayUpper { get; set; }
        public int MonthUpper { get; set; }
        public string CreateDate { get; set; }
    }
}
