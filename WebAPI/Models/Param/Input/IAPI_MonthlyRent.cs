using Domain.WebAPI.Input.HiEasyRentAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_MonthlyRent
    {
        public string CUSTID { get; set; }
        public string CUSTNM { get; set; }
        public string EMAIL { get; set; }
        public string MonProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public string SDATE { get; set; }       //合約起日
        public string EDATE { get; set; }       //合約迄日
        public int IsMoto { get; set; }
        public int RCVAMT { get; set; }
        public string UNIMNO { get; set; }
        public string CARDNO { get; set; }
        public string AUTHCODE { get; set; }    //授權碼
        public string NORDNO { get; set; }      //網刷編號
        public string INVKIND { get; set; }     //發票聯式
        public string CARRIERID { get; set; }
        public string NPOBAN { get; set; }
        public string INVTITLE { get; set; }    //發票抬頭
        public string INVADDR { get; set; }     //發票地址
        public List<NPR130SavePaymentList> tbPaymentDetail { get; set; }
    }
}
