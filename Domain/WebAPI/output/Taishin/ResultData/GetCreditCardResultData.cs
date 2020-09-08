using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.ResultData
{
    public class GetCreditCardResultData
    {
        /// <summary>
        /// 
        /// </summary>
        public string CardHash { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BankNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CoBranded { get; set; }
        public string CoBrandCardEventCode { get; set; }
        public string CoBrandCardStartDate { get; set; }
        public string CoBrandCardEndDate { get; set; }
        public string CardToken { get; set; }
        public string CardNumber { get; set; }
        public string CardName { get; set; }
        public string ExpDate { get; set; }
        public string CardType { get; set; }
        public string CardStatus { get; set; }
        public string PaymentType { get; set; }
        public string AvailableAmount { get; set; }
    }
}
