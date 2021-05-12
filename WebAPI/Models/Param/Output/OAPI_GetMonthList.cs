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

    public class OAPI_AllMonthList_Car: OAPI_GetMonthList
    {
        public int IsMotor { get; set; }
        public List<MonCardParam> NorMonCards { get; set; }

        public List<MonCardParam> MixMonCards { get; set; }
    }

    public class OAPI_AllMonthList_Moto : OAPI_GetMonthList
    {
        public int IsMotor { get; set; }
        public List<MonCardParam> NorMonCards { get; set; }
    }

    public class OAPI_MyMonthList: OAPI_GetMonthList
    {
        public MonCardParam_My MyCar { get; set; }
        public MonCardParam_My MyMoto { get; set; }
    } 
}