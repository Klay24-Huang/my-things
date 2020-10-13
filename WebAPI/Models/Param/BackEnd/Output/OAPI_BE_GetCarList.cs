using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Output
{
    public class OAPI_BE_GetCarList
    {
        public List<iRentCarBase> CarList { set; get; }
    }
}