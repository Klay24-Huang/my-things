using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_HandleCarBind: IAPI_BE_Base
    {
        public List<BE_CarBindImportData> CarBindImportData { set; get; }
    }
}