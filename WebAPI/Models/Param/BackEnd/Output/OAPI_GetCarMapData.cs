using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.TB.BackEnd;

namespace WebAPI.Models.Param.BackEnd.Output
{
    public class OAPI_GetCarMapData
    {
        public List<CarMapData> lstData { get; set; }
    }
}