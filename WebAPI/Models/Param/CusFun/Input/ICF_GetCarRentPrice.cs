using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.CusFun.Input
{
    public class ICF_GetCarRentPrice: ICF_CarRentInCompute
    {
        /// <summary>
        /// 月租Id
        /// </summary>
        /// <mark>對應TB_MonthlyRent-MonthlyRentId</mark>
        public Int64 MonId { get; set; }
        public string IDNO { get; set; }
    }
}