using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin
{
    public class BindRequestParams
    {
        public string OrderNo { get; set; }
        public string MemberId { get; set; }
        public string ResultUrl { get; set; }
        public string SuccessUrl { get; set; }
        public string FailUrl { get; set; }
        public string PaymentType { get; set; }

        public string BankNo { get; set; }
        public string CoBranded { get; set; }
        public string CoBrandCardEventCode { get; set; }
        public string CoBrandCardStartDate { get; set; }
        public string CoBrandCardEndDate { get; set; }
        public string CardHash { get; set; }
        public string CardToken { get; set; }
        public string CardNumber { get; set; }
        public string ExpDate { get; set; }
        public string CardName { get; set; }
        public string CardStatus { get; set; }
        public string CardType { get; set; }
    }
}
