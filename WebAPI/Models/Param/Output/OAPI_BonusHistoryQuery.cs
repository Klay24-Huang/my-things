using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.TB;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_BonusHistoryQuery
    {
        public List<BonusHistoryData> BonusObj { set; get; }
        public int TotalGIFTPOINT { set; get; }
        public int TotalLASTPOINT { set; get; }
    }
}