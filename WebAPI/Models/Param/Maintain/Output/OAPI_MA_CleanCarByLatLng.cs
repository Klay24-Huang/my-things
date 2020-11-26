using Domain.TB.Maintain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Maintain.Output
{
    public class OAPI_MA_CleanCarByLatLng
    {
        public int projType { set; get; }
        public int total { set; get; }
        public List<CarCleanDataNew> CarList { set; get; }
    }
}