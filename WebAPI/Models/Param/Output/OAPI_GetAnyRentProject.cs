using Domain.WebAPI.output.rootAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 
    /// </summary>
    public class OAPI_GetAnyRentProject
    {
        public List<ProjectObj> GetAnyRentProjectObj { set; get; }
    }
}