using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 預約
    /// </summary>
    public class IAPI_Booking
    {
        public string ProjID { set; get; }
        public DateTime? SDate { set; get; }
    }
}