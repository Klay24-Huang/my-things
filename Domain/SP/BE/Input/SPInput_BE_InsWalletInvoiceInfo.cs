using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_InsWalletInvoiceInfo
    {
        public string IDNO { get; set; }
        public int Amount { get; set; }
        public int Tax { get; set; }
        public int HandleFee { get; set; }
        public int InvoiceType { get; set; }
        public string CustID { get; set; }
        public string Carrier { get; set; }
        public string NPOBAN { get; set; }
        public string RVBANK { get; set; }
        public string RVACNT { get; set; }
        public string RV_NAME { get; set; }
        public string UserID { get; set; }
        public string PRGName { get; set; }
        public long LogID { get; set; }
        public string DocURL { get; set; }

    }
}
