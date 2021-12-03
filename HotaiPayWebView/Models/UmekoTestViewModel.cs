using Domain.Flow.Hotai;
using Domain.TB.Hotai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotaiPayWebView.Models
{
    public class UmekoTestViewModel
    {
        public HotaiCardInfo hotaiCardInfo { get; set; }

        public OFN_HotaiCreditCardList oFN_HotaiCreditCardList { get; set; }

        public string errorCode { get; set; }

        public string CTBCIDNO { get; set; }

        public string Birthday { get; set; }

        public string OrderID { get; set; }

        public int CardToken { get; set; } = 1863;

    }

}