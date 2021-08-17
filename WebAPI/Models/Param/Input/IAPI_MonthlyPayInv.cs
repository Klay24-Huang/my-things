using Domain.WebAPI.Input.HiEasyRentAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_MonthlyPayInv 
    {
        /// <summary>
        /// 月租主檔流水號
        /// </summary>
        public int MonthlyRentId { get; set; }

        /// <summary>
        /// 身分證
        /// </summary>
        public string IdNo { get; set; }
    }
}
