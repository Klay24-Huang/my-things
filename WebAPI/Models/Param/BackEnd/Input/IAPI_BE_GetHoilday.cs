using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_GetHoilday:IAPI_BE_Base
    {
        public int QueryYear { set; get; }
        public int QuerySeason { set; get; }
    }
}