﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Subscription
{
    public class SPOut_GetSubsHist
    {
        public string MonProjID { get; set; }
        public int MonProPeriod { get; set; }
        public int ShortDays { get; set; }
        public string MonProjNM { get; set; }
        public int PeriodPrice { get; set; }
        public double CarWDHours { get; set; }
        public double CarHDHours { get; set; }
        public double MotoTotalMins { get; set; }
        public Int64 MonthlyRentId { get; set; }
        public double WDRateForCar { get; set; }
        public double HDRateForCar { get; set; }
        public double WDRateForMoto { get; set; }
        public double HDRateForMoto { get; set; }
        public DateTime PayDate { get; set; }//null值於map過濾
        public int IsMoto { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PerNo { get; set; }        
        public int TradeNo { get; set; }
        public string InvType { get; set; }
        public string unified_business_no { get; set; }
        public string invoiceCode { get; set; }
        public string invoice_date { get; set; }
        public int invoice_price { get; set; }
        /// <summary>
        /// 是否為城市車手 1:是 0:否 20210525 ADD BY ADAM
        /// </summary>
        public int IsMix { get; set; }
        /// <summary>
        /// 載具條碼 20210619 ADD BY ADAM
        /// </summary>
        public string CARRIERID { get; set; }
        /// <summary>
        /// 捐贈碼 20210619 ADD BY ADAM
        /// </summary>
        public string NPOBAN { get; set; }
        /// <summary>
        /// 發票類型
        /// <para>1:愛心碼</para>
        /// <para>2:email</para>
        /// <para>3:二聯</para>
        /// <para>4:三聯</para>
        /// <para>5:手機條碼</para>
        /// <para>6:自然人憑證</para>
        /// </summary>
        public int InvoiceType { get; set; }
        /// <summary>
        /// 捐贈協會名稱
        /// </summary>
        public string NPOBAN_Name { get; set; }
    }
}