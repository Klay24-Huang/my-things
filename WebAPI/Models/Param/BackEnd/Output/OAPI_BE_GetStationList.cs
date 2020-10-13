using Domain.TB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Output
{
    public class OAPI_BE_GetStationList
    {

        public List<iRentStationBaseInfo> StationList { set; get; }
    }
}