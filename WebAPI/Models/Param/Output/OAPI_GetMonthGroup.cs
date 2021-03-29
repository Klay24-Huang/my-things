using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetMonthGroup
    {
        public string MonProDisc { get; set; }
        public List<MonCardParam> MonCards { get; set; }
    }
}