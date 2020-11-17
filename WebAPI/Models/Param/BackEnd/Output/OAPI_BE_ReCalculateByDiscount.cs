using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Output
{
    public class OAPI_BE_ReCalculateByDiscount
    {
        /// <summary>
        /// 折抵後新價
        /// </summary>
        public int NewFinalPrice { set; get; }
        /// <summary>
        /// 舊總額-新總額
        /// </summary>
        public int DiffFinalPrice { set; get; }
        public int RentPrice { set; get; }
    }
}