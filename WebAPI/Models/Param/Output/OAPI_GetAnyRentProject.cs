using Domain.WebAPI.output.rootAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 
    /// </summary>
    public class OAPI_GetAnyRentProject
    {
        /// <summary>
        /// 目前可使用訂閱制月租
        /// </summary>
        public List<OAPI_NowSubsCard> NowSubsCards { get; set; }

        public List<ProjectObj> GetAnyRentProjectObj { set; get; }
    }
}