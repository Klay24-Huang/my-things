using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_BE_InsWalletInvoiceInfo
    {
        public string UserID { get; set; }
        public string IDNO { get; set; }
        public string cashAmount { get; set; }
        public string invoiceMode { get; set; }
        public int handleFee { get; set; }
        public int tax { get; set; }
        public string carrier { get; set; }
        public string CustID { get; set; }
        public string NPOBAN { get; set; }
        public string RVBANk { get; set; }
        public string RVACNT { get; set; }
        public string RV_NAME { get; set; }
    }
}