using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_DelSubsHist
    {
        public Int64 MonthlyRentId { get; set; }
        public DateTime? SetNow { get; set; }
    }
}