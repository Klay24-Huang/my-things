using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_WalletHistory
    {
        public int TotalCount { set; get; }
        public int TotalAmount { set; get; }
        public List<DetailData> Details { set; get; }
    }
}