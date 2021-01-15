using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    //usp_GetEstimate-return
    public class GetFullProjectVM
    {
        public string PROJID { get; set; }
        public string CarTypeGroupCode { get; set; }
        public double PRICE { get; set; }
        public double PRICE_H { get; set; }
    }
}