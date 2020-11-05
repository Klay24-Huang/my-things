using Domain.TB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_ManagerStationList
    {
        /// <summary>
        /// 管轄據點資料
        /// </summary>
        public List<iRentManagerStation> ManagerStationObj { set; get; }
    }
}