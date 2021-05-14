using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetSubsHist
    {
        public List<OAPI_GetSubsHist_Param> Hists { get; set; }
    }

    public class OAPI_GetSubsHist_Param
    {
        public string MonProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public string MonProjNM { get; set; }
        public int PeriodPrice { get; set; }
        public double CarWDHours { get; set; }
        public double CarHDHours { get; set; }
        public double MotoTotalMins { get; set; }
        public double WDRateForCar { get; set; }
        public double HDRateForCar { get; set; }
        public double WDRateForMoto { get; set; }
        public double HDRateForMoto { get; set; }
        public int IsMoto { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int PerNo { get; set; }
        public Int64? MonthlyRentId { get; set; }
        public string InvType { get; set; }
        public string unified_business_no { get; set; }
        public string invoiceCode { get; set; }
        public string invoice_date { get; set; }
        public int? invoice_price { get; set; }
    }
}