using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Output
{
    public class OAPI_BE_BonusQuery
    {
        /// <summary>
        /// 汽車剩餘點數加總
        /// </summary>
        public int TotalCarLASTPOINT { set; get; }
        /// <summary>
        /// 機車剩餘點數加總
        /// </summary>
        public int TotalMotorLASTPOINT { set; get; }

        /// <summary>
        /// 剩餘點數加總
        /// </summary>
        public int TotalLASTPOINT { set; get; }
    }
}