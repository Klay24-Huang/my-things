using Domain.TB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 路邊租還
    /// </summary>
    public class OAPI_AnyRent
    {
        public List<AnyRentObj> AnyRentObj { set; get; }
    }
}