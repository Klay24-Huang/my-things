using Domain.TB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 同站租還
    /// </summary>
    public class OAPI_NormalRent
    {
        public List<iRentStationData> NormalRentObj { set; get; }
    }
}