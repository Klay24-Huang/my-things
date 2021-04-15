using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetMonthList
    {
        public int ReMode { get; set; } = 0;
    }

    public class OAPI_AllMonthList: OAPI_GetMonthList
    {
        public List<MonCardParam> NorMonCards { get; set; }

        public List<MonCardParam> MixMonCards { get; set; }
    }
    
    public class OAPI_MyMonthList: OAPI_GetMonthList
    {
        public List<MonCardParam> MyCards { get; set; }
        public List<MonCardParam> OtherCards { get; set; }
    } 
}